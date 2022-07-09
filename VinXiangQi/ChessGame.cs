using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinXiangQi
{
    internal class ChessGame
    {
        public class Chessman
        {
            public enum Side
            {
                Red,
                Black
            }

            public enum ChessTypes
            {
                che,
                ma,
                pao,
                xiang,
                shi,
                jiang,
                bing
            }

            public Side ChessSide;
            public ChessTypes ChessType;
            Dictionary<ChessTypes, string> FenTable = new Dictionary<ChessTypes, string> {
                { ChessTypes.che, "r" }, { ChessTypes.ma, "n" }, { ChessTypes.pao, "c" }, { ChessTypes.xiang, "b" },
                { ChessTypes.shi, "a" }, { ChessTypes.jiang, "k" }, { ChessTypes.bing, "p" }
            };

            public string Fen
            {
                get
                {
                    string fen = FenTable[ChessType];
                    if (ChessSide == Side.Red)
                    {
                        fen = fen.ToUpper();
                    }
                    return fen;
                }
                set
                {
                    char chr = value[0];
                }
            }
        }
    }
}
