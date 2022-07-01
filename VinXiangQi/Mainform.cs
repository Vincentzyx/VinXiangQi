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

        public static IntPtr GameHandle = IntPtr.Zero;
        public static IntPtr ClickHandle = IntPtr.Zero;
        public static YoloScorer<YoloXiangQiModel> Model = new YoloScorer<YoloXiangQiModel>("Models/best.onnx");
        public static string[,] Board = new string[9, 10];
        public static string[,] LastBoard = new string[9, 10];
        public static Point[,] PositionMap = new Point[9, 10];
        public static Rectangle BoardArea = new Rectangle(-1, -1, -1, -1);
        public static Rectangle BoardAreaRaw = new Rectangle(-1, -1, -1, -1);
        Bitmap LastBoardArea = null;
        Thread thCheckImage;
        Bitmap DetectDisplayBitmap;
        Graphics DetectGDI;
        Bitmap ResultDisplayBitmap = new Bitmap(400, 440);
        Graphics ResultDisplayGDI;
        EngineHelper Engine;
        bool Running = true;
        string LastFen = "";
        string ExpectedFen = "";
        public ChessMove BestMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
        public ChessMove PonderMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
        public static int EngineAnalyzeCount = 0;
        public ScreenShotMode ScreenshotMode;
        public ScreenshotForm SSForm = new ScreenshotForm();
        public static ProgramSettings Settings = new ProgramSettings();
        public static string SettingsPath = "./settings.json";
        public static Dictionary<string, Solution> SolutionList = new Dictionary<string, Solution>();
        public static Solution CurrentSolution;
        public static Size ClickOffset = new Size(0, 0);

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
        
        public enum ScreenShotMode
        {
            Handle,
            Window
        }

        public class ProgramSettings
        {
            public Dictionary<string, EngineSettings> EngineList = new Dictionary<string, EngineSettings>();
            public string SelectedEngine = "";
            public float ScaleFactor = 0.8f;
            public bool AutoGo = true;
            public double StepTime = 1.0;
            public bool RedSide = true;
            public int ThreadCount = 8;
            public bool KeepDetecting = false;
            public bool UniversalMode = false;
            public string SelectedSolution = "";
            public EngineSettings CurrentEngine { 
                get
                {
                    if (EngineList.ContainsKey(SelectedEngine))
                    {
                        return EngineList[SelectedEngine];
                    }
                    return null;
                } 
            }
        }


        public class EngineSettings
        {
            public string ExePath = "";
            public Dictionary<string, string> Configs = new Dictionary<string, string>();
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
            LoadSettings();
            InitSolutions();
            InitSettingsUI();
            thCheckImage = new Thread(new ThreadStart(CheckImageLoop));
            thCheckImage.Start();
            InitResultDisplay();
            InitEngine();
        }

        void LoadSettings()
        {
            if (File.Exists(SettingsPath))
            {
                Settings = JsonConvert.DeserializeObject<ProgramSettings>(File.ReadAllText(SettingsPath));
            }
        }

        void SaveSettings()
        {
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(Settings));
        }

        void InitSolutions()
        {
            if (!Directory.Exists("./Solutions"))
            {
                Directory.CreateDirectory("./Solutions");
            }
            string[] paramsKeys = "截图标题 截图类 点击标题 点击类".Split(' ');
            foreach (var file in Directory.GetFiles("./Solutions"))
            {
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
                    SolutionList.Add(file.Split('\\').Last().Split('.')[0], new Solution(paramList[0], paramList[1], paramList[2], paramList[3]));
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
            if (Settings.UniversalMode)
            {
                SSForm.Visible = true;
            }
            else
            {
                SSForm.Visible = false;
            }
        }

        void InitResultDisplay()
        {
            pictureBox_board.Image = ResultDisplayBitmap;
            ResultDisplayGDI = Graphics.FromImage(ResultDisplayBitmap);
            RenderResultBoard();
        }

        void InitEngine()
        {
            if (Engine != null && !Engine.Engine.HasExited)
            {
                Engine.Stop();
            }
            if (Settings.SelectedEngine.Length > 0 && Settings.EngineList.ContainsKey(Settings.SelectedEngine))
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

        void DisplayStatus(string status)
        {
            this.Invoke(new Action(() => label_status.Text = "识别状态: " + status));
        }

        private void Engine_InfoEvent(string cmd, Dictionary<string, string> infos)
        {
            this.Invoke(new Action(() =>
            {
                string info_str = "";
                if (infos.ContainsKey("score"))
                {
                    if (infos["score"] == "mate" && double.Parse(infos["depth"]) > 40)
                    {
                        return;
                    }
                    richTextBox_engine_log.AppendText(
                        $"深度: {infos["depth"]} " +
                        $"得分: {infos["score"]} " +
                        $"时间: {(infos.ContainsKey("time") ? Math.Round(double.Parse(infos["time"]) / 1000, 1) : 0)}秒 " +
                        $"nps: {(infos.ContainsKey("nps") ? Math.Round(double.Parse(infos["nps"]) / 1000) : 0)}K" +
                        "\n"
                        );
                    richTextBox_engine_log.ScrollToCaret();
                }
                if (infos.ContainsKey("pv"))
                {
                    string[] moves = infos["pv"].Split(' ');
                    string bestMove = moves[0];
                    Point fromPoint = Utils.Move2Point(bestMove.Substring(0, 2), Settings.RedSide);
                    Point toPoint = Utils.Move2Point(bestMove.Substring(2, 2), Settings.RedSide);
                    richTextBox_engine_log.AppendText(string.Join(" ", moves) + "\n");
                    BestMove = new ChessMove(fromPoint, toPoint);
                    if (moves.Length > 1)
                    {
                        string ponderMove = moves[1];
                        Point ponderFrom = Utils.Move2Point(ponderMove.Substring(0, 2), Settings.RedSide);
                        Point ponderTo = Utils.Move2Point(ponderMove.Substring(2, 2), Settings.RedSide);
                        PonderMove = new ChessMove(ponderFrom, ponderTo);
                    }
                    RenderResultBoard();
                }
            }));
        }

        private void Engine_BestMoveEvent(string bestMove, string ponderMove)
        {
            if (EngineAnalyzeCount > 1)
            {
                EngineAnalyzeCount--;
                return;
            }
            Point fromPoint = Utils.Move2Point(bestMove.Substring(0, 2), Settings.RedSide);
            Point toPoint = Utils.Move2Point(bestMove.Substring(2, 2), Settings.RedSide);
            Point fromClickPoint = PositionMap[fromPoint.X, fromPoint.Y];
            Point toClickPoint = PositionMap[toPoint.X, toPoint.Y];
            BestMove = new ChessMove(fromPoint, toPoint);
            if (ponderMove != "")
            {
                Point ponderFrom = Utils.Move2Point(ponderMove.Substring(0, 2), Settings.RedSide);
                Point ponderTo = Utils.Move2Point(ponderMove.Substring(2, 2), Settings.RedSide);
                PonderMove = new ChessMove(ponderFrom, ponderTo);
            }
            else
            {
                PonderMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
            }
            RenderResultBoard();
            Board[toPoint.X, toPoint.Y] = Board[fromPoint.X, fromPoint.Y];
            Board[fromPoint.X, fromPoint.Y] = null;
            ExpectedFen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b");
            if (Settings.AutoGo)
            {
                MouseLeftClick(fromClickPoint.X, fromClickPoint.Y);
                Thread.Sleep(300);
                MouseLeftClick(toClickPoint.X, toClickPoint.Y);
                Thread.Sleep(100);
            }
            EngineAnalyzeCount--;
        }

        Bitmap Screenshot()
        {
            if (Settings.UniversalMode && SSForm != null)
            {
                return SSForm.Screenshot();
            }
            else
            {
                return ScreenshotHelper.GetWindowCapture(GameHandle);
            }
        }

        void MouseLeftClick(int x, int y)
        {
            if (Settings.UniversalMode && SSForm != null)
            {
                MouseHelper.VirtualMouse.MoveTo(SSForm.Left + x, SSForm.Top + y);
                MouseHelper.VirtualMouse.LeftClick();
            }
            else
            {
                MouseHelper.MouseLeftClick(ClickHandle, x, y);
            }
        }

        void CheckImageLoop()
        {
            int gcCount = 0;
            bool ForceRefresh = false;
            bool HaveUpdated = false;
            string lastFen = "";
            string currentFen = "";
            int currentFenCount = 0;
            while (Running)
            {
                try
                {
                    gcCount = (gcCount + 1) % 20;
                    if (gcCount == 0) GC.Collect();
                    if (checkBox_debug.Checked)
                    {
                        pictureBox_show_result.Image = Screenshot();
                        pictureBox_show_result.Refresh();
                        Thread.Sleep(200);
                        continue;
                    }
                    if (GameHandle == IntPtr.Zero && !Settings.UniversalMode)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    Bitmap screenshot = Screenshot();
                    Size maxSize = screenshot.Size;
                    int sameCount = 0;
                    if (LastBoardArea != null && BoardArea.X != -1)
                    {
                        Bitmap currentBoard = (Bitmap)ImageHelper.CropImage(screenshot, BoardArea);
                        if (Settings.KeepDetecting)
                        {
                            DisplayStatus("YOLO检测图像");
                            bool result = ModelPredict(currentBoard);
                            DisplayStatus("YOLO检测图像完成");
                            if (result)
                            {
                                LastBoardArea = currentBoard;
                                string fen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b");
                                if (fen != lastFen)
                                {
                                    if (fen != currentFen)
                                    {
                                        currentFen = fen;
                                        currentFenCount = 1;
                                    }
                                    else
                                    {
                                        currentFenCount++;
                                        if (currentFenCount >= 2)
                                        {
                                            BoardReloaded();
                                            lastFen = fen;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                BoardArea = new Rectangle(-1, -1, -1, -1);
                                continue;
                            }
                            HaveUpdated = false;
                        }
                        else
                        {
                            if (!ImageHelper.AreEqual(currentBoard, LastBoardArea) || ForceRefresh)
                            {
                                sameCount = 0;
                                if (ForceRefresh) ForceRefresh = false;
                                HaveUpdated = true;
                                LastBoardArea = currentBoard;
                                this.Invoke(new Action(() =>
                                {
                                    DisplayStatus("检测到图像变动");
                                }));
                            }
                            else
                            {
                                if (sameCount >= 0 && HaveUpdated)
                                {
                                    DisplayStatus("YOLO检测图像");
                                    bool result = ModelPredict(currentBoard);
                                    DisplayStatus("YOLO检测图像完成");
                                    if (result)
                                    {
                                        LastBoardArea = currentBoard;
                                        BoardReloaded();
                                    }
                                    else
                                    {
                                        BoardArea = new Rectangle(-1, -1, -1, -1);
                                        continue;
                                    }
                                    HaveUpdated = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (LastBoardArea == null || !ImageHelper.AreEqual(screenshot, LastBoardArea))
                        {
                            DisplayStatus("重载图像范围");
                            bool result = ModelPredict(screenshot);
                            if (result)
                            {
                                BoardArea = Utils.ExpendArea(Utils.RestoreArea(BoardArea, BoardAreaRaw), maxSize);
                                LastBoardArea = (Bitmap)ImageHelper.CropImage(screenshot, BoardArea);
                                ForceRefresh = true;
                                DisplayStatus("更新范围成功");
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                if (Settings.KeepDetecting)
                {
                    Thread.Sleep(500);
                }
                else
                {
                    Thread.Sleep(300);
                }
            }
        }

        public void RenderResultBoard()
        {
            this.Invoke(new Action(() =>
            {
                int width = 40;
                int height = 40;
                int xoffset = width / 2;
                int yoffset = height / 2;
                ResultDisplayGDI.Clear(Color.White);
                ResultDisplayGDI.DrawImage(Properties.Resources.board, 0, 0, width * 10, height * 11);
                // draw chess
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        if (Board[x, y] != null)
                        {
                            ResultDisplayGDI.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject(Board[x, y]), x * width + xoffset, y * height + yoffset, width, height);
                        }
                    }
                }
                // draw hint
                Pen rp = new Pen(Color.FromArgb(180, Color.Brown), 8);
                Pen bp = new Pen(Color.FromArgb(180, Color.Gray), 8);
                rp.EndCap = LineCap.ArrowAnchor;
                bp.EndCap = LineCap.ArrowAnchor;
                if (BestMove.From.X != -1)
                {
                    Point bestFrom = new Point(BestMove.From.X * width + xoffset + width / 2, BestMove.From.Y * height + yoffset + height / 2);
                    Point bestTo = new Point(BestMove.To.X * width + xoffset + width / 2, BestMove.To.Y * height + yoffset + height / 2);
                    if (radioButton_side_red.Checked)
                    {
                        ResultDisplayGDI.DrawLine(rp, bestFrom, bestTo);
                    }
                    else
                    {
                        ResultDisplayGDI.DrawLine(bp, bestFrom, bestTo);
                    }
                }
                if (PonderMove.From.X != -1)
                {
                    Point ponderFrom = new Point(PonderMove.From.X * width + xoffset + width / 2, PonderMove.From.Y * height + yoffset + height / 2);
                    Point ponderTo = new Point(PonderMove.To.X * width + xoffset + width / 2, PonderMove.To.Y * height + yoffset + height / 2);
                    if (radioButton_side_red.Checked)
                    {
                        ResultDisplayGDI.DrawLine(bp, ponderFrom, ponderTo);
                    }
                    else
                    {
                        ResultDisplayGDI.DrawLine(rp, ponderFrom, ponderTo);
                    }
                }
                pictureBox_board.Invoke(new Action(() => pictureBox_board.Refresh()));
            }));
        }

        bool DetectBoard(List<YoloPrediction> predictions)
        {
            RectangleF board = new Rectangle(-1, -1, -1, -1);
            int totalChessmanCount = 0;
            double totalChessmanWidth = 0, totalChessmanHeight = 0;
            foreach (var prediction in predictions) // iterate predictions to draw results
            {
                if (prediction.Label.Name == "board")
                {
                    board = prediction.Rectangle;
                }
                else
                {
                    totalChessmanCount++;
                    totalChessmanWidth += prediction.Rectangle.Width;
                    totalChessmanHeight += prediction.Rectangle.Height;
                }
            }
            double avgChessmanWidth = totalChessmanWidth / totalChessmanCount;
            double avgChessmanHeight = totalChessmanHeight / totalChessmanCount;
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    Board[x, y] = null;
                }
            }
            if (board.X == -1) return false;
            if (board.Width / board.Height > 1.3 || board.Width / board.Height < 0.7) return false;
            if (board.Width < avgChessmanWidth * 8 || board.Height < avgChessmanHeight * 9) return false;
            BoardAreaRaw = new Rectangle((int)board.X, (int)board.Y, (int)board.Width, (int)board.Height);
            float gridWidth = board.Width / 8f;
            float gridHeight = board.Height / 9f;
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    double pos_x = board.X + x * gridWidth;
                    double pos_y = board.Y + y * gridHeight;
                    PositionMap[x, y] = new Point((int)pos_x + BoardArea.X, (int)pos_y + BoardArea.Y);
                }
            }
            foreach (var prediction in predictions)
            {
                if (prediction.Label.Name == "board") continue;
                float centerX = prediction.Rectangle.X + prediction.Rectangle.Width / 2;
                float centerY = prediction.Rectangle.Y + prediction.Rectangle.Height / 2;
                float offsetX = centerX - board.X + gridWidth / 2; // offset from the board
                float offsetY = centerY - board.Y + gridHeight / 2;
                int xPos = (int)Math.Floor(offsetX / gridWidth);
                int yPos = (int)Math.Floor(offsetY / gridHeight);
                if (xPos >= 0 && xPos <= 8 && yPos >= 0 && yPos <= 9)
                {
                    Board[xPos, yPos] = prediction.Label.Name;
                }
                if (prediction.Label.Name == "r_jiang")
                {
                    if (yPos < 5)
                    {
                        Settings.RedSide = false;
                    }
                    else
                    {
                        Settings.RedSide = true;
                    }
                    this.Invoke(new Action(() =>
                    {
                        radioButton_side_red.Checked = Settings.RedSide;
                        radioButton_side_black.Checked = !Settings.RedSide;
                    }));
                }
            }
            return true;
        }

        bool ModelPredict(Bitmap image)
        {
            DetectDisplayBitmap = new Bitmap(image.Width, image.Height);
            DetectGDI = Graphics.FromImage(DetectDisplayBitmap);
            DetectGDI.SmoothingMode = SmoothingMode.AntiAlias; //使绘图质量最高，即消除锯齿
            DetectGDI.InterpolationMode = InterpolationMode.HighQualityBicubic;
            DetectGDI.CompositingQuality = CompositingQuality.HighQuality;
            DetectGDI.DrawImage(image, 0, 0);
            DateTime st = DateTime.Now;
            List<YoloPrediction> predictions = Model.Predict(image);
            Debug.WriteLine("模型耗时: " + Math.Round((DateTime.Now - st).TotalSeconds, 2).ToString() + "s");
            foreach (var prediction in predictions) // iterate predictions to draw results
            {
                double score = Math.Round(prediction.Score, 2);
                Color c = Color.Red;
                DetectGDI.DrawRectangles(new Pen(c, 1),
                    new[] { prediction.Rectangle });

                var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);
                float centerX = prediction.Rectangle.X + prediction.Rectangle.Width / 2;
                float centerY = prediction.Rectangle.Y + prediction.Rectangle.Height / 2;
                DetectGDI.FillEllipse(Brushes.Lime, new RectangleF(centerX - 2, centerY - 2, 4, 4));
                DetectGDI.DrawString($"{prediction.Label.Name}",
                    new Font("Arial", 16, GraphicsUnit.Pixel), new SolidBrush(c),
                    new PointF(x, y));
            }
            pictureBox_show_result.Image = DetectDisplayBitmap;
            return DetectBoard(predictions);
        }
        
        void BoardReloaded()
        {
            RenderResultBoard();
            this.Invoke(new Action(() =>
            {
                string fen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b");
                if (fen == null) return;
                if (fen != LastFen && fen != ExpectedFen)
                {
                    LastFen = fen;
                    DisplayStatus("开始分析");
                    EngineAnalyzeCount++;
                    Engine.StartAnalyze(fen, Settings.StepTime);
                }
            }));
        }
        
        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            Running = false;
            Engine.Stop();
        }

        private void timer_set_window_hwnd_Tick(object sender, EventArgs e)
        {
            if (MouseHelper.GetAsyncKeyState(Keys.F2))
            {
                GameHandle = (IntPtr)ScreenshotHelper.WindowFromPoint(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void pictureBox_board_MouseDown(object sender, MouseEventArgs e)
        {
            bool MouseOnImage = false;
            Point MousePositionOnImage = new Point(-1, -1);
            double img_ratio = (double)ResultDisplayBitmap.Width / (double)ResultDisplayBitmap.Height;
            double box_ratio = (double)pictureBox_board.Width / (double)pictureBox_board.Height;
            double display_ratio;
            int display_x, display_y, display_width, display_height;
            if (box_ratio >= img_ratio)
            {
                display_y = 0;
                display_height = pictureBox_board.Height;
                display_x = (int)(pictureBox_board.Width - display_height * img_ratio) / 2;
                display_width = (int)(display_height * img_ratio);
                display_ratio = (double)ResultDisplayBitmap.Height / display_height;
            }
            else
            {
                display_x = 0;
                display_width = pictureBox_board.Width;
                display_y = (int)(pictureBox_board.Height - display_width / img_ratio) / 2;
                display_height = (int)(display_width / img_ratio);
                display_ratio = (double)ResultDisplayBitmap.Width / display_width;
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
                img_x = ResultDisplayBitmap.Width - 1;
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
                img_y = ResultDisplayBitmap.Height - 1;
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
                    Point p = PositionMap[pos.X, pos.Y];
                    MouseLeftClick(p.X, p.Y);
                }
            }
        }

        private void pictureBox_board_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void button_redetect_Click(object sender, EventArgs e)
        {
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
            EngineAnalyzeCount = 0;
            LastFen = "";
            ExpectedFen = "";
            //Task.Run(new Action(() =>
            //{
            //    Bitmap screenshot = Screenshot(GameHandle);
            //    Size maxSize = screenshot.Size;
            //    try
            //    { }
            //    if (LastBoardArea == null || !ImageHelper.AreEqual(screenshot, LastBoardArea))
            //    {
            //        bool result = ModelPredict(screenshot);
            //        if (result)
            //        {
            //            BoardArea = Utils.ExpendArea(Utils.RestoreArea(BoardArea, BoardAreaRaw), maxSize);
            //            LastBoardArea = (Bitmap)ImageHelper.CropImage(screenshot, BoardArea);
            //            BoardReloaded();
            //        }
            //        LastBoardArea = screenshot;
            //    }
            //}));
        }

        private void numericUpDown_thread_count_ValueChanged(object sender, EventArgs e)
        {
            string threadStr = numericUpDown_thread_count.Value.ToString();
            if (Engine != null)
            {
                Engine.SetOption("Threads", threadStr);
            }
            Settings.ThreadCount = (int)numericUpDown_thread_count.Value;
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

        private void button_load_engine_Click(object sender, EventArgs e)
        {

        }

        private void comboBox_engine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_engine.SelectedItem.ToString() != Settings.SelectedEngine)
            {
                if (Engine != null)
                {
                    Engine.Stop();
                }
                Settings.SelectedEngine = comboBox_engine.SelectedItem.ToString();
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
            Task.Run(new Action(() =>
            {
                DisplayStatus("准备获取游戏窗口句柄...2");
                Thread.Sleep(1000);
                DisplayStatus("准备获取游戏窗口句柄...1");
                Thread.Sleep(1000);
                GameHandle = (IntPtr)ScreenshotHelper.WindowFromPoint(Cursor.Position.X, Cursor.Position.Y);
                DisplayStatus("游戏窗口句柄: " + GameHandle);
            }));
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
            if (Settings.UniversalMode)
            {
                SSForm.Visible = true;
            }
            else
            {
                SSForm.Visible = false;
            }
        }

        private void checkBox_auto_go_CheckedChanged(object sender, EventArgs e)
        {
            Settings.AutoGo = checkBox_auto_go.Checked;
            SaveSettings();
        }

        private void comboBox_solution_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                    ClickOffset = new Size(clickRect.Width - gameRect.Width, clickRect.Height - gameRect.Height);
                }
                Settings.SelectedSolution = key;
                SaveSettings();
            }
        }

        private void ToolStripMenuItem_copy_fen_Click(object sender, EventArgs e)
        {
            string fen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b");
            Clipboard.SetText(fen);
            MessageBox.Show("局面: \n" + fen + "\n 已经复制到剪贴板");
        }
    }
}
