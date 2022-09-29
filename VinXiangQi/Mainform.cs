using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Yolov5Net;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models;
using System.Drawing.Drawing2D;
using System.Threading;
using System.IO;
using Newtonsoft.Json;


namespace VinXiangQi
{
    public partial class Mainform : Form
    {
        public Mainform()
        {
            InitializeComponent();
        }

        // 程序版本
        public static string Version = "1.3.5";

        // 用于截图的窗口句柄
        public static IntPtr GameHandle = IntPtr.Zero;
        // 用于点击的窗口句柄
        public static IntPtr ClickHandle = IntPtr.Zero;
        // 棋盘识别模型
        public static Dictionary<string, YoloScorer<YoloXiangQiModel>> ModelList = new Dictionary<string, YoloScorer<YoloXiangQiModel>>();
        public static YoloScorer<YoloXiangQiModel> Model;
        // UCI引擎封装
        public static EngineHelper Engine;
        // 开局库
        public static Dictionary<string, OpenBookHelper.OpenBook> OpenBookList = new Dictionary<string, OpenBookHelper.OpenBook>();
        // 用于储存方案供选择
        public static Dictionary<string, Solution> SolutionList = new Dictionary<string, Solution>();
        // 当前选择的方案
        public static Solution CurrentSolution;
        // 用于存放每个棋盘上的点对应图片上点击坐标
        public static Point[,] ClickPositionMap = new Point[9, 10];
        // 当GameHandle和ClickHandle不是同一个时，获取他们尺寸的插值作为图片上坐标和点击坐标转换的偏移量
        public static Size ClickOffset = new Size(0, 0);
        // 用于截图和识别的主线程
        Thread DetectionThread;
        // 用于自动点击的线程
        Thread AutoClickThread;
        // 用于在原始截图上画出检测框和分类
        Bitmap YoloDisplayBitmap;
        Graphics YoloGDI;
        // 用于绘制示意棋盘
        Bitmap BoardDisplayBitmap = new Bitmap(400, 440);
        Graphics BoardGDI;
        // 用于在示意棋盘上显示箭头
        ChessMove BestMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
        ChessMove PonderMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
        ChessMove BackgroundAnalysisMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
        // Flag 用于在程序退出时示意其他子线程退出
        public static bool Running = true;
        // 用于判断引擎当前是否存在别的分析任务，是否需要给引擎发送stop，是否需要跳过发送stop产生的bestmove
        public static int EngineAnalyzeCount = 0;
        // 全局设置
        public static ProgramSettings Settings = new ProgramSettings();
        public static string SettingsPath = @".\settings.json";
        public static string MODEL_FOLDER = @".\Models\";
        public static string OPENBOOK_FOLDER = @".\OpenBooks\";
        public static string SOLUTION_FOLDER = @".\Solutions\";
        // 是否开始连线
        public static bool DetectEnabled = false;
        // 自动点击的图片
        public static List<Bitmap> AutoClickImageList = new List<Bitmap>();
        public static Random Rand = new Random();

        public class Solution
        {
            public string ScreenshotCaption;
            public string ScreenshotClass;
            public string ClickCaption;
            public string ClickClass;
            public Solution(string ScreenshotCaption, string ScreenshotClass, string ClickCaption, string ClickClass)
            {
                this.ScreenshotCaption = ScreenshotCaption;
                this.ScreenshotClass = ScreenshotClass;
                this.ClickCaption = ClickCaption;
                this.ClickClass = ClickClass;
            }
        }

        public struct ChessMove
        {
            public ChessMove(Point from, Point to)
            {
                this.From = from;
                this.To = to;
            }
            public Point From;
            public Point To;
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            try
            {
                StatisticsHelper.Statistics();
                this.Text = $"Vin象棋 {Version}";
                InitFolders();
                LoadSettings();
                InitOpenBooks();
                InitYoloModels();
                InitSolutions();
                InitSettingsUI();
                InitThreads();
                InitResultDisplay();
                InitEngine();
                Settings.BackgroundAnalysis = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadSettings()
        {
            if (File.Exists(SettingsPath))
            {
                Settings = JsonConvert.DeserializeObject<ProgramSettings>(File.ReadAllText(SettingsPath));
            }
        }

        public static void SaveSettings()
        {
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(Settings));
        }

        void InitThreads()
        {
            DetectionThread = new Thread(new ThreadStart(DetectionLoop));
            DetectionThread.Start();
            AutoClickThread = new Thread(new ThreadStart(AutoClickLoop));
            AutoClickThread.Start();
        }

        void InitFolders()
        {
            if (!Directory.Exists(MODEL_FOLDER))
            {
                Directory.CreateDirectory(MODEL_FOLDER);
                Thread.Sleep(200);
            }
            if (!Directory.Exists(SOLUTION_FOLDER))
            {
                Directory.CreateDirectory(SOLUTION_FOLDER);
                Thread.Sleep(200);
            }
            if (!Directory.Exists(OPENBOOK_FOLDER))
            {
                Directory.CreateDirectory(OPENBOOK_FOLDER);
            }
        }

        public static void InitOpenBooks()
        {
            foreach (var book in OpenBookList)
            {
                try
                {
                    book.Value.Dispose();
                }
                catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
            }
            OpenBookList.Clear();
            foreach (var file in Directory.GetFiles(OPENBOOK_FOLDER))
            {
                if (file.Split('.').Last() == "obk")
                {
                    try
                    {
                        string[] parts = file.Split('\\').Last().Split('.');
                        string name = string.Join(".", parts.Take(parts.Length - 1));
                        var book = new OpenBookHelper.OpenBook(file);
                        OpenBookList.Add(name, book);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("加载开局库 " + file + " 失败：" + file + "\n" + ex.ToString());
                    }
                }
            }
        }

        void InitYoloModels()
        {
            foreach (string file in Directory.GetFiles("./Models"))
            {
                if (file.EndsWith(".onnx"))
                {
                    string modelName = file.Split('\\').Last().Replace(".onnx", "");
                    var model = new YoloScorer<YoloXiangQiModel>(file);
                    ModelList.Add(modelName, model);
                    if (Settings.YoloModel == modelName)
                    {
                        Model = model;
                    }
                }
            }
        }

        void InitSolutions()
        {
            if (!Directory.Exists(@".\Solutions"))
            {
                Directory.CreateDirectory(@".\Solutions");
            }
            comboBox_solution.Items.Clear();
            SolutionList.Clear();
            string[] paramsKeys = "截图标题 截图类 点击标题 点击类".Split(' ');
            foreach (var folder in Directory.GetDirectories(@".\Solutions"))
            {
                if (folder.EndsWith(@"\自定义方案")) continue;
                string file = folder + @"\window.txt";
                if (!File.Exists(file)) continue;
                string[] lines = File.ReadAllLines(file);
                if (lines.Length == 4)
                {
                    List<string> paramList = new List<string>();
                    for (int i = 0; i < 4; i++)
                    {
                        string line = lines[i];
                        string[] args = line.Split('=');
                        if (args[0] != paramsKeys[i])
                        {
                            MessageBox.Show("方案: " + file + " 格式错误: 第" + (i + 1) + "行应为" + paramsKeys[i]);
                            break;
                        }
                        if (args[1] != "")
                        {
                            paramList.Add(args[1]);
                        }
                        else
                        {
                            paramList.Add(null);
                        }
                    }
                    SolutionList.Add(folder.Split('\\').Last(), new Solution(paramList[0], paramList[1], paramList[2], paramList[3]));
                }
                else
                {
                    MessageBox.Show("方案: " + file + " 格式错误");
                }
            }
            comboBox_solution.Items.Clear();
            foreach (var solution in SolutionList)
            {
                comboBox_solution.Items.Add(solution.Key);
                if (solution.Key == Settings.SelectedSolution)
                {
                    comboBox_solution.SelectedIndex = comboBox_solution.Items.Count - 1;
                }
            }
            if (comboBox_solution.SelectedItem == null)
            {
                if (comboBox_solution.Items.Count > 0)
                {
                    Settings.SelectedSolution = comboBox_solution.Items[0].ToString();
                    comboBox_solution.SelectedIndex = 0;
                }
                else
                {
                    Settings.SelectedSolution = "";
                }
                SaveSettings();
            }
        }

        void InitSettingsUI()
        {
            // 引擎多选框
            comboBox_engine.Items.Clear();
            comboBox_engine.Text = "";
            int selectedIndex = 0;
            foreach (var engine in Settings.EngineList)
            {
                comboBox_engine.Items.Add(engine.Key);
                if (engine.Key == Settings.SelectedEngine)
                {
                    selectedIndex = comboBox_engine.Items.Count - 1;
                }
            }
            if (comboBox_engine.Items.Count > 0)
            {
                comboBox_engine.SelectedIndex = selectedIndex;
                Settings.SelectedEngine = comboBox_engine.Items[selectedIndex].ToString();
                SaveSettings();
            }
            // 线程数
            numericUpDown_thread_count.Value = Settings.ThreadCount;
            // 步时
            numericUpDown_step_time.Value = (decimal)Settings.EngineStepTime;
            // 思考深度
            numericUpDown_engine_depth.Value = (decimal)Settings.EngineDepth;
            // 自动走棋
            checkBox_auto_go.Checked = Settings.AutoGo;
            // 缩放比
            numericUpDown_scale_factor.Value = (decimal)Settings.ScaleFactor;
            // 通用截图
            checkBox_universal_mode.Checked = Settings.UniversalMode;
            // 通用鼠标
            checkBox_universal_mouse.Checked = Settings.UniversalMouse;
            // 分析模式
            checkBox_analyze_mode.Checked = Settings.AnalyzingMode;
            // 自动点击
            checkBox_auto_click.Checked = Settings.AutoClick;
            // 绝杀自动立即走棋
            checkBox_stop_when_mate.Checked = Settings.StopWhenMate;
            // 后台思考
            checkBox_background_analysis.Checked = Settings.BackgroundAnalysis;
            // 开局库最短时间
            numericUpDown_min_time.Value = (decimal)Settings.MinTimeUsingOpenbook;
            // 自动走棋分数
            numericUpDown_stop_score.Value = (decimal)Settings.StopScore;
            // Yolo模型选择
            comboBox_yolo_models.Items.Clear();
            foreach (var yolo in ModelList)
            {
                comboBox_yolo_models.Items.Add(yolo.Key);
                if (yolo.Key == Settings.YoloModel)
                {
                    comboBox_yolo_models.SelectedIndex = comboBox_yolo_models.Items.Count - 1;
                }
            }
            if (comboBox_yolo_models.Items.Count > 0)
            {
                if (comboBox_yolo_models.SelectedItem == null) comboBox_yolo_models.SelectedIndex = 0;
            }
        }

        void InitResultDisplay()
        {
            pictureBox_board.Image = BoardDisplayBitmap;
            BoardGDI = Graphics.FromImage(BoardDisplayBitmap);
            RenderDisplayBoard();
        }

        void InitEngine()
        {
            if (Engine != null)
            {
                Engine.Stop();
            }
            if (Settings.SelectedEngine.Length > 0 && Settings.EngineList.ContainsKey(Settings.SelectedEngine))
            {
                EngineSettings engineSettings = Settings.EngineList[Settings.SelectedEngine];
                if (File.Exists(engineSettings.ExePath))
                {
                    var configs = Settings.EngineList[Settings.SelectedEngine].Configs;
                    if (configs.ContainsKey("Threads"))
                    {
                        configs["Threads"] = Settings.ThreadCount.ToString();
                    }
                    else
                    {
                        configs.Add("Threads", Settings.ThreadCount.ToString());
                    }
                    Engine = new EngineHelper(engineSettings.ExePath, configs);
                    Engine.BestMoveEvent += Engine_BestMove_Event;
                    Engine.InfoEvent += Engine_InfoEvent;
                    Engine.Init();
                }

            }
        }

        void ResetDetection()
        {
            Engine.StopAnalyze();
            Engine.AnalyzeQueue.Clear();
            Engine.SkipList.Clear();
            InvalidCount = 0;
            if (!string.IsNullOrEmpty(Settings.SelectedSolution))
            {
                string key = Settings.SelectedSolution;
                CurrentSolution = SolutionList[key];
                GameHandle = ScreenshotHelper.FindWindow(CurrentSolution.ScreenshotClass, CurrentSolution.ScreenshotCaption);
                if (CurrentSolution.ClickClass == null && CurrentSolution.ClickCaption == null)
                {
                    ClickHandle = GameHandle;
                }
                else
                {
                    ClickHandle = ScreenshotHelper.FindWindowEx(GameHandle, IntPtr.Zero, CurrentSolution.ClickClass, CurrentSolution.ClickCaption);
                }
            }
            ReloadBoard();
            BoardArea = new Rectangle(-1, -1, -1, -1);
            EngineAnalyzeCount = 0;
            CurrentBoard = null;
            LastBoard = null;
            PendingBoard = null;
            ExpectedSelfGoBoard = null;
            ExpectedMove = "";
            ExpectedBoardMap.Clear();
        }

        void StartDetection(bool fromOpponent)
        {
            if (Engine == null)
            {
                DetectEnabled = false;
                MessageBox.Show("引擎未加载！");
                ModeDisplay();
                return;
            }
            if (checkBox_debug.Checked)
            {
                checkBox_debug.Checked = false;
            }
            if (BackgroundAnalyzing)
            {
                BackgroundAnalyzing = false;
            }
            TurnToOpponent = fromOpponent;
            DetectEnabled = true;
            ResetDetection();
            ModeDisplay();
        }

        void DisplayStatus(string status)
        {
            Debug.WriteLine(status);
            this.Invoke(new Action(() => label_status.Text = "识别状态: " + status));
        }

        void ModeDisplay(string mode="")
        {
            if (string.IsNullOrEmpty(mode))
            {
                mode = DetectEnabled ? "识别中" : "空闲";
            }
            Debug.WriteLine(mode);
            this.Invoke(new Action(() => label_detection_status.Text = "连线状态: " + mode));
        }

        void MouseLeftClient_2Point(int x1, int y1, int x2, int y2)
        {
            if (Settings.UniversalMouse)
            {
                var originalPos = MouseHelper.MouseOperations.GetCursorPosition();
                var windowPos = ScreenshotHelper.GetWindowRectangle(GameHandle);
                MouseHelper.MouseOperations.SetCursorPosition(x1 + windowPos.X, y1 + windowPos.Y);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftDown);
                Thread.Sleep(50);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftUp);
                Thread.Sleep(50);
                MouseHelper.MouseOperations.SetCursorPosition(x2 + windowPos.X, y2 + windowPos.Y);
                Thread.Sleep(50);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftDown);
                Thread.Sleep(50);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftUp);
                Thread.Sleep(50);
                MouseHelper.MouseOperations.SetCursorPosition(originalPos.X, originalPos.Y);
            }
            else
            {
                MouseHelper.MouseLeftClick(ClickHandle, x1, y1);
                Thread.Sleep(300);
                MouseHelper.MouseLeftClick(ClickHandle, x2, y2);
            }
        }

        void MouseLeftClick(int x, int y)
        {
            
            if (Settings.UniversalMouse)
            {
                var originalPos = MouseHelper.MouseOperations.GetCursorPosition();
                var windowPos = ScreenshotHelper.GetWindowRectangle(GameHandle);
                MouseHelper.MouseOperations.SetCursorPosition(x + windowPos.X, y + windowPos.Y);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftDown);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftUp);
                MouseHelper.MouseOperations.SetCursorPosition(originalPos.X, originalPos.Y);
                //Thread.Sleep(50);
                //MouseHelper.MouseOperations.SetCursorPosition(originalPos.X, originalPos.Y);
            }
            else
            {
                MouseHelper.MouseLeftClick(ClickHandle, x, y);
            }
        }

        void GetHandle()
        {
            Task.Run(new Action(() =>
            {
                DisplayStatus("请点击目标窗体并等待 准备获取游戏窗口句柄...2");
                Thread.Sleep(1000);
                DisplayStatus("请点击目标窗体并等待 准备获取游戏窗口句柄...1");
                Thread.Sleep(1000);
                GameHandle = ScreenshotHelper.GetForegroundWindow();
                ClickHandle = (IntPtr)ScreenshotHelper.WindowFromPoint(Cursor.Position.X, Cursor.Position.Y);
                DisplayStatus("游戏窗口句柄: " + GameHandle);
                ScreenshotHelper.WindowHandleInfo handleInfo = new ScreenshotHelper.WindowHandleInfo(GameHandle);
                var handles = handleInfo.GetAllChildHandles();
                Debug.WriteLine("GameHandle " + GameHandle + " " + ScreenshotHelper.GetWindowTitle(GameHandle));
                Debug.WriteLine(ScreenshotHelper.GetWindowRectangleWithShadow(GameHandle));
                Debug.WriteLine("ClickHandle " + ClickHandle + " " + ScreenshotHelper.GetWindowTitle(ClickHandle));
                Debug.WriteLine(ScreenshotHelper.GetWindowRectangleWithShadow(ClickHandle));
                Settings.SelectedSolution = "";
                foreach (var handle in handles)
                {
                    Debug.WriteLine(handle + " " + ScreenshotHelper.GetWindowTitle(handle));
                    Debug.WriteLine(ScreenshotHelper.GetWindowRectangleWithShadow(handle));
                    Debug.WriteLine(ScreenshotHelper.GetWindowClass(handle));
                }    
                this.Invoke(new Action(() =>
                {
                    comboBox_solution.SelectedItem = null;
                    comboBox_solution.Text = "自定义方案";
                }));
            }));
        }

        Bitmap Screenshot()
        {
            try
            {
                if (Settings.SelectedSolution == "剪切板")
                {
                    if (Clipboard.ContainsImage())
                    {
                        Debug.WriteLine("Before " + Clipboard.ContainsImage().ToString());
                        Bitmap img = (Bitmap)Clipboard.GetImage().Clone();
                        Debug.WriteLine("After " + Clipboard.ContainsImage().ToString());
                        Debug.WriteLine("-=-=-=-=-=-==-=");
                        return img;
                    }
                    else
                    {
                        Debug.WriteLine("NO Clipboard Image");
                        Bitmap no_image = new Bitmap(150, 100);
                        Graphics gdi = Graphics.FromImage(no_image);
                        gdi.DrawString("剪切板中没有图片", this.Font, Brushes.Black, 0, 40);
                        gdi.Dispose();
                        return no_image;
                    }
                }
                if (Settings.UniversalMode)
                {
                    Rectangle rect;
                    if (Settings.UniversalMouse)
                    {
                        rect = ScreenshotHelper.GetWindowRectangle(GameHandle);
                    }
                    else
                    {
                        rect = ScreenshotHelper.GetWindowRectWithoutTitle(GameHandle);
                    }
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        Bitmap b = new Bitmap(rect.Width, rect.Height);
                        Graphics g = Graphics.FromImage(b);
                        g.CopyFromScreen(rect.Location, new Point(0, 0), rect.Size);
                        return b;
                    }
                    else
                    {
                        return new Bitmap(1, 1);
                    }
                }
                else
                {
                    return ScreenshotHelper.GetWindowCapture(GameHandle);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                GameHandle = IntPtr.Zero;
                ClickHandle = IntPtr.Zero;
                return new Bitmap(1, 1);
            }
        }

        void LoadCurrentSolution()
        {
            string[] imgExtensions = new string[] { "png", "jpg", "bmp", "jpeg" };
            if (Settings.SelectedSolution == "剪切板")
            {
                ClickHandle = (IntPtr)1;
                GameHandle = (IntPtr)1;
                return;
            }
            if (Settings.SelectedSolution != "" && Settings.SelectedSolution != "自定义方案")
            {
                string key = Settings.SelectedSolution;
                CurrentSolution = SolutionList[key];
                GameHandle = ScreenshotHelper.FindWindow(CurrentSolution.ScreenshotClass, CurrentSolution.ScreenshotCaption);
                if (CurrentSolution.ClickClass == null && CurrentSolution.ClickCaption == null)
                {
                    ClickHandle = GameHandle;
                    ClickOffset = new Size(0, 0);
                }
                else
                {
                    ScreenshotHelper.WindowHandleInfo handleInfo = new ScreenshotHelper.WindowHandleInfo(GameHandle);
                    var childHandles = handleInfo.GetAllChildHandles();
                    foreach (var child in childHandles)
                    {
                        string childTitle = ScreenshotHelper.GetWindowTitle(child);
                        string childClass = ScreenshotHelper.GetWindowClass(child);
                        if ((CurrentSolution.ClickCaption == null || childTitle == CurrentSolution.ClickCaption) &&
                            (CurrentSolution.ClickClass == null || childClass == CurrentSolution.ClickClass))
                        {
                            ClickHandle = child;
                            break;
                        }
                    }
                    ClickHandle = ScreenshotHelper.FindWindowEx(GameHandle, IntPtr.Zero, CurrentSolution.ClickClass, CurrentSolution.ClickCaption);
                    Rectangle gameRect = ScreenshotHelper.GetWindowRectangle(GameHandle);
                    Rectangle clickRect = ScreenshotHelper.GetWindowRectangleWithShadow(ClickHandle);
                    ClickOffset = new Size(0, clickRect.Height - gameRect.Height);
                }

                string autoClickPath = @".\Solutions\" + Settings.SelectedSolution + @"\AutoClick";
                if (Directory.Exists(autoClickPath))
                {
                    AutoClickImageList.Clear();
                    foreach (var image in Directory.GetFiles(autoClickPath))
                    {
                        if (imgExtensions.Contains(image.Split('.').Last()))
                        {
                            Bitmap tmpImg = new Bitmap(image);
                            Bitmap img = new Bitmap(tmpImg);
                            tmpImg.Dispose();
                            AutoClickImageList.Add(img);
                        }
                    }
                }
                SaveSettings();
            }
            else if(Settings.SelectedSolution != "自定义方案")
            {
                if (Directory.Exists(@".\Solutions\自定义方案") && Directory.Exists(@".\Solutions\自定义方案\AutoClick"))
                {
                    AutoClickImageList.Clear();
                    foreach (var image in Directory.GetFiles(@".\Solutions\自定义方案\AutoClick"))
                    {
                        if (imgExtensions.Contains(image.Split('.').Last()))
                        {
                            Bitmap tmpImg = new Bitmap(image);
                            Bitmap img = new Bitmap(tmpImg);
                            tmpImg.Dispose();
                            AutoClickImageList.Add(img);
                        }
                    }
                }
            }
        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            Running = false;
            if (Engine != null)
            {
                Engine.Stop();
            }
            foreach (var book in OpenBookList)
            {
                try
                {
                    book.Value.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private void timer_set_window_hwnd_Tick(object sender, EventArgs e)
        {
            if (MouseHelper.GetAsyncKeyState(Keys.F2))
            {
                GetHandle();
            }
        }

        private void pictureBox_board_MouseDown(object sender, MouseEventArgs e)
        {
            bool MouseOnImage = false;
            Point MousePositionOnImage = new Point(-1, -1);
            double img_ratio = (double)BoardDisplayBitmap.Width / (double)BoardDisplayBitmap.Height;
            double box_ratio = (double)pictureBox_board.Width / (double)pictureBox_board.Height;
            double display_ratio;
            int display_x, display_y, display_width, display_height;
            if (box_ratio >= img_ratio)
            {
                display_y = 0;
                display_height = pictureBox_board.Height;
                display_x = (int)(pictureBox_board.Width - display_height * img_ratio) / 2;
                display_width = (int)(display_height * img_ratio);
                display_ratio = (double)BoardDisplayBitmap.Height / display_height;
            }
            else
            {
                display_x = 0;
                display_width = pictureBox_board.Width;
                display_y = (int)(pictureBox_board.Height - display_width / img_ratio) / 2;
                display_height = (int)(display_width / img_ratio);
                display_ratio = (double)BoardDisplayBitmap.Width / display_width;
            }
            int img_x, img_y;
            bool mouse_on_x, mouse_on_y;
            if (e.X <= display_x)
            {
                img_x = 0;
                mouse_on_x = false;
            }
            else if (e.X >= display_x + display_width)
            {
                img_x = BoardDisplayBitmap.Width - 1;
                mouse_on_x = false;
            }
            else
            {
                img_x = (int)((e.X - display_x) * display_ratio);
                mouse_on_x = true;
            }
            if (e.Y <= display_y)
            {
                img_y = 0;
                mouse_on_y = false;
            }
            else if (e.Y >= display_y + display_height)
            {
                img_y = BoardDisplayBitmap.Height - 1;
                mouse_on_y = false;
            }
            else
            {
                img_y = (int)((e.Y - display_y) * display_ratio);
                mouse_on_y = true;
            }
            if (mouse_on_x && mouse_on_y)
            {
                MouseOnImage = true;
                MousePositionOnImage = new Point(img_x, img_y);
            }
            else
            {
                MouseOnImage = false;
            }
            if (MouseOnImage)
            {
                int width = 40;
                int height = 40;
                int xoffset = width / 2;
                int yoffset = height / 2;
                Point pos = new Point(-1, -1);
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        if (MousePositionOnImage.X >= x * width + xoffset && MousePositionOnImage.X <= x * width + xoffset + width &&
                            MousePositionOnImage.Y >= y * height + yoffset && MousePositionOnImage.Y <= y * height + yoffset + height)
                        {
                            pos = new Point(x, y);
                            break;
                        }
                    }
                    if (pos.X != -1) break;
                }
                if (pos.X != -1)
                {
                    Point p = ClickPositionMap[pos.X, pos.Y];
                    MouseLeftClick(p.X, p.Y);
                }
            }
        }

        private void button_redetect_Click(object sender, EventArgs e)
        {
            ResetDetection();
        }

        private void numericUpDown_thread_count_ValueChanged(object sender, EventArgs e)
        {
            string threadStr = numericUpDown_thread_count.Value.ToString();
            if (Engine != null)
            {
                Engine.SetOption("Threads", threadStr);
            }
            Settings.ThreadCount = (int)numericUpDown_thread_count.Value;
            if (Settings.CurrentEngine == null)
            {
                return;
            }
            if (Settings.CurrentEngine.Configs.ContainsKey("Threads"))
            {
                Settings.CurrentEngine.Configs["Threads"] = threadStr;
            }
            else
            {
                Settings.CurrentEngine.Configs.Add("Threads", threadStr);
            }
            SaveSettings();
        }

        private void button_engine_add_Click(object sender, EventArgs e)
        {
            if (openFileDialog_engine.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog_engine.FileName;
                path = path.Replace(Environment.CurrentDirectory, ".");
                if (Settings.EngineList.ContainsKey(path))
                {
                    MessageBox.Show("该引擎已存在");
                    return;
                }
                EngineSettings esettings = new EngineSettings();
                esettings.ExePath = path;
                Settings.EngineList.Add(path, esettings);
                SaveSettings();
                InitSettingsUI();
            }
        }

        private void comboBox_engine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_engine.SelectedItem.ToString() != Settings.SelectedEngine)
            {
                Settings.SelectedEngine = comboBox_engine.SelectedItem.ToString();
                if (!Settings.EngineList.ContainsKey(Settings.SelectedEngine) || !File.Exists(Settings.EngineList[Settings.SelectedEngine].ExePath))
                {
                    MessageBox.Show("引擎文件不存在！");
                    return;
                }
                InitEngine();
            }
        }

        private void button_engine_delete_Click(object sender, EventArgs e)
        {
            if (Settings.SelectedEngine.Length > 0)
            {
                if (Engine != null) Engine.Stop();
                Settings.EngineList.Remove(Settings.SelectedEngine);
                if (Settings.EngineList.Count > 0)
                {
                    Settings.SelectedEngine = Settings.EngineList.Keys.First();
                }
                else
                {
                    Settings.SelectedEngine = "";
                }
                SaveSettings();
                InitSettingsUI();
            }
        }

        private void numericUpDown_step_time_ValueChanged(object sender, EventArgs e)
        {
            Settings.EngineStepTime = (double)numericUpDown_step_time.Value;
            SaveSettings();
        }

        private void button_get_hwnd_Click(object sender, EventArgs e)
        {
            button_stop_detection_Click(sender, e);
            GetHandle();
        }

        private void numericUpDown_scale_factor_ValueChanged(object sender, EventArgs e)
        {
            Settings.ScaleFactor = 1.0f / (float)numericUpDown_scale_factor.Value;
            SaveSettings();
        }

        private void checkBox_universal_mode_CheckedChanged(object sender, EventArgs e)
        {
            Settings.UniversalMode = checkBox_universal_mode.Checked;
            SaveSettings();
        }

        private void checkBox_auto_go_CheckedChanged(object sender, EventArgs e)
        {
            Settings.AutoGo = checkBox_auto_go.Checked;
            if (Settings.AutoGo)
            {
                checkBox_analyze_mode.Checked = false;
            }
            SaveSettings();
        }

        private void comboBox_solution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_solution.SelectedItem != null)
            {
                Settings.SelectedSolution = comboBox_solution.SelectedItem.ToString();
                SaveSettings();
                LoadCurrentSolution();
            }
            else
            {
                if (comboBox_solution.Text == "自定义方案")
                {
                    Settings.SelectedSolution = "自定义方案";
                    SaveSettings();
                    LoadCurrentSolution();
                }                    
            }
        }

        private void ToolStripMenuItem_copy_fen_Click(object sender, EventArgs e)
        {
            string fen = Utils.BoardToFen(CurrentBoard,RedSide ? "w" : "b",RedSide ? "w" : "b");
            Clipboard.SetText(fen);
            MessageBox.Show("局面: \n" + fen + "\n 已经复制到剪贴板");
        }

        private void checkBox_debug_CheckedChanged(object sender, EventArgs e)
        {
            ScreenDebug = checkBox_debug.Checked;
            if (checkBox_debug.Checked)
            {
                if (DetectEnabled)
                {
                    DetectEnabled = false;
                    ModeDisplay();
                }
            }
        }

        private void checkBox_analyze_mode_CheckedChanged(object sender, EventArgs e)
        {
            if (Engine != null)
            {
                Engine.StopAnalyze();
                Engine.AnalyzeQueue.Clear();
                Engine.SkipList.Clear();
            }
            Settings.AnalyzingMode = checkBox_analyze_mode.Checked;
            if (Settings.AnalyzingMode)
            {
                if (Settings.AutoGo)
                {
                    checkBox_auto_go.Checked = false;
                }
            }
            SaveSettings();
        }

        private void comboBox_yolo_models_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_yolo_models.SelectedItem != null)
            {
                Settings.YoloModel = comboBox_yolo_models.SelectedItem.ToString();
            }
            Model = ModelList[Settings.YoloModel];
            SaveSettings();
        }

        private void button_engine_settings_Click(object sender, EventArgs e)
        {
            EngineManageForm engineManageForm = new EngineManageForm();
            engineManageForm.ShowDialog();
            if (!Settings.EngineList.ContainsKey(Settings.SelectedEngine))
            {
                if (Settings.EngineList.Count > 0)
                {
                    Settings.SelectedEngine = Settings.EngineList.Keys.ToList()[0];
                    InitEngine();
                }
                else
                {
                    Settings.SelectedEngine = "";
                }
                SaveSettings();
            }
            InitSettingsUI();
        }

        private void checkBox_universal_mouse_CheckedChanged(object sender, EventArgs e)
        {
            Settings.UniversalMouse = checkBox_universal_mouse.Checked;
            SaveSettings();
        }

        private void button_screenshot_Click(object sender, EventArgs e)
        {
            Bitmap screen = Screenshot();
            if (!Directory.Exists(@".\Solutions\" + Settings.SelectedSolution))
            {
                Directory.CreateDirectory(@".\Solutions\" + Settings.SelectedSolution);
            }
            ImageEditForm editForm = new ImageEditForm(screen, DateTime.Now.ToString("yyyyMMddHHmmss"), @".\Solutions\" + Settings.SelectedSolution + @"\AutoClick");
            editForm.StartPosition = FormStartPosition.CenterParent;
            editForm.Text = "方案 " + Settings.SelectedSolution + " 自动点击图片管理";
            editForm.ShowDialog();
            InitSolutions();
        }

        private void checkBox_auto_click_CheckedChanged(object sender, EventArgs e)
        {
            Settings.AutoClick = checkBox_auto_click.Checked;
            SaveSettings();
        }

        private void checkBox_stop_when_mate_CheckedChanged(object sender, EventArgs e)
        {
            Settings.StopWhenMate = checkBox_stop_when_mate.Checked;
            SaveSettings();
        }

        private void button_go_immediately_Click(object sender, EventArgs e)
        {
            Engine.StopAnalyze();
        }

        private void button_openbook_settings_Click(object sender, EventArgs e)
        {
            OpenBookSettingsForm opsf = new OpenBookSettingsForm();
            opsf.StartPosition = FormStartPosition.CenterParent;
            opsf.ShowDialog();
        }

        private void button_save_as_solution_Click(object sender, EventArgs e)
        {
            SolutionSavingForm ssf = new SolutionSavingForm();
            ssf.StartPosition = FormStartPosition.CenterParent;
            ssf.ShowDialog();
            InitSolutions();
        }

        private void ToolStripMenuItem_about_Click(object sender, EventArgs e)
        {
            AboutForm abf = new AboutForm();
            abf.StartPosition = FormStartPosition.CenterParent;
            abf.Show();
        }

        private void checkBox_background_analysis_CheckedChanged(object sender, EventArgs e)
        {
            Settings.BackgroundAnalysis = checkBox_background_analysis.Checked;
            SaveSettings();
        }

        private void button_start_from_self_Click(object sender, EventArgs e)
        {
            StartDetection(false);
        }

        private void button_start_from_oppo_Click(object sender, EventArgs e)
        {
            StartDetection(true);
        }

        private void button_stop_detection_Click(object sender, EventArgs e)
        {
            DetectEnabled = false;
            checkBox_debug.Checked = true;
            if (Engine != null)
            {
                Engine.Stop();
            }
            ModeDisplay();
        }

        private void numericUpDown_min_time_ValueChanged(object sender, EventArgs e)
        {
            Settings.MinTimeUsingOpenbook = (double)numericUpDown_min_time.Value;
            SaveSettings();
        }

        private void numericUpDown_engine_depth_ValueChanged(object sender, EventArgs e)
        {
            Settings.EngineDepth = (int)numericUpDown_engine_depth.Value;
            SaveSettings();
        }

        private void numericUpDown_stop_score_ValueChanged(object sender, EventArgs e)
        {
            Settings.StopScore = (int)numericUpDown_stop_score.Value;
            SaveSettings();
        }

        private void button_advance_settings_Click(object sender, EventArgs e)
        {
            DetectionSettingsForm detectionSettingsForm = new DetectionSettingsForm();
            detectionSettingsForm.StartPosition = FormStartPosition.CenterParent;
            detectionSettingsForm.ShowDialog();
        }

        private void pictureBox_show_result_Click(object sender, EventArgs e)
        {
            if (pictureBox_show_result.Image != null)
            {
                ImageDisplayForm imageDisplayForm = new ImageDisplayForm(pictureBox_show_result.Image);
                imageDisplayForm.Show();
            }
        }

        private void button_clipboard_solution_Click(object sender, EventArgs e)
        {
            if (SolutionList.ContainsKey("剪切板"))
            {
                Settings.SelectedSolution = "剪切板";
            }
            else
            {
                SolutionList.Add("剪切板", new Solution("", "", "", ""));
                Settings.SelectedSolution = "剪切板";
            }
        }
    }
}
