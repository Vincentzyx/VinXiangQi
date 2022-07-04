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

        // 用于截图的窗口句柄
        public static IntPtr GameHandle = IntPtr.Zero;
        // 用于点击的窗口句柄
        public static IntPtr ClickHandle = IntPtr.Zero;
        // 用来储存Yolo模型路径供选择
        public static List<string> YoloModels = new List<string>();
        // 棋盘识别模型
        public static YoloScorer<YoloXiangQiModel> Model;
        // UCCI引擎封装
        public static EngineHelper Engine;
        // 用于储存方案供选择
        public static Dictionary<string, Solution> SolutionList = new Dictionary<string, Solution>();
        // 当前选择的方案
        public static Solution CurrentSolution;
        // 主棋盘
        public static string[,] Board = new string[9, 10];
        // 上一个棋盘，用于比对判断当前识别状态是否存在动画等非法清空
        public static string[,] LastBoard = new string[9, 10];
        // 当软件自动走棋时，将该棋盘设为走棋后的状态，当Board变成ExpectedBoard状态时，不触发引擎计算
        public static string[,] ExpectedBoard = new string[9, 10];
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
        // Flag 用于在程序退出时示意其他子线程退出
        public static bool Running = true;
        // 用于判断引擎当前是否存在别的分析任务，是否需要给引擎发送stop，是否需要跳过发送stop产生的bestmove
        public static int EngineAnalyzeCount = 0;
        public static int EngineSkipCount = 0;
        // 全局设置
        public static ProgramSettings Settings = new ProgramSettings();
        public static string SettingsPath = "./settings.json";
        // 是否开始连线
        public static bool DetectEnabled = false;
        // 自动点击的图片
        public static List<Bitmap> AutoClickImageList = new List<Bitmap>();

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
                InitFolders();
                LoadSettings();
                InitYoloModels();
                InitSolutions();
                InitSettingsUI();
                InitThreads();
                InitResultDisplay();
                InitEngine();
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
            if (!Directory.Exists("./Models"))
            {
                Directory.CreateDirectory("./Models");
                Thread.Sleep(200);
            }
            if (!Directory.Exists("./Solutions"))
            {
                Directory.CreateDirectory("./Solutions");
                Thread.Sleep(200);
            }
        }

        void InitYoloModels()
        {
            foreach (string file in Directory.GetFiles("./Models"))
            {
                if (file.EndsWith(".onnx"))
                {
                    YoloModels.Add(file.Split('\\').Last());
                }
            }
            if (Settings.YoloModel != "")
            {
                if (File.Exists("./Models/" + Settings.YoloModel))
                {
                    Model = new YoloScorer<YoloXiangQiModel>("./Models/" + Settings.YoloModel);
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
            numericUpDown_step_time.Value = (decimal)Settings.StepTime;
            // 自动走棋
            checkBox_auto_go.Checked = Settings.AutoGo;
            // 缩放比
            numericUpDown_scale_factor.Value = (decimal)Settings.ScaleFactor;
            // 红黑方
            radioButton_side_red.Checked = Settings.RedSide;
            radioButton_side_black.Checked = !Settings.RedSide;
            // 持续检测
            checkBox_keep_detect.Checked = Settings.KeepDetecting;
            if (Settings.KeepDetecting)
            {
                label_detect_mode.Text = "识别模式: 持续识别";
            }
            else
            {
                label_detect_mode.Text = "识别模式: 动画结束后识别";
            }
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
            // Yolo模型选择
            comboBox_yolo_models.Items.Clear();
            foreach (string yolo in YoloModels)
            {
                comboBox_yolo_models.Items.Add(yolo);
                if (yolo == Settings.YoloModel)
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
                if (File.Exists(Settings.SelectedEngine))
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
                    Engine = new EngineHelper(Settings.SelectedEngine, configs);
                    Engine.BestMoveEvent += Engine_BestMoveEvent;
                    Engine.InfoEvent += Engine_InfoEvent;
                    Engine.Init();
                }

            }
        }

        void DisplayStatus(string status)
        {
            Debug.WriteLine(status);
            this.Invoke(new Action(() => label_status.Text = "识别状态: " + status));
        }

        void MouseLeftClient_2Point(int x1, int y1, int x2, int y2)
        {
            if (Settings.UniversalMouse)
            {
                var originalPos = MouseHelper.MouseOperations.GetCursorPosition();
                var windowPos = ScreenshotHelper.GetWindowRectangle(GameHandle);
                MouseHelper.MouseOperations.SetCursorPosition(x1 + windowPos.X, y1 + windowPos.Y);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftDown);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftUp);
                MouseHelper.MouseOperations.SetCursorPosition(x2 + windowPos.X, y2 + windowPos.Y);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftDown);
                MouseHelper.MouseOperations.MouseEvent(MouseHelper.MouseOperations.MouseEventFlags.LeftUp);
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
                //MouseHelper.MouseOperations.SetCursorPosition(originalPos.X, originalPos.Y);
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
                this.Invoke(new Action(() =>
                {
                    comboBox_solution.SelectedItem = null;
                    comboBox_solution.Text = "自定义方案";
                }));
            }));
        }

        Bitmap Screenshot()
        {
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

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            Running = false;
            Utils.FreeFenToChina();
            if (Engine != null)
            {
                Engine.Stop();
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
            InvalidCount = 0;
            if (comboBox_solution.SelectedItem != null)
            {
                string key = comboBox_solution.SelectedItem.ToString();
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
            BoardArea = new Rectangle(-1, -1, -1, -1);
            EngineAnalyzeCount = 0;
            LastBoard = null;
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
                if (!File.Exists(Settings.SelectedEngine))
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
            Settings.StepTime = (double)numericUpDown_step_time.Value;
            SaveSettings();
        }

        private void radioButton_side_red_CheckedChanged(object sender, EventArgs e)
        {
            Settings.RedSide = radioButton_side_red.Checked;
            SaveSettings();
        }

        private void button_get_hwnd_Click(object sender, EventArgs e)
        {
            GetHandle();
        }

        private void numericUpDown_scale_factor_ValueChanged(object sender, EventArgs e)
        {
            Settings.ScaleFactor = (float)numericUpDown_scale_factor.Value;
            SaveSettings();
        }

        private void checkBox_keep_detect_CheckedChanged(object sender, EventArgs e)
        {
            Settings.KeepDetecting = checkBox_keep_detect.Checked;
            if (Settings.KeepDetecting)
            {
                label_detect_mode.Text = "识别模式: 持续识别";
            }
            else
            {
                label_detect_mode.Text = "识别模式: 动画结束后识别";
            }
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
            string[] imgExtensions = new string[] { "png", "jpg", "bmp", "jpeg" };
            if (comboBox_solution.SelectedItem != null)
            {
                string key = comboBox_solution.SelectedItem.ToString();
                CurrentSolution = SolutionList[key];
                GameHandle = ScreenshotHelper.FindWindow(CurrentSolution.ScreenshotClass, CurrentSolution.ScreenshotCaption);
                if (CurrentSolution.ClickClass == null && CurrentSolution.ClickCaption == null)
                {
                    ClickHandle = GameHandle;
                    ClickOffset = new Size(0, 0);
                }
                else
                {
                    ClickHandle = ScreenshotHelper.FindWindowEx(GameHandle, IntPtr.Zero, CurrentSolution.ClickClass, CurrentSolution.ClickCaption);
                    Rectangle gameRect = ScreenshotHelper.GetWindowRectangle(GameHandle);
                    Rectangle clickRect = ScreenshotHelper.GetWindowRectangle(ClickHandle);
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
                Settings.SelectedSolution = key;
                SaveSettings();
            }
            else
            {
                if (comboBox_solution.Text == "自定义方案")
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
        }

        private void ToolStripMenuItem_copy_fen_Click(object sender, EventArgs e)
        {
            string fen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b", Settings.RedSide ? "w" : "b");
            Clipboard.SetText(fen);
            MessageBox.Show("局面: \n" + fen + "\n 已经复制到剪贴板");
        }

        private void checkBox_start_connecting_CheckedChanged(object sender, EventArgs e)
        {
            DetectEnabled = checkBox_start_connecting.Checked;
            if (DetectEnabled)
            {
                if (Engine == null)
                {
                    DetectEnabled = false;
                    checkBox_start_connecting.Checked = false;
                    MessageBox.Show("引擎未加载！");
                }
                if (checkBox_debug.Checked)
                {
                    checkBox_debug.Checked = false;
                }
            }
        }

        private void checkBox_debug_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_debug.Checked)
            {
                if (DetectEnabled)
                {
                    checkBox_start_connecting.Checked = false;
                }
            }
        }

        private void checkBox_analyze_mode_CheckedChanged(object sender, EventArgs e)
        {
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
            if (File.Exists("./Models/" + Settings.YoloModel))
            {
                Model = new YoloScorer<YoloXiangQiModel>("./Models/" + Settings.YoloModel);
                SaveSettings();
            }                
            else
            {
                MessageBox.Show("选定的模型文件不存在");
            }
        }

        private void button_engine_settings_Click(object sender, EventArgs e)
        {
            if (Engine != null && Engine.OptionList.Count > 0)
            {
                EngineSettingsForm engineSettingsForm = new EngineSettingsForm();
                engineSettingsForm.Text = Settings.CurrentEngine.ExePath + " 引擎设置";
                engineSettingsForm.ShowDialog();                
            }
            else
            {
                MessageBox.Show("引擎未启动或没有设置选项");
            }
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
    }
}
