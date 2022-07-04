using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Yolov5Net.Scorer;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace VinXiangQi
{
    partial class Mainform
    {
        // 图片上方形的棋盘区域 x, y, width, height
        Rectangle BoardArea = new Rectangle(-1, -1, -1, -1);
        // Yolo模型识别出的棋盘区域 x, y, width, height，因为可能送进模型的已经是裁切过的棋盘区域，所以该数值需要被还原为上方的BoardArea
        Rectangle BoardAreaRaw = new Rectangle(-1, -1, -1, -1);
        // 上一次截图时，棋盘显示区域的图片，用于对比图片来判断是否处在动画中
        Bitmap LastBoardAreaBitmap = null;
        // 当失败次数过多时，自动重载
        int InvalidCount = 0;
        // 是否第一次收到绝杀
        bool FirstMate = true;

        void AutoClickLoop()
        {
            while (Running)
            {
                try
                {
                    if (!Settings.AutoClick)
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                    Bitmap screen = Screenshot();
                    foreach (Bitmap image in AutoClickImageList.ToArray())
                    {
                        Point pos = ImageHelper.FindImageFromTop(screen, image);
                        if (pos.X != -1)
                        {
                            Point clickPos = new Point(pos.X + image.Width / 2, pos.Y + image.Height / 2);
                            MouseLeftClick(clickPos.X, clickPos.Y);
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    File.AppendAllText("err.log", DateTime.Now.ToString() + "\r\n" + ex.ToString() + "\r\n\r\n");
                }
                Thread.Sleep(5000);
            }
        }

        void DetectionLoop()
        {
            int gcCount = 0;
            bool ForceRefresh = false;
            bool HaveUpdated = false;
            string[,] CurrentBoard = null;
            int currentFenCount = 0;
            while (Running)
            {
                try
                {
                    gcCount = (gcCount + 1) % 20;
                    if (gcCount == 0) GC.Collect();
                    if (checkBox_debug.Checked)
                    {
                        this.Invoke(new Action(() =>
                        {
                            pictureBox_show_result.Image = Screenshot();
                            pictureBox_show_result.Refresh();
                        }));
                        Thread.Sleep(200);
                        continue;
                    }
                    if (GameHandle == IntPtr.Zero || !DetectEnabled)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    Bitmap screenshot = Screenshot();
                    Size maxSize = screenshot.Size;
                    bool reloaded = false;
                    if (LastBoardAreaBitmap != null && BoardArea.X != -1)
                    {
                        Bitmap currentBoard = (Bitmap)ImageHelper.CropImage(screenshot, BoardArea);
                        if (Settings.KeepDetecting)
                        {
                            DisplayStatus("检测图像");
                            bool result = ModelPredict(currentBoard);
                            if (result)
                            {
                                LastBoardAreaBitmap = currentBoard;
                                var compareResult = Utils.CompareBoard(LastBoard, Board);
                                if (compareResult.DiffCount > 0)
                                {
                                    DisplayStatus("棋盘发生变化");
                                    if (EngineAnalyzeCount > 0)
                                    {
                                        Engine.StopAnalyze();
                                        EngineSkipCount++;
                                    }
                                    var compare2 = Utils.CompareBoard(CurrentBoard, Board);
                                    if (compare2.DiffCount > 0)
                                    {
                                        CurrentBoard = (string[,])Board.Clone();
                                        currentFenCount = 1;
                                        reloaded = false;
                                    }
                                    else
                                    {
                                        currentFenCount++;
                                        if (currentFenCount >= 2 && !reloaded)
                                        {
                                            ReloadBoard();
                                            reloaded = true;
                                        }
                                    }
                                }
                                else
                                {
                                    DisplayStatus("棋盘未改变");
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
                            if (!ImageHelper.AreEqual(currentBoard, LastBoardAreaBitmap) || ForceRefresh)
                            {
                                if (ForceRefresh) ForceRefresh = false;
                                HaveUpdated = true;
                                LastBoardAreaBitmap = currentBoard;
                                this.Invoke(new Action(() =>
                                {
                                    DisplayStatus("检测到图像变动");
                                }));
                            }
                            else
                            {
                                if (HaveUpdated)
                                {
                                    DisplayStatus("YOLO检测图像");
                                    bool result = ModelPredict(currentBoard);
                                    if (result)
                                    {
                                        LastBoardAreaBitmap = currentBoard;
                                        ReloadBoard();
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
                        if (LastBoardAreaBitmap == null || !ImageHelper.AreEqual(screenshot, LastBoardAreaBitmap))
                        {
                            DisplayStatus("识别不到棋盘，重置图像范围");
                            bool result = ModelPredict(screenshot);
                            if (result)
                            {
                                BoardArea = Utils.ExpendArea(Utils.RestoreArea(BoardArea, BoardAreaRaw), maxSize);
                                LastBoardAreaBitmap = (Bitmap)ImageHelper.CropImage(screenshot, BoardArea);
                                ForceRefresh = true;
                                DisplayStatus("更新范围成功");
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    File.AppendAllText("err.log", DateTime.Now.ToString() + "\r\n" + ex.ToString() + "\r\n\r\n");
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

        public void RenderDisplayBoard()
        {
            this.Invoke(new Action(() =>
            {
                int width = 40;
                int height = 40;
                int xoffset = width / 2;
                int yoffset = height / 2;
                BoardGDI.Clear(Color.White);
                BoardGDI.DrawImage(Properties.Resources.board, 0, 0, width * 10, height * 11);
                // draw chess
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        if (Board[x, y] != null)
                        {
                            BoardGDI.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject(Board[x, y]), x * width + xoffset, y * height + yoffset, width, height);
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
                        BoardGDI.DrawLine(rp, bestFrom, bestTo);
                    }
                    else
                    {
                        BoardGDI.DrawLine(bp, bestFrom, bestTo);
                    }
                }
                if (PonderMove.From.X != -1)
                {
                    Point ponderFrom = new Point(PonderMove.From.X * width + xoffset + width / 2, PonderMove.From.Y * height + yoffset + height / 2);
                    Point ponderTo = new Point(PonderMove.To.X * width + xoffset + width / 2, PonderMove.To.Y * height + yoffset + height / 2);
                    if (radioButton_side_red.Checked)
                    {
                        BoardGDI.DrawLine(bp, ponderFrom, ponderTo);
                    }
                    else
                    {
                        BoardGDI.DrawLine(rp, ponderFrom, ponderTo);
                    }
                }
                pictureBox_board.Refresh();
            }));
        }

        bool GetBoardFromPrediction(List<YoloPrediction> predictions)
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
            string[,] tmpBoard = new string[9, 10];
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    tmpBoard[x, y] = null;
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
                    ClickPositionMap[x, y] = new Point((int)pos_x + BoardArea.X, (int)pos_y + BoardArea.Y);
                }
            }
            foreach (var prediction in predictions)
            {
                if (prediction.Label.Name == "board") continue;
                float centerX = prediction.Rectangle.X + prediction.Rectangle.Width / 2;
                float centerY = prediction.Rectangle.Y + prediction.Rectangle.Height / 2;
                float offsetX = centerX - board.X; // offset from the board
                float offsetY = centerY - board.Y;
                int xPos = (int)Math.Round(offsetX / gridWidth);
                int yPos = (int)Math.Round(offsetY / gridHeight);
                if (xPos >= 0 && xPos <= 8 && yPos >= 0 && yPos <= 9)
                {
                    tmpBoard[xPos, yPos] = prediction.Label.Name;
                }
                else if (xPos == -1 || xPos == 9 || yPos == -1 || yPos == 10)
                {
                    Debug.WriteLine(prediction.Label.Name + " 位置检测错误");
                    return false;
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
            Board = tmpBoard;
            return true;
        }

        bool ModelPredict(Bitmap image)
        {
            YoloDisplayBitmap = new Bitmap(image.Width, image.Height);
            YoloGDI = Graphics.FromImage(YoloDisplayBitmap);
            YoloGDI.DrawImage(image, 0, 0);
            DateTime st = DateTime.Now;
            List<YoloPrediction> predictions = Model.Predict(image);
            //Debug.WriteLine("模型耗时: " + Math.Round((DateTime.Now - st).TotalSeconds, 2).ToString() + "s");
            foreach (var prediction in predictions) // iterate predictions to draw results
            {
                double score = Math.Round(prediction.Score, 2);
                Color c = Color.Red;
                YoloGDI.DrawRectangles(new Pen(c, 1),
                    new[] { prediction.Rectangle });

                var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);
                float centerX = prediction.Rectangle.X + prediction.Rectangle.Width / 2;
                float centerY = prediction.Rectangle.Y + prediction.Rectangle.Height / 2;
                YoloGDI.FillEllipse(Brushes.Lime, new RectangleF(centerX - 2, centerY - 2, 4, 4));
                YoloGDI.DrawString($"{prediction.Label.Name}",
                    new Font("Arial", 16, GraphicsUnit.Pixel), new SolidBrush(c),
                    new PointF(x, y));
            }
            //RectangleF board = new Rectangle(-1, -1, -1, -1);
            //int totalChessmanCount = 0;
            //double totalChessmanWidth = 0, totalChessmanHeight = 0;
            //foreach (var prediction in predictions) // iterate predictions to draw results
            //{
            //    if (prediction.Label.Name == "board")
            //    {
            //        board = prediction.Rectangle;
            //    }
            //    else
            //    {
            //        totalChessmanCount++;
            //        totalChessmanWidth += prediction.Rectangle.Width;
            //        totalChessmanHeight += prediction.Rectangle.Height;
            //    }
            //}
            //float gridWidth = board.Width / 8f;
            //float gridHeight = board.Height / 9f;
            //foreach (var prediction in predictions)
            //{
            //    if (prediction.Label.Name == "board") continue;
            //    float centerX = prediction.Rectangle.X + prediction.Rectangle.Width / 2;
            //    float centerY = prediction.Rectangle.Y + prediction.Rectangle.Height / 2;
            //    float offsetX = centerX - board.X; // offset from the board
            //    float offsetY = centerY - board.Y;
            //    int xPos = (int)Math.Round(offsetX / gridWidth);
            //    int yPos = (int)Math.Round(offsetY / gridHeight);
            //    YoloGDI.DrawString("(" + xPos + "," + yPos + ")", new Font("Arial", 20, GraphicsUnit.Pixel), Brushes.Green, prediction.Rectangle.X, prediction.Rectangle.Y);
            //}
            pictureBox_show_result.Image = YoloDisplayBitmap;
            return GetBoardFromPrediction(predictions);
        }

        void ReloadBoard()
        {
            var compResult = Utils.CompareBoard(LastBoard, Board);
            if (compResult.DiffCount == 0) return;
            string opponentSymbol = Settings.RedSide ? "b_" : "r_";
            string mySymbol = Settings.RedSide ? "r_" : "b_";
            Debug.WriteLine($"Diff Result: {compResult.DiffCount} R: {compResult.RedDiff} B: {compResult.BlackDiff}");
            if (compResult.BlackDiff + compResult.RedDiff >= compResult.DiffCount && compResult.DiffCount < 10)
            {
                DisplayStatus("变化数小于消失数，判断为动画中，跳过");
                InvalidCount++;
                if (InvalidCount > 8)
                {
                    DisplayStatus("错误次数过多，自动重置");
                    InvalidCount = 0;
                    button_redetect_Click(null, null);
                }
                return;
            }
            if (compResult.DiffCount == 2 && compResult.Chess.Contains(mySymbol))
            {
                RenderDisplayBoard();
                if (Settings.AnalyzingMode)
                {
                    InvalidCount = 0;
                    LastBoard = (string[,])Board.Clone();
                    EngineAnalyzeCount++;
                    DisplayStatus("开始引擎计算");
                    string oppofen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b", Settings.RedSide ? "b" : "w");
                    Engine.StartAnalyze(oppofen, Settings.StepTime);
                }
                else
                {
                    InvalidCount = 0;
                    LastBoard = (string[,])Board.Clone();
                    DisplayStatus("己方棋子变化，跳过分析");
                }
                return;
            }
            var compResultExpected = Utils.CompareBoard(ExpectedBoard, Board);
            if (compResultExpected.DiffCount == 0)
            {
                RenderDisplayBoard();
                DisplayStatus("和预期棋盘一样，跳过");
                return;
            }
            if ((compResult.BlackDiff > 1 || compResult.RedDiff > 1) && compResult.DiffCount < 10)
            {
                DisplayStatus("差别过大，可能有动画，跳过");
                InvalidCount++;
                if (InvalidCount > 8)
                {
                    DisplayStatus("错误次数过多，自动重置");
                    InvalidCount = 0;
                    button_redetect_Click(null, null);
                }
                return;
            }
            RenderDisplayBoard();
            InvalidCount = 0;
            EngineAnalyzeCount++;
            LastBoard = (string[,])Board.Clone();
            DisplayStatus("开始引擎计算");
            string fen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b", Settings.RedSide ? "w" : "b");
            Engine.StartAnalyze(fen, Settings.StepTime);
        }

        private void Engine_InfoEvent(string cmd, Dictionary<string, string> infos)
        {

            string info_str = "";
            if (infos.ContainsKey("score"))
            {
                if (Settings.StopWhenMate)
                {
                    if (infos["score"].Contains("绝杀") && int.Parse(infos["score"].Split('(').Last().Split(')').First()) > 0)
                    {
                        if (FirstMate)
                        {
                            FirstMate = false;
                            Engine.StopAnalyze();
                            return;
                        }                            
                    }
                    else
                    {
                        FirstMate = true;
                    }
                }

                if (infos["score"].Contains("绝杀") && (double.Parse(infos["depth"]) < 40 || double.Parse(infos["depth"]) > 50))
                {
                    return;
                }
                info_str += (
                    $"深度: {infos["depth"]} " +
                    $"得分: {infos["score"]} " +
                    $"时间: {(infos.ContainsKey("time") ? Math.Round(double.Parse(infos["time"]) / 1000, 1) : 0)}秒 " +
                    $"nps: {(infos.ContainsKey("nps") ? Math.Round(double.Parse(infos["nps"]) / 1000) : 0)}K" +
                    "\r\n"
                    );
            }
            if (infos.ContainsKey("pv"))
            {
                string[,] board = (string[,])Board.Clone();
                string fen = Utils.BoardToFen(board, Settings.RedSide ? "w" : "b", Settings.RedSide ? "w" : "b");
                string[] moves = infos["pv"].Split(' ');
                string bestMove = moves[0];
                Point fromPoint = Utils.Move2Point(bestMove.Substring(0, 2), Settings.RedSide);
                Point toPoint = Utils.Move2Point(bestMove.Substring(2, 2), Settings.RedSide);
                try
                {
                    //Debug.WriteLine(fen);
                    //Debug.WriteLine(string.Join(" ", moves));
                    //Debug.WriteLine(Utils.FenToChina_S(fen, string.Join(" ", moves), 1));
                    //Debug.WriteLine("------");
                    //info_str += Utils.FenToChina_S(fen, string.Join(" ", moves), 1) + "\r\n";
                    info_str += string.Join(" ", moves) + "\r\n";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                BestMove = new ChessMove(fromPoint, toPoint);
                if (moves.Length > 1)
                {
                    string ponderMove = moves[1];
                    Point ponderFrom = Utils.Move2Point(ponderMove.Substring(0, 2), Settings.RedSide);
                    Point ponderTo = Utils.Move2Point(ponderMove.Substring(2, 2), Settings.RedSide);
                    PonderMove = new ChessMove(ponderFrom, ponderTo);
                }
                RenderDisplayBoard();
            }
            if (info_str != "")
            {
                this.Invoke(new Action(() =>
                {
                    textBox_engine_log.AppendText(info_str);
                }));
            }
        }

        private void Engine_BestMoveEvent(string bestMove, string ponderMove)
        {
            EngineAnalyzeCount--;
            if (bestMove.Length != 4) return;
            Point fromPoint = Utils.Move2Point(bestMove.Substring(0, 2), Settings.RedSide);
            Point toPoint = Utils.Move2Point(bestMove.Substring(2, 2), Settings.RedSide);
            Point fromClickPoint = ClickPositionMap[fromPoint.X, fromPoint.Y];
            Point toClickPoint = ClickPositionMap[toPoint.X, toPoint.Y];
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
            RenderDisplayBoard();
            ExpectedBoard = (string[,])Board.Clone();
            ExpectedBoard[toPoint.X, toPoint.Y] = ExpectedBoard[fromPoint.X, fromPoint.Y];
            ExpectedBoard[fromPoint.X, fromPoint.Y] = null;
            if (Settings.AutoGo)
            {
                MouseLeftClient_2Point(fromClickPoint.X, fromClickPoint.Y, toClickPoint.X, toClickPoint.Y);
            }
        }
    }
}
