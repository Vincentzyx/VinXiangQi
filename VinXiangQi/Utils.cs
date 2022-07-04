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

        public static FenToChina_Ss FenToChina_Extern = null;
        static IntPtr pDll, pFuncAddr;

        [DllImport("FenToChina.dll", BestFitMapping = true)]
        public static extern string FenToChina_S(string fen, string pvs, int rows);
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate string FenToChina_Ss(string fen, string pvs, int num);
        
        public static string FenToChina(string fen, string pvs)                            
        {
            //if (FenToChina_Extern == null)
            //{
            //    pDll = LoadLibrary(@"FenToChina.dll");
            //    pFuncAddr = GetProcAddress(pDll, "FenToChina_S");
            //    FenToChina_Extern = (FenToChina_Ss)Marshal.GetDelegateForFunctionPointer(pFuncAddr, typeof(FenToChina_Ss));
            //}
            //string result = FenToChina_Extern(fen, pvs, 1);
            return FenToChina_S(fen, pvs, 1);
        }

        public static void FreeFenToChina()
        {
            if (FenToChina_Extern != null)
            {
                FreeLibrary(pDll);
            }
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
            
            fen = fen.Substring(0, fen.Length - 1) + " " + nextPlayer + " - - 0 1";
            return fen;
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
