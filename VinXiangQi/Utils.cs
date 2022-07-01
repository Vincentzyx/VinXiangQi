using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Yolov5Net.Scorer;

namespace VinXiangQi
{
    public static class Utils
    {
        

        public static string BoardToFen(string[,] Board, string next_player = "w")
        {
            Dictionary<string, string> FenMap = new Dictionary<string, string>
            {
                { "che",  "r" }, {"ma", "n"}, {"xiang", "b"}, {"shi", "a"}, {"jiang", "k"}, {"pao", "c" }, {"bing", "p" }
            };
            string fen = "";
            int emptyCount = 0;
            if (next_player == "w")
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
            
            fen = fen.Substring(0, fen.Length - 1) + " " + next_player + " - - 0 1";
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
