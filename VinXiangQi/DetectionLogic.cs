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
        // 图片上方形的棋盘区域 x, y, width, height 包括棋盘的框框 一个扩展的范围
        Rectangle BoardArea = new Rectangle(-1, -1, -1, -1);
        // Yolo模型识别出的棋盘区域 x, y, width, height，因为可能送进模型的已经是裁切过的棋盘区域，所以该数值需要被还原为上方的BoardArea
        Rectangle BoardAreaRaw = new Rectangle(-1, -1, -1, -1);
        // 上一次截图时，棋盘显示区域的图片，用于对比图片来判断是否处在动画中
        Bitmap LastBoardAreaBitmap = null;
        // 当失败次数过多时，自动重载
        int InvalidCount = 0;
        // 是否第一次收到绝杀
        bool FirstMate = true;
        Color NormalColor = Color.Black;
        Color PositiveColor = Color.DarkBlue;
        Color NegativeColor = Color.DarkRed;
        Color MateColor = Color.Green;

        public string ExpectedMove = "";
        public bool ChangeDetectedAfterClick = false;
        public bool StartFromOpponent = false;
        public bool BackgroundAnalyzing = false;

        //Dictionary<string, Bitmap> TemplateList = new Dictionary<string, Bitmap>();
        //public bool TemplateInitiated = false;

        public enum ResultSource
        {
            Openbook,
            ChessDB,
            Engine
        }

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
                Thread.Sleep(2000);
            }
        }

        void DetectionLoop()
        {
            int gcCount = 0;
            bool ForceRefresh = false;
            bool HaveUpdated = false;
            string[,] CurrentBoard = null;
            int currentFenCount = 0;
            int autoGoFailCount = 0;
            while (Running)
            {
                DateTime startTime = DateTime.Now;
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
                        if (Settings.KeepDetecting || true) // 强制使用持续检测
                        {
                            DisplayStatus("检测图像");
                            bool result = ModelDetectBoard(currentBoard, false);
                            if (result)
                            {
                                LastBoardAreaBitmap = currentBoard;
                                var compareResult = Utils.CompareBoard(LastBoard, Board);
                                if (compareResult.DiffCount > 0)
                                {
                                    if (ChangeDetectedAfterClick == false)
                                    {
                                        BestMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
                                        ChangeDetectedAfterClick = true;
                                    }
                                    var compare2 = Utils.CompareBoard(CurrentBoard, Board);
                                    if (compare2.DiffCount > 0)
                                    {
                                        DisplayStatus("棋盘发生变化 等待确认 1");
                                        CurrentBoard = (string[,])Board.Clone();
                                        currentFenCount = 1;
                                        reloaded = false;
                                    }
                                    else
                                    {
                                        currentFenCount++;
                                        if (currentFenCount >= 3 && !reloaded)
                                        {
                                            DisplayStatus("棋盘发生变化 确认成功 重载棋盘");
                                            ReloadBoard();
                                            currentFenCount = 0;
                                            reloaded = true;
                                            autoGoFailCount = 0;
                                        }
                                        else
                                        {
                                            DisplayStatus("棋盘发生变化 等待确认 " + currentFenCount);
                                        }
                                    }
                                }
                                else
                                {
                                    DisplayStatus("棋盘未改变");
                                    if (Settings.AutoGo && !ChangeDetectedAfterClick)
                                    {
                                        autoGoFailCount++;
                                        if (autoGoFailCount >= 2)
                                        {
                                            PlayChess(ExpectedMove);
                                            DisplayStatus("检测到可能的落子失败，重试");
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
                        else // 动画结束后检测
                        {
                            //if (!ImageHelper.AreEqual(currentBoard, LastBoardAreaBitmap) || ForceRefresh)
                            //{
                            //    if (ForceRefresh) ForceRefresh = false;
                            //    HaveUpdated = true;
                            //    LastBoardAreaBitmap = currentBoard;
                            //    this.Invoke(new Action(() =>
                            //    {
                            //        DisplayStatus("检测到图像变动");
                            //    }));
                            //}
                            //else
                            //{
                            //    if (HaveUpdated)
                            //    {
                            //        DisplayStatus("YOLO检测图像");
                            //        bool result = ModelDetectBoard(currentBoard);
                            //        if (result)
                            //        {
                            //            LastBoardAreaBitmap = currentBoard;
                            //            ReloadBoard();
                            //        }
                            //        else
                            //        {
                            //            BoardArea = new Rectangle(-1, -1, -1, -1);
                            //            continue;
                            //        }
                            //        HaveUpdated = false;
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        DisplayStatus("识别不到棋盘，重置图像范围");
                        bool result = ModelDetectBoard(screenshot, true);
                        if (result)
                        {
                            BoardArea = Utils.ExpendArea(Utils.RestoreArea(BoardArea, BoardAreaRaw), maxSize);
                            LastBoardAreaBitmap = (Bitmap)ImageHelper.CropImage(screenshot, BoardArea);
                            ForceRefresh = true;
                            DisplayStatus("更新范围成功");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    File.AppendAllText("err.log", DateTime.Now.ToString() + "\r\n" + ex.ToString() + "\r\n\r\n");
                }
                DateTime endTime = DateTime.Now;
                double miliSec = (endTime - startTime).TotalMilliseconds;
                Debug.WriteLine($"总耗时: {miliSec}ms");
                if (miliSec < 500)
                {
                    Thread.Sleep((int)(500 - miliSec));
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
                        if (Board[x, y] != null && Board[x, y] != "")
                        {
                            string name = Board[x, y];
                            BoardGDI.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject(Board[x, y]), x * width + xoffset, y * height + yoffset, width, height);
                        }
                    }
                }
                // draw hint
                Pen rp = new Pen(Color.FromArgb(180, Color.OrangeRed), 10);
                Pen bp = new Pen(Color.FromArgb(180, Color.Blue), 10);
                Pen rp_dim = new Pen(Color.FromArgb(180, Color.DarkOrange), 8);
                Pen bp_dim = new Pen(Color.FromArgb(180, Color.DarkGreen), 8);
                rp.EndCap = LineCap.ArrowAnchor;
                bp.EndCap = LineCap.ArrowAnchor;
                rp_dim.EndCap = LineCap.ArrowAnchor;
                bp_dim.EndCap = LineCap.ArrowAnchor;
                if (BestMove.From.X != -1)
                {
                    Point bestFrom = new Point(BestMove.From.X * width + xoffset + width / 2, BestMove.From.Y * height + yoffset + height / 2);
                    Point bestTo = new Point(BestMove.To.X * width + xoffset + width / 2, BestMove.To.Y * height + yoffset + height / 2);
                    if (Settings.RedSide)
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
                    if (Settings.RedSide)
                    {
                        BoardGDI.DrawLine(bp, ponderFrom, ponderTo);
                    }
                    else
                    {
                        BoardGDI.DrawLine(rp, ponderFrom, ponderTo);
                    }
                }
                if (BackgroundAnalysisMove.From.X != -1)
                {
                    Point bgFrom = new Point(BackgroundAnalysisMove.From.X * width + xoffset + width / 2, BackgroundAnalysisMove.From.Y * height + yoffset + height / 2);
                    Point bgTo = new Point(BackgroundAnalysisMove.To.X * width + xoffset + width / 2, BackgroundAnalysisMove.To.Y * height + yoffset + height / 2);
                    if (Settings.RedSide)
                    {
                        BoardGDI.DrawLine(rp_dim, bgFrom, bgTo);
                    }
                    else
                    {
                        BoardGDI.DrawLine(bp_dim, bgFrom, bgTo);
                    }
                }
                pictureBox_board.Refresh();
            }));
        }

        //void InitTemplates(Bitmap image, List<YoloPrediction> predictions)
        //{
        //    TemplateList.Clear();
        //    foreach (var pred in predictions)
        //    {
        //        if (pred.Label.Name != "board" && !TemplateList.ContainsKey(pred.Label.Name))
        //        {
        //            RectangleF rect = pred.Rectangle;
        //            RectangleF smallRect = new RectangleF((float)(rect.X + rect.Width * 0.2), (float)(rect.Y + rect.Height * 0.2), (float)(rect.Width * 0.6), (float)(rect.Height * 0.6));
        //            Bitmap templateImage = image.Clone(smallRect, image.PixelFormat);
        //            TemplateList.Add(pred.Label.Name, templateImage);
        //            templateImage.Save("./templates/" + pred.Label.Name + ".png");
        //        }
        //    }
        //    image.Save("./templates/image.png");
        //    TemplateInitiated = true;
        //}

        bool ModelDetectBoard(Bitmap image, bool refreshBoard)
        {
            var predictions = ModelPredict(image);
            return GetBoardFromPrediction(predictions, refreshBoard);
        }

        //bool DetectBoard(Bitmap image)
        //{
        //    var predictions = ModelPredict(image);
        //    if (!TemplateInitiated)
        //    {
        //        InitTemplates(image, predictions);
        //    }
        //    return GetBoardFromPrediction(predictions);
        //}

        //bool TemplateDetectBoard(Bitmap image)
        //{
        //    if (!TemplateInitiated)
        //    {
        //        return DetectBoard(image);
        //    }
        //    DateTime startTime = DateTime.Now;
        //    Bitmap TemplateDisplayBitmap = new Bitmap(image.Width, image.Height);
        //    Graphics TemplateGDI = Graphics.FromImage(TemplateDisplayBitmap);
        //    TemplateGDI.DrawImage(image, 0, 0);
        //    TemplateGDI.DrawRectangle(Pens.Red, BoardAreaRaw);
        //    int gridX = BoardAreaRaw.Width / 8;
        //    int gridY = BoardAreaRaw.Height / 9;
        //    int offsetX = BoardAreaRaw.X - gridX / 2;
        //    int offsetY = BoardAreaRaw.Y - gridY / 2;
        //    for (int y = 0; y < 10; y++)
        //    {
        //        for (int x = 0; x < 9; x++)
        //        {
        //            Bitmap area = image.Clone(new Rectangle(offsetX + x * gridX, offsetY + y * gridY, gridX, gridY), image.PixelFormat);
        //            foreach (var template in TemplateList)
        //            {
        //                DateTime startTime1 = DateTime.Now;
        //                List<Rectangle> matches = OpenCVHelper.MatchTemplate(area, template.Value);
        //                if (matches.Count > 0)
        //                {
        //                    var match = matches[0];
        //                    Rectangle match_offseted = new Rectangle(match.X + offsetX + x * gridX, match.Y + offsetY + y * gridY, match.Width, match.Height);
        //                    TemplateGDI.DrawRectangle(Pens.Red, match_offseted);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    //foreach (var template in TemplateList)
        //    //{
        //    //    DateTime startTime1 = DateTime.Now;
        //    //    List<Rectangle> matches = OpenCVHelper.MatchTemplate(image, template.Value);
        //    //    Debug.WriteLine($"模板匹配耗时: {(DateTime.Now - startTime1).TotalSeconds}");
        //    //    foreach (var match in matches)
        //    //    {
        //    //        //TemplateGDI.DrawRectangle(Pens.Red, match);
        //    //    }
        //    //}
        //    Debug.WriteLine($"模板匹配总耗时: {(DateTime.Now - startTime).TotalSeconds}");
        //    pictureBox_show_result.Image = TemplateDisplayBitmap;
        //    return true;
        //}

        bool GetBoardFromPrediction(List<YoloPrediction> predictions, bool refreshBoard = false)
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
                    double chessRatio = prediction.Rectangle.Width / prediction.Rectangle.Height;
                    if (chessRatio > 1.3 || chessRatio < 0.7) continue;
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
            if (refreshBoard)
            {
                BoardAreaRaw = new Rectangle((int)board.X, (int)board.Y, (int)board.Width, (int)board.Height);
            }
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
                double chessRatio = prediction.Rectangle.Width / prediction.Rectangle.Height;
                if (chessRatio > 1.3 || chessRatio < 0.7) continue;
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

                    }));
                }
            }
            Board = tmpBoard;
            return true;
        }

        List<YoloPrediction> ModelPredict(Bitmap image)
        {
            YoloDisplayBitmap = new Bitmap(image.Width, image.Height);
            YoloGDI = Graphics.FromImage(YoloDisplayBitmap);
            YoloGDI.DrawImage(image, 0, 0);
            DateTime st = DateTime.Now;
            List<YoloPrediction> predictions = Model.Predict(image);
            Debug.WriteLine("模型耗时: " + Math.Round((DateTime.Now - st).TotalSeconds, 2).ToString() + "s");
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
            pictureBox_show_result.Image = YoloDisplayBitmap;
            return predictions;
        }

        ResultSource StartAnalyze(string fen)
        {
            if (Settings.UseOpenBook)
            {
                try
                {
                    var resultList = OpenBookHelper.QueryAll(OpenBookList, fen);
                    int index = 0;
                    if (resultList.Count > 0)
                    {
                        if (Settings.OpenbookMode == ProgramSettings.OpenBookMode.Random)
                        {
                            index = Rand.Next(resultList.Count);
                        }
                        else
                        {
                            // sort 
                            resultList.Sort((q1, q2) =>
                            {
                                return q1.Score - q2.Score;
                            });
                            index = 0;
                        }
                        var result = resultList[index];
                        DisplayStatus("命中 " + result.Book + " 开局库");
                        this.Invoke(new Action(() =>
                        {
                            if (result.Score > 20000)
                            {
                                this.Invoke(new Action(() => textBox_engine_log.SelectionColor = MateColor));
                            }
                            else if (result.Score >= 0)
                            {
                                this.Invoke(new Action(() => textBox_engine_log.SelectionColor = PositiveColor));
                            }
                            else
                            {
                                this.Invoke(new Action(() => textBox_engine_log.SelectionColor = NegativeColor));
                            }
                            textBox_engine_log.AppendText(
                                $"开局库 {result.Book} {result.Memo}\r\n" +
                                $"得分: {result.Score}\r\n" + Utils.FenToChina(Board, new string[] { result.Move }, Settings.RedSide) +
                                "\r\n"
                                );
                            if (checkBox_auto_scroll.Checked)
                            {
                                textBox_engine_log.ScrollToCaret();
                            }
                            EngineAnalyzeCount++;
                            ReceiveBestMove(result.Move, "", ResultSource.Openbook);
                        }));
                        return ResultSource.Openbook;
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        textBox_engine_log.AppendText("查询开局库时出错: " + ex.Message);
                    }));
                }
            }
            if (Settings.UseChessDB)
            {
                try
                {
                    DisplayStatus("检索云库中");
                    var result = ChessDBHelper.GetPV(fen);
                    if (result != null)
                    {
                        DisplayStatus("命中云库");
                        this.Invoke(new Action(() =>
                        {
                            if (result.Score > 20000)
                            {
                                this.Invoke(new Action(() => textBox_engine_log.SelectionColor = MateColor));
                            }
                            else if (result.Score >= 0)
                            {
                                this.Invoke(new Action(() => textBox_engine_log.SelectionColor = PositiveColor));
                            }
                            else
                            {
                                this.Invoke(new Action(() => textBox_engine_log.SelectionColor = NegativeColor));
                            }
                            textBox_engine_log.AppendText($"云库 深度: {result.Depth} 得分: {result.Score}\r\n");
                            textBox_engine_log.SelectionColor = Color.Black;
                            textBox_engine_log.AppendText(Utils.FenToChina(Board, result.PV.ToArray(), Settings.RedSide));
                            textBox_engine_log.AppendText("\r\n\r\n");
                            if (checkBox_auto_scroll.Checked)
                            {
                                textBox_engine_log.ScrollToCaret();
                            }
                            string bestMove = result.PV[0];
                            string ponderMove = "";
                            if (result.PV.Count >= 2) ponderMove = result.PV[1];
                            EngineAnalyzeCount++;
                            ReceiveBestMove(bestMove, ponderMove, ResultSource.ChessDB);
                        }));
                        return ResultSource.ChessDB;
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        textBox_engine_log.AppendText("查询云库时出错: " + ex.Message);
                    }));
                }
            }

            DisplayStatus("开始引擎计算");
            EngineAnalyzingBoard = (string[,])Board.Clone();
            Engine.StartAnalyze(fen, Settings.StepTime);
            return ResultSource.Engine;
        }

        void ReloadBoard()
        {
            var compareWithLast = Utils.CompareBoard(LastBoard, Board);
            var compareWithExpected = Utils.CompareBoard(ExpectedBoard, Board);
            if (compareWithLast.DiffCount == 0) return;
            string opponentSymbol = Settings.RedSide ? "b_" : "r_";
            string mySymbol = Settings.RedSide ? "r_" : "b_";
            Debug.WriteLine($"Diff Result: {compareWithLast.DiffCount} R: {compareWithLast.RedDiff} B: {compareWithLast.BlackDiff}");
            if (StartFromOpponent)
            {
                StartFromOpponent = false;
                if (Settings.AnalyzingMode)
                {
                    InvalidCount = 0;
                    LastBoard = (string[,])Board.Clone();
                    DisplayStatus("分析对手");
                    string oppofen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b", Settings.RedSide ? "b" : "w");
                    EngineAnalyzingBoard = (string[,])Board.Clone();
                    StartAnalyze(oppofen);
                }
                else
                {
                    LastBoard = (string[,])Board.Clone();
                    ExpectedBoard = null;
                    ExpectedMove = "";
                    RenderDisplayBoard();
                    DisplayStatus("从对手开始");
                    return;
                }
            }
            if (compareWithLast.BlackDiff > 1 || compareWithLast.RedDiff > 1)
            {
                InvalidCount++;
                DisplayStatus($"棋盘变化数异常: 黑棋消失: {compareWithLast.BlackDiff} 红棋消失:{compareWithLast.RedDiff} 判断为动画中，跳过 | 错误次数: {InvalidCount}");
                if (InvalidCount > 2)
                {
                    DisplayStatus("错误次数过多，自动重置");
                    ResetDetection();
                }
                return;
            }
            if (compareWithLast.DiffCount != 2 && compareWithExpected.DiffCount != 2 && compareWithLast.DiffCount < 10)
            {
                InvalidCount++;
                DisplayStatus($"棋盘变化数异常: {compareWithLast.DiffCount} 判断为动画中，跳过 | 错误次数: {InvalidCount}");
                if (InvalidCount > 2)
                {
                    DisplayStatus("错误次数过多，自动重置");
                    ResetDetection();
                }
                return;
            }
            if (compareWithLast.DiffCount == 2 && compareWithLast.Chess != null && compareWithLast.Chess.Contains(mySymbol))
            {
                RenderDisplayBoard();
                if (Settings.AnalyzingMode)
                {
                    InvalidCount = 0;
                    LastBoard = (string[,])Board.Clone();
                    EngineAnalyzeCount++;
                    DisplayStatus("开始引擎计算");
                    string oppofen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b", Settings.RedSide ? "b" : "w");
                    EngineAnalyzingBoard = (string[,])Board.Clone();
                    Engine.StartAnalyze(oppofen, Settings.StepTime);
                }
                else
                {
                    InvalidCount = 0;
                    LastBoard = (string[,])Board.Clone();
                    DisplayStatus("己方棋子变化，跳过分析");
                    ExpectedMove = "";
                }
                return;
            }
            if (compareWithExpected.DiffCount == 0)
            {
                LastBoard = (string[,])Board.Clone();
                ExpectedMove = "";
                RenderDisplayBoard();
                DisplayStatus("和预期棋盘一样，跳过");
                return;
            }
            if ((compareWithLast.BlackDiff > 1 || compareWithLast.RedDiff > 1) && compareWithLast.DiffCount < 10)
            {
                InvalidCount++;
                DisplayStatus($"差别过大，可能有动画，跳过 | 错误次数: {InvalidCount}");
                if (InvalidCount > 2)
                {
                    DisplayStatus("错误次数过多，自动重置");
                    ResetDetection();
                }
                return;
            }
            RenderDisplayBoard();
            InvalidCount = 0;
            EngineAnalyzeCount++;
            LastBoard = (string[,])Board.Clone();
            
            string fen = Utils.BoardToFen(Board, Settings.RedSide ? "w" : "b", Settings.RedSide ? "w" : "b");
            if (Settings.BackgroundAnalysis && BackgroundAnalyzing)
            {
                BackgroundAnalyzing = false;
                var compareWithPonder = Utils.CompareBoard(PonderBoard, Board);
                if (compareWithPonder.DiffCount == 0)
                {
                    DisplayStatus("后台思考命中");
                    Engine.PonderHit();
                }
                else
                {
                    DisplayStatus("开始引擎计算");
                    Engine.PonderMiss();
                    EngineAnalyzingBoard = (string[,])Board.Clone();
                    StartAnalyze(fen);
                }
            }
            else
            {
                StartAnalyze(fen);
            }
        }

        private void Engine_InfoEvent(string cmd, Dictionary<string, string> infos)
        {

            string info_str = "";
            if (infos.ContainsKey("score"))
            {
                if (Settings.StopWhenMate)
                {
                    int currScore = 0;
                    if (infos["score"].Contains("绝杀") && int.Parse(infos["score"].Split('(').Last().Split(')').First()) > 0)
                    {
                        if (FirstMate)
                        {
                            FirstMate = false;
                            Engine.StopAnalyze();
                        }                            
                    }
                    else if (int.TryParse(infos["score"], out currScore) && currScore > 20000)
                    {
                        if (FirstMate)
                        {
                            FirstMate = false;
                            Engine.StopAnalyze();
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
                int score;
                if (int.TryParse(infos["score"], out score))
                {
                    if (score >= 0)
                    {
                        this.Invoke(new Action(() => textBox_engine_log.SelectionColor = PositiveColor));
                    }
                    else
                    {
                        this.Invoke(new Action(() => textBox_engine_log.SelectionColor = NegativeColor));
                    }
                }
                else
                {
                    if (infos["score"].Contains("绝杀"))
                    {
                        this.Invoke(new Action(() => textBox_engine_log.SelectionColor = MateColor));
                    }
                }
                info_str = "";
                if (BackgroundAnalyzing)
                {
                    info_str += "[后台思考] ";
                }
                info_str += (
                    $"深度: {infos["depth"]} " +
                    $"得分: {infos["score"]} " +
                    $"时间: {(infos.ContainsKey("time") ? Math.Round(double.Parse(infos["time"]) / 1000, 1) : 0)}秒 " +
                    $"nps: {(infos.ContainsKey("nps") ? Math.Round(double.Parse(infos["nps"]) / 1000) : 0)}K" +
                    "\r\n"
                    );
                this.Invoke(new Action(() =>
                {
                    textBox_engine_log.AppendText(info_str);
                }));
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
                    info_str = Utils.FenToChina(EngineAnalyzingBoard, moves, Settings.RedSide) + "\r\n";
                    this.Invoke(new Action(() =>
                    {
                        textBox_engine_log.SelectionColor = NormalColor;
                        textBox_engine_log.AppendText(info_str + "\r\n");
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                if (!BackgroundAnalyzing)
                {
                    BackgroundAnalysisMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
                    BestMove = new ChessMove(fromPoint, toPoint);
                    if (moves.Length > 1)
                    {
                        string ponderMove = moves[1];
                        Point ponderFrom = Utils.Move2Point(ponderMove.Substring(0, 2), Settings.RedSide);
                        Point ponderTo = Utils.Move2Point(ponderMove.Substring(2, 2), Settings.RedSide);
                        PonderMove = new ChessMove(ponderFrom, ponderTo);
                    }
                }
                else
                {
                    BackgroundAnalysisMove = new ChessMove(fromPoint, toPoint);
                }
                RenderDisplayBoard();
            }
            if (info_str != "")
            {
                this.Invoke(new Action(() =>
                {
                    if (checkBox_auto_scroll.Checked)
                    {
                        textBox_engine_log.ScrollToCaret();
                    }
                }));
            }
        }

        void ReceiveBestMove(string bestMove, string ponderMove, ResultSource source)
        {
            bestMove = bestMove.Trim('\0', ' ');
            ponderMove = ponderMove.Trim('\0', ' ');
            string mySideStr = Settings.RedSide ? "r" : "b";
            if (bestMove.Length != 4) return;
            Point fromPoint = Utils.Move2Point(bestMove.Substring(0, 2), Settings.RedSide);
            Point toPoint = Utils.Move2Point(bestMove.Substring(2, 2), Settings.RedSide);
            Point ponderFrom = new Point(-1, -1);
            Point ponderTo = new Point(-1, -1);
            BestMove = new ChessMove(fromPoint, toPoint);
            if (ponderMove != "")
            {
                ponderFrom = Utils.Move2Point(ponderMove.Substring(0, 2), Settings.RedSide);
                ponderTo = Utils.Move2Point(ponderMove.Substring(2, 2), Settings.RedSide);
                PonderMove = new ChessMove(ponderFrom, ponderTo);
            }
            else
            {
                PonderMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
            }
            if (!BackgroundAnalyzing)
            {
                BackgroundAnalysisMove = new ChessMove(new Point(-1, -1), new Point(-1, -1));
            }
            RenderDisplayBoard();
            ExpectedBoard = (string[,])Board.Clone();
            ExpectedBoard[toPoint.X, toPoint.Y] = ExpectedBoard[fromPoint.X, fromPoint.Y];
            ExpectedBoard[fromPoint.X, fromPoint.Y] = null;
            ExpectedMove = bestMove;
            if (Settings.AutoGo && Board[fromPoint.X, fromPoint.Y] != null && Board[fromPoint.X, fromPoint.Y].StartsWith(mySideStr + "_"))
            {
                ChangeDetectedAfterClick = false;
                PlayChess(bestMove);
                if (Settings.BackgroundAnalysis && ponderMove != "" && source == ResultSource.Engine)
                {
                    PonderBoard = (string[,])ExpectedBoard.Clone();
                    PonderBoard[ponderTo.X, ponderTo.Y] = PonderBoard[ponderFrom.X, ponderFrom.Y];
                    PonderBoard[ponderFrom.X, ponderFrom.Y] = null;
                    string ponderFen = Utils.BoardToFen(PonderBoard, Settings.RedSide ? "w" : "b", Settings.RedSide ? "w" : "b");
                    EngineAnalyzingBoard = (string[,])PonderBoard.Clone();
                    Engine.StartAnalyzePonder(ponderFen, Settings.StepTime);
                    BackgroundAnalyzing = true;
                }
            }
        }

        private void Engine_BestMove_Event(string bestMove, string ponderMove)
        {
            EngineAnalyzeCount--;
            ReceiveBestMove(bestMove, ponderMove, ResultSource.Engine);
        }


        public void PlayChess(string move)
        {
            if (move.Length != 4) return;
            Point fromPoint = Utils.Move2Point(move.Substring(0, 2), Settings.RedSide);
            Point toPoint = Utils.Move2Point(move.Substring(2, 2), Settings.RedSide);
            Point fromClickPoint = ClickPositionMap[fromPoint.X, fromPoint.Y];
            Point toClickPoint = ClickPositionMap[toPoint.X, toPoint.Y];
            ChangeDetectedAfterClick = false;
            MouseLeftClient_2Point(fromClickPoint.X, fromClickPoint.Y, toClickPoint.X, toClickPoint.Y);
        }
    }
}
