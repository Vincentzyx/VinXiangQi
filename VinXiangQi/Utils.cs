using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Yolov5Net.Scorer;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VinXiangQi
{
    public static class Utils
    {
        public struct BoardCompareResult
        {
            public Point From;
            public Point To;
            public string Chess;
            public string Target;
            public int DiffCount;
            public int RedDiff;
            public int BlackDiff;
        }
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        public static string intToChina(int x)
        {
            if (x < 0 || x > 9) return "";
            string[] stringNum = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            return stringNum[x];
        }
        
        public static string nameToChina(string name)
        {
            bool isRed = name.Substring(0, 1) == "r";
            name = name.Substring(2);
            if (name == "che")
            {
                name = "车";
            }
            else if (name == "ma")
            {
                name = "马";
            }
            else if (name == "xiang")
            {
                name = isRed ? "相" : "象";
            }
            else if (name == "shi")
            {
                name = isRed ? "仕" : "士";
            }
            else if (name == "jiang")
            {
                name = isRed ? "帅" : "将";
            }
            else if (name == "pao")
            {
                name = "炮";
            }
            else if (name == "bing")
            {
                name = isRed ? "兵" : "卒";
            }
            return name;
        }

        public static string ChangeStrToSBC(string str)
        {

            char[] c = str.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 0)
                    {
                        b[0] = (byte)(b[0] - 32);
                        b[1] = 255;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            //半角  
            string strNew = new string(c);
            return strNew;
        }
        
        public static string FenToChina(string[,] cboard, string[] moves, bool redSide)
        {
            string[,] board = (string[,])cboard.Clone();
            moves = (string[])moves.Clone();
            for (int i = 0; i < moves.Length; i++)
            {
                string ret = "";
                Point fromPoint = Utils.Move2Point(moves[i].Substring(0, 2), redSide);
                Point toPoint = Utils.Move2Point(moves[i].Substring(2, 2), redSide);
                string name = board[fromPoint.X, fromPoint.Y];
                if (name == "" || name == null) continue;
                bool isRed = name.Substring(0, 1) == "r";

                int X1 = fromPoint.X + 1;
                int X2 = toPoint.X + 1;
                int Y = toPoint.Y - fromPoint.Y;
                if (redSide == isRed)
                {
                    X1 = 10 - X1;
                    X2 = 10 - X2;
                    Y = -1 * Y;
                }
                string MoveName = "";

                //车马炮兵,判断前中后
                if (name.Contains("che") || name.Contains("ma") || name.Contains("pao") || name.Contains("bing"))
                {
                    int Front = 0, Back = 0;
                    for (int j = 0; j < 10; j++)
                    {
                        if (board[fromPoint.X, j] == name)
                        {
                            if (j < fromPoint.Y) Front++;
                            if (j > fromPoint.Y) Back++;
                        }
                    }
                    if (Front > 0 || Back > 0)
                    {
                        if (Back == 0 && Front > 0)
                        {
                            MoveName = "后";
                        }
                        else if (Back > 0 && Front == 0)
                        {
                            MoveName = "前";
                        }
                        else if (Back == 1 && Front == 1)
                        {
                            MoveName = "中";
                        }
                        else
                        {
                            MoveName = intToChina(Front + 1);//需要转为大写汉字
                        }
                    }
                }
                string StartStr = "", EndStr = "";
                if (MoveName == "") StartStr = isRed ? intToChina(X1) : ChangeStrToSBC(X1 + "");
                if (isRed)
                {
                    EndStr = intToChina(X2);
                }
                else
                {
                    EndStr = ChangeStrToSBC(X2 + "");
                }
                MoveName += nameToChina(name);
                string MoveDir = "";
                if (Y == 0)
                {
                    MoveDir = "平" + (isRed ? intToChina(X2) : ChangeStrToSBC(X2 + ""));
                }
                else if (Y > 0)
                {
                    MoveDir = "进" + (isRed ? intToChina(Y) : ChangeStrToSBC(Y + ""));
                }
                else
                {
                    MoveDir = "退" + (isRed ? intToChina(-Y) : ChangeStrToSBC(-Y + ""));
                }

                board[fromPoint.X, fromPoint.Y] = "";
                board[toPoint.X, toPoint.Y] = name;

                name = name.Substring(2);
                if (name == "jiang")
                {
                    ret = MoveName + StartStr + MoveDir;
                }
                else if (name == "shi" || name == "xiang" || name == "ma")
                {
                    if (Y > 0)
                    {
                        MoveDir = "进";
                    }
                    else
                    {
                        MoveDir = "退";
                    }
                    ret = MoveName + StartStr + MoveDir + EndStr;
                }
                else
                {
                    ret = MoveName + StartStr + MoveDir;
                }
                moves[i] = ret;

            }
            return string.Join(" ", moves);
        }

        public static BoardCompareResult CompareBoard(string[,] from, string[,] to)
        {
            BoardCompareResult result = new BoardCompareResult();
            int diffCount = 0;
            if (from == null || to == null)
            {
                result.DiffCount = 32;
                return result;
            }
            int bFromCount = 0;
            int bToCount = 0;
            int rFromCount = 0;
            int rToCount = 0;
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (from[x, y] != null && from[x, y].Contains("b_"))
                    {
                        bFromCount++;
                    }
                    if (to[x, y] != null && to[x, y].Contains("b_"))
                    {
                        bToCount++;
                    }
                    if (from[x, y] != null && from[x, y].Contains("r_"))
                    {
                        rFromCount++;
                    }
                    if (to[x, y] != null && to[x, y].Contains("r_"))
                    {
                        rToCount++;
                    }
                    if (from[x, y] != to[x, y])
                    {
                        if (to[x, y] == null)
                        {
                            result.From = new Point(x, y);
                            diffCount++;
                        }
                        else
                        {
                            result.To = new Point(x, y);
                            result.Chess = to[x, y];
                            result.Target = from[x, y];
                            diffCount++;
                        }
                    }
                }
            }
            result.RedDiff = rFromCount - rToCount;
            result.BlackDiff = bFromCount - bToCount;
            result.DiffCount = diffCount;
            return result;
        }        

        public static string BoardToFen(string[,] Board, string myPos, string nextPlayer)
        {
            Dictionary<string, string> FenMap = new Dictionary<string, string>
            {
                { "che",  "r" }, {"ma", "n"}, {"xiang", "b"}, {"shi", "a"}, {"jiang", "k"}, {"pao", "c" }, {"bing", "p" }
            };
            string fen = "";
            int emptyCount = 0;
            if (myPos == "w")
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        if (Board[x, y] == null)
                        {
                            emptyCount++;
                        }
                        else
                        {
                            if (emptyCount > 0)
                            {
                                fen += emptyCount.ToString();
                                emptyCount = 0;
                            }
                            string[] nameInfo = Board[x, y].Split('_');
                            if (nameInfo[0] == "r") fen += FenMap[nameInfo[1]].ToUpper();
                            else fen += FenMap[nameInfo[1]];
                        }
                    }
                    if (emptyCount > 0)
                    {
                        fen += emptyCount.ToString();
                        emptyCount = 0;
                    }
                    fen += "/";
                }
            }
            else
            {
                for (int y = 9; y >= 0; y--)
                {
                    for (int x = 8; x >= 0; x--)
                    {
                        if (Board[x, y] == null)
                        {
                            emptyCount++;
                        }
                        else
                        {
                            if (emptyCount > 0)
                            {
                                fen += emptyCount.ToString();
                                emptyCount = 0;
                            }
                            string[] nameInfo = Board[x, y].Split('_');
                            if (nameInfo[0] == "r") fen += FenMap[nameInfo[1]].ToUpper();
                            else fen += FenMap[nameInfo[1]];
                        }
                    }
                    if (emptyCount > 0)
                    {
                        fen += emptyCount.ToString();
                        emptyCount = 0;
                    }
                    fen += "/";
                }
            }
            
            fen = fen.Substring(0, fen.Length - 1) + " " + nextPlayer;
            return fen;
        }

        public static string MirrorFenLeftRight(string fen)
        {
            // rnbaka2r/9/1c2b1nc1/p1p1p1p1p/9/2P6/P3P1P1P/1CN4C1/9/R1BAKABNR w - - 0 1
            string[] args = fen.Split(' ');
            string board = args[0];
            string[] rows = board.Split('/');
            List<string> newRows = new List<string>();
            for (int i = 0; i < rows.Length; i++)
            {
                string row = rows[i];
                string newRow = "";
                for (int j = row.Length - 1; j >= 0; j--)
                {
                    newRow += row[j];
                }
                newRows.Add(newRow);
            }
            string newBoard = string.Join("/", newRows);
            return newBoard + " " + string.Join(" ", args.Skip(1));
        }

        public static string MirrorFenRedBlack(string fen)
        {
            // rnbaka2r/9/1c2b1nc1/p1p1p1p1p/9/2P6/P3P1P1P/1CN4C1/9/R1BAKABNR w - - 0 1
            string[] args = fen.Split(' ');
            string board = args[0];
            string[] rows = board.Split('/');
            List<string> newRows = new List<string>();
            for (int i = rows.Length - 1; i >= 0; i--)
            {
                string newRow = "";
                foreach (char c in rows[i])
                {
                    string s = c.ToString();
                    if (c >= 'a' && c <= 'z')
                    {
                        newRow += s.ToUpper();
                    }
                    else
                    {
                        newRow += s.ToLower();
                    }
                }
                newRows.Add(newRow);
            }
            string newBoard = string.Join("/", newRows);
            string nextPlayer = args[1];
            nextPlayer = nextPlayer == "b" ? "w" : "b";
            return newBoard + " " + nextPlayer + " " + string.Join(" ", args.Skip(2));
        }

        public static bool CheckChessmanValid(string chess, int x, int y, bool redSide)
        {
            string[] args = chess.Split('_');
            string side = args[0];
            string type = args[1];
            if (!redSide)
            {
                y = 9 - y;
            }
            if (side == "r")
            {
                if (type == "jiang")
                {
                    if (x >= 3 && x <= 5 && y >= 7 && y <= 9)
                    {
                        return true;
                    }
                    return false;
                }
                else if (type == "shi")
                {
                    if (x == 3 && y == 7 || x == 5 && y == 7 || x == 4 && y == 8 || x == 3 && y == 9 || x == 5 && y == 9)
                    {
                        return true;
                    }
                    return false;
                }
                else if (type == "xiang")
                {
                    if (x == 2 && y == 9 || x == 6 && y == 9 ||
                        x == 0 && y == 7 || x == 4 && y == 7 || x == 8 && y == 7 ||
                        x == 2 && y == 5 || x == 6 && y == 5)
                    {
                        return true;
                    }
                    return false;
                }
                else if (type == "bing")
                {
                    if ((y == 5 || y == 6) && (x == 0 || x == 2 || x == 4 || x == 6 || x == 8))
                    {
                        return true;
                    }
                    else if (y <= 4)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (type == "jiang")
                {
                    if (x >= 3 && x <= 5 && y >= 0 && y <= 2)
                    {
                        return true;
                    }
                    return false;
                }
                else if (type == "shi")
                {
                    if (x == 3 && y == 0 || x == 5 && y == 0 || x == 4 && y == 1 || x == 3 && y == 2 || x == 5 && y == 2)
                    {
                        return true;
                    }
                    return false;
                }
                else if (type == "xiang")
                {
                    if (x == 2 && y == 0 || x == 6 && y == 0 || 
                        x == 0 && y == 2 || x == 4 && y == 2 || x == 8 && y == 2 ||
                        x == 2 && y == 4 || x == 6 && y == 4)
                    {
                        return true;
                    }
                    return false;
                }
                else if (type == "bing")
                {
                    if ((y == 3 || y == 4) && (x == 0 || x == 2 || x == 4 || x == 6 || x == 8))
                    {
                        return true;
                    }
                    else if (y >= 5)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool CheckBoardValid(string[,] board, bool redSide)
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            Dictionary<string, int> maxCounts = new Dictionary<string, int>
            {
                { "che", 2 },
                { "ma", 2 },
                { "pao", 2 },
                { "xiang", 2 },
                { "shi", 2 },
                { "jiang", 1 },
                { "bing", 5 }
            };
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (!string.IsNullOrEmpty(board[x, y]))
                    {
                        string chessMan = board[x, y];
                        if (counts.ContainsKey(chessMan))
                        {
                            counts[chessMan]++;
                        }
                        else
                        {
                            counts.Add(chessMan, 1);
                        }
                        if (!CheckChessmanValid(chessMan, x, y, redSide))
                        {
                            return false;
                        }
                    }
                }
            }
            foreach (var c in counts)
            {
                string type = c.Key.Split('_')[1];
                if (c.Value > maxCounts[type])
                {
                    return false;
                }
            }
            if (!counts.ContainsKey("b_jiang") || !counts.ContainsKey("r_jiang"))
            {
                return false;
            }
            return true;
        }

        public static Point Move2Point(string move, bool redSide)
        {
            int x = move[0] - 'a';
            int y = move[1] - '0';
            if (redSide)
            {
                return new Point(x, 9 - y);
            }
            else
            {
                return new Point(8 - x, y);
            }
        }

        public static string Point2Move(Point from, Point to)
        {

            string[] letters = "a b c d e f g h i j k l m n o p q r s t u v w x y z".Split(' ');
            string move = "";
            move += letters[from.X];
            move += (9 - from.Y).ToString();
            move += letters[to.X];
            move += (9 - to.Y).ToString();
            return move;
        }

        public static Rectangle ExpendArea(Rectangle area, Size maxSize)
        {
            float gridWidth = area.Width / 8;
            float gridHeight = area.Height / 9;
            Rectangle newArea = new Rectangle((int)(area.X - gridWidth), (int)(area.Y - gridHeight), (int)(area.Width + gridWidth * 2), (int)(area.Height + gridHeight * 2));
            if (newArea.X < 0) newArea.X = 0;
            if (newArea.X + newArea.Width > maxSize.Width) newArea.Width = maxSize.Width - newArea.X;
            if (newArea.Y < 0) newArea.Y = 0;
            if (newArea.Y + newArea.Height > maxSize.Height) newArea.Height = maxSize.Height - newArea.Y;
            return newArea;
        }

        public static Rectangle RestoreArea(Rectangle cropArea, Rectangle boardArea)
        {
            return new Rectangle(cropArea.X + boardArea.X, cropArea.Y + boardArea.Y, boardArea.Width, boardArea.Height);
        }
    }
}
