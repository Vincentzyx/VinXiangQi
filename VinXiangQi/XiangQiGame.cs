using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinXiangQi;

namespace VinXiangQi
{
    public class XiangQiGame
    {
        string fen;
        string[,] board;
        string turn;

        public XiangQiGame(string fen="rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w")
        {
            this.fen = fen;
            this.board = this.fenToBoard(fen);
        }

        public class MoveResult
        {
            public Point From;
            public Point To;
            public string[,] Board;
            public string TargetPiece;
            public string MyPiece;
            public MoveResult(Point from, Point to, string[,] board, string targetPiece, string myPiece)
            {
                From = from;
                To = to;
                Board = board;
                TargetPiece = targetPiece;
                MyPiece = myPiece;
            }
        }


        Dictionary<string, MoveResult> generateAllMoves()
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (board[x, y] != "")
                    {
                        var moveMap = getValidMoveMap(new Point(x, y));
                        for (int iy = 0; iy < 10; iy++)
                        {
                            for (int ix = 0; ix < 9; ix++)
                            {
                                if (moveMap[ix, iy] == "eat" || moveMap[ix, iy] == "go")
                                {
                                    var newBoard = (string[,])board.Clone();
                                    string fromPiece = board[x, y];
                                    string toPiece = board[ix, iy];
                                    Point from = new Point(x, y);
                                    Point to = new Point(ix, iy);

                                }
                            }
                        }
                    }
                }
            }
            return null;
        }


        string getSide(string piece)
        {
            string redPieces = "RNBAKCP";
            return redPieces.Contains(piece) ? "w" : "b";
        }

        string getType(string piece)
        {
            return piece.ToLower();
        }

        string getValidType(string mySide, Point target)
        {
            string targetPiece = this.getPiece(target);
            if (targetPiece == null)
            {
                return "invalid";
            }
            else if (targetPiece == "")
            {
                return "go";
            }
            else if (mySide != this.getSide(targetPiece))
            {
                return "eat";
            }
            else
            {
                return "block";
            }
        }

        string[,] getValidMoveMap(Point pos)
        {
            string piece = this.getPiece(pos);
            string side = this.getSide(piece);
            string type = this.getType(piece);
            if (type == "r")
            {
                return this.getValidMoveMapRooks(side, pos);
            }
            else if (type == "n")
            {
                return this.getValidMoveMapKnights(side, pos);
            }
            else if (type == "a")
            {
                return this.getValidMoveMapAdvisors(side, pos);
            }
            else if (type == "b")
            {
                return this.getValidMoveMapBishops(side, pos);
            }
            else if (type == "k")
            {
                return this.getValidMoveMapKings(side, pos);
            }
            else if (type == "c")
            {
                return this.getValidMoveMapCannons(side, pos);
            }
            else if (type == "p")
            {
                return this.getValidMoveMapPawns(side, pos);
            }
            else
            {
                return this.getEmptyBoard();
            }
        }

        string[,] getValidMoveMapRooks(string side, Point pos)
        {
            string[,] moveMap = this.getEmptyBoard();
            int x = pos.X;
            int y = pos.Y;
            for (int xi = x; xi < 9; xi++)
            {
                if (xi == x) continue;
                string validType = this.getValidType(side, new Point(xi, y));
                if (validType == "block")
                {
                    break;
                }
                moveMap[y, xi] = validType;
                if (validType == "eat")
                {
                    break;
                }
            }
            for (int xi = x; xi >= 0; xi--)
            {
                if (xi == x) continue;
                string validType = this.getValidType(side, new Point(xi, y));
                if (validType == "block")
                {
                    break;
                }
                moveMap[y, xi] = validType;
                if (validType == "eat")
                {
                    break;
                }
            }
            for (int yi = y; yi < 10; yi++)
            {
                if (yi == y) continue;
                string validType = this.getValidType(side, new Point(x, yi));
                if (validType == "block")
                {
                    break;
                }
                moveMap[yi, x] = validType;
                if (validType == "eat")
                {
                    break;
                }
            }
            for (int yi = y; yi >= 0; yi--)
            {
                if (yi == y) continue;
                string validType = this.getValidType(side, new Point(x, yi));
                if (validType == "block")
                {
                    break;
                }
                moveMap[yi, x] = validType;
                if (validType == "eat")
                {
                    break;
                }
            }
            return moveMap;
        }

        string[,] getValidMoveMapKnights(string side, Point pos)
        {
            string[,] moveMap = this.getEmptyBoard();
            int x = pos.X;
            int y = pos.Y;
            if (this.getPiece(new Point(x + 1, y)) == "")
            {
                this.setMoveMap(moveMap, new Point(x + 2, y + 1), this.getValidType(side, new Point(x + 2, y + 1)));
                this.setMoveMap(moveMap, new Point(x + 2, y - 1), this.getValidType(side, new Point(x + 2, y - 1)));
            }
            if (this.getPiece(new Point(x - 1, y)) == "")
            {
                this.setMoveMap(moveMap, new Point(x - 2, y + 1), this.getValidType(side, new Point(x - 2, y + 1)));
                this.setMoveMap(moveMap, new Point(x - 2, y - 1), this.getValidType(side, new Point(x - 2, y - 1)));
            }
            if (this.getPiece(new Point(x, y + 1)) == "")
            {
                this.setMoveMap(moveMap, new Point(x + 1, y + 2), this.getValidType(side, new Point(x + 1, y + 2)));
                this.setMoveMap(moveMap, new Point(x - 1, y + 2), this.getValidType(side, new Point(x - 1, y + 2)));
            }
            if (this.getPiece(new Point(x, y - 1)) == "")
            {
                this.setMoveMap(moveMap, new Point(x + 1, y - 2), this.getValidType(side, new Point(x + 1, y - 2)));
                this.setMoveMap(moveMap, new Point(x - 1, y - 2), this.getValidType(side, new Point(x - 1, y - 2)));
            }
            return moveMap;
        }

        string[,] getValidMoveMapAdvisors(string side, Point pos)
        {
            string[,] moveMap = this.getEmptyBoard();
            int x = pos.X;
            int y = pos.Y;
            int y_lower, y_upper;
            if (y <= 2)
            {
                y_lower = 0;
                y_upper = 2;
            }
            else if (y >= 7)
            {
                y_lower = 7;
                y_upper = 9;
            }
            else
            {
                y_lower = y - 1;
                y_upper = y + 1;
            }
            int x_lower = 3;
            int x_upper = 5;
            if (x + 1 <= x_upper && y + 1 <= y_upper)
            {
                string moveType = this.getValidType(side, new Point(x + 1, y + 1));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x + 1, y + 1), moveType);
                }
            }
            if (x - 1 >= x_lower && y + 1 <= y_upper)
            {
                string moveType = this.getValidType(side, new Point(x - 1, y + 1));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x - 1, y + 1), moveType);
                }
            }
            if (x + 1 <= x_upper && y - 1 >= y_lower)
            {
                string moveType = this.getValidType(side, new Point(x + 1, y - 1));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x + 1, y - 1), moveType);
                }
            }
            if (x - 1 >= x_lower && y - 1 >= y_lower)
            {
                string moveType = this.getValidType(side, new Point(x - 1, y - 1));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x - 1, y - 1), moveType);
                }
            }
            return moveMap;
        }

        string[,] getValidMoveMapBishops(string side, Point pos)
        {
            string[,] moveMap = this.getEmptyBoard();
            int x = pos.X;
            int y = pos.Y;
            int y_lower, y_upper;
            if (y <= 4)
            {
                y_lower = 0;
                y_upper = 4;
            }
            else if (y >= 5)
            {
                y_lower = 5;
                y_upper = 9;
            }
            else
            {
                y_lower = y - 1;
                y_upper = y + 1;
            }
            if (x + 2 <= 9 && y + 2 <= y_upper)
            {
                string blocker = this.getPiece(new Point(x + 1, y + 1));
                if (blocker == "")
                {
                    string moveType = this.getValidType(side, new Point(x + 2, y + 2));
                    if (moveType == "eat" || moveType == "go")
                    {
                        this.setMoveMap(moveMap, new Point(x + 2, y + 2), moveType);
                    }
                }
            }
            if (x - 2 >= 0 && y + 2 <= y_upper)
            {
                string blocker = this.getPiece(new Point(x - 1, y + 1));
                if (blocker == "")
                {
                    string moveType = this.getValidType(side, new Point(x - 2, y + 2));
                    if (moveType == "eat" || moveType == "go")
                    {
                        this.setMoveMap(moveMap, new Point(x - 2, y + 2), moveType);
                    }
                }
            }
            if (x + 2 <= 9 && y - 2 >= y_lower)
            {
                string blocker = this.getPiece(new Point(x + 1, y - 1));
                if (blocker == "")
                {
                    string moveType = this.getValidType(side, new Point(x + 2, y - 2));
                    if (moveType == "eat" || moveType == "go")
                    {
                        this.setMoveMap(moveMap, new Point(x + 2, y - 2), moveType);
                    }
                }
            }
            if (x - 2 >= 0 && y - 2 >= y_lower)
            {
                string blocker = this.getPiece(new Point(x - 1, y - 1));
                if (blocker == "")
                {
                    string moveType = this.getValidType(side, new Point(x - 2, y - 2));
                    if (moveType == "eat" || moveType == "go")
                    {
                        this.setMoveMap(moveMap, new Point(x - 2, y - 2), moveType);
                    }
                }
            }
            return moveMap;
        }

        string[,] getValidMoveMapKings(string side, Point pos)
        {
            string[,] moveMap = this.getEmptyBoard();
            int x = pos.X;
            int y = pos.Y;
            int y_lower, y_upper;
            if (y <= 2)
            {
                y_lower = 0;
                y_upper = 2;
            }
            else if (y >= 7)
            {
                y_lower = 7;
                y_upper = 9;
            }
            else
            {
                y_lower = y - 1;
                y_upper = y + 1;
            }
            int x_lower = 3;
            int x_upper = 5;
            if (x + 1 <= x_upper)
            {
                string moveType = this.getValidType(side, new Point(x + 1, y));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x + 1, y), moveType);
                }
            }
            if (x - 1 >= x_lower)
            {
                string moveType = this.getValidType(side, new Point(x - 1, y));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x - 1, y), moveType);
                }
            }
            if (y + 1 <= y_upper)
            {
                string moveType = this.getValidType(side, new Point(x, y + 1));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x, y + 1), moveType);
                }
            }
            if (y - 1 >= y_lower)
            {
                string moveType = this.getValidType(side, new Point(x, y - 1));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x, y - 1), moveType);
                }
            }
            return moveMap;
        }

        string[,] getValidMoveMapCannons(string side, Point pos)
        {
            string[,] moveMap = this.getEmptyBoard();
            int x = pos.X;
            int y = pos.Y;
            bool canEat = false;
            for (int xi = x; xi < 9; xi++)
            {
                if (xi == x) continue;
                string validType = this.getValidType(side, new Point(xi, y));
                if (!canEat && (validType == "block" || validType == "eat"))
                {
                    canEat = true;
                }
                else if (canEat && validType == "eat")
                {
                    this.setMoveMap(moveMap, new Point(xi, y), "eat");
                    break;
                }
                else if (!canEat && validType == "go")
                {
                    this.setMoveMap(moveMap, new Point(xi, y), "go");
                }
            }
            canEat = false;
            for (int xi = x; xi >= 0; xi--)
            {
                if (xi == x) continue;
                string validType = this.getValidType(side, new Point(xi, y));
                if (!canEat && (validType == "block" || validType == "eat"))
                {
                    canEat = true;
                }
                else if (canEat && validType == "eat")
                {
                    this.setMoveMap(moveMap, new Point(xi, y), "eat");
                    break;
                }
                else if (!canEat && validType == "go")
                {
                    this.setMoveMap(moveMap, new Point(xi, y), "go");
                }
            }
            canEat = false;
            for (int yi = y; yi < 9; yi++)
            {
                if (yi == y) continue;
                string validType = this.getValidType(side, new Point(x, yi));
                if (!canEat && (validType == "block" || validType == "eat"))
                {
                    canEat = true;
                }
                else if (canEat && validType == "eat")
                {
                    this.setMoveMap(moveMap, new Point(x, yi), "eat");
                    break;
                }
                else if (!canEat && validType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x, yi), "go");
                }
            }
            canEat = false;
            for (int yi = y; yi >= 0; yi--)
            {
                if (yi == y) continue;
                string validType = this.getValidType(side, new Point(x, yi));
                if (!canEat && (validType == "block" || validType == "eat"))
                {
                    canEat = true;
                }
                else if (canEat && validType == "eat")
                {
                    this.setMoveMap(moveMap, new Point(x, yi), "eat");
                    break;
                }
                else if (!canEat && validType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x, yi), "go");
                }
            }
            return moveMap;
        }

        string[,] getValidMoveMapPawns(string side, Point pos)
        {
            string[,] moveMap = this.getEmptyBoard();
            int x = pos.X;
            int y = pos.Y;
            string kingSide = this.getSideByKing(pos);
            if (side == kingSide)
            {
                if (y <= 4)
                {
                    string moveType = this.getValidType(side, new Point(x, y + 1));
                    if (moveType == "eat" || moveType == "go")
                    {
                        this.setMoveMap(moveMap, new Point(x, y + 1), moveType);
                    }
                }
                else
                {
                    string moveType = this.getValidType(side, new Point(x, y - 1));
                    if (moveType == "eat" || moveType == "go")
                    {
                        this.setMoveMap(moveMap, new Point(x, y - 1), moveType);
                    }
                }
            }
            else
            {
                string moveType;
                if (y <= 4)
                {
                    moveType = this.getValidType(side, new Point(x, y - 1));
                    if (moveType == "eat" || moveType == "go")
                    {
                        this.setMoveMap(moveMap, new Point(x, y - 1), moveType);
                    }
                }
                else
                {
                    moveType = this.getValidType(side, new Point(x, y + 1));
                    if (moveType == "eat" || moveType == "go")
                    {
                        this.setMoveMap(moveMap, new Point(x, y + 1), moveType);
                    }
                }
                moveType = this.getValidType(side, new Point(x + 1, y));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x + 1, y), moveType);
                }
                moveType = this.getValidType(side, new Point(x - 1, y));
                if (moveType == "eat" || moveType == "go")
                {
                    this.setMoveMap(moveMap, new Point(x - 1, y), moveType);
                }
            }
            return moveMap;
        }


        void makeMove(Point from, Point to)
        {
            this.board[to.Y, to.X] = this.board[from.Y, from.X];
            this.board[from.Y, from.X] = "";
        }


        string getPiece(Point pos)
        {
            if (pos == null) return "";
            if (pos.X < 0 || pos.X > 8 || pos.Y < 0 || pos.Y > 9) return "invalid";
            return this.board[pos.Y, pos.X];
        }


        void setMoveMap(string[,] moveMap, Point pos, string value)
        {
            if (pos.X < 0 || pos.X > 8 || pos.Y < 0 || pos.Y > 9) return;
            moveMap[pos.Y, pos.X] = value;
        }


        string getSideByKing(Point pos)
        {
            int x = pos.X;
            int y = pos.Y;
            if (y <= 4)
            {
                for (int xi = 3; xi <= 5; xi++)
                {
                    for (int yi = 0; yi <= 2; yi++)
                    {
                        string piece = this.getPiece(new Point(xi, yi));
                        if (piece == "K")
                        {
                            return "w";
                        }
                        else if (piece == "k")
                        {
                            return "b";
                        }
                    }
                }
            }
            else
            {
                for (int xi = 3; xi <= 5; xi++)
                {
                    for (int yi = 7; yi <= 9; yi++)
                    {
                        string piece = this.getPiece(new Point(xi, yi));
                        if (piece == "K")
                        {
                            return "w";
                        }
                        else if (piece == "k")
                        {
                            return "b";
                        }
                    }
                }
            }
            return "";
        }


        void initBoard()
        {
            this.fen = "rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w";
            this.fenToBoard(this.fen);
        }


        string boardToFen(string[,] board, string myPos, string nextPlayer = "w")
        {
            var fen = "";
            var emptyCount = 0;
            if (board == null) return "";
            if (myPos == "w")
            {
                for (var y = 0; y < 10; y++)
                {
                    for (var x = 0; x < 9; x++)
                    {
                        if (board[y, x] == "")
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
                            fen += board[y, x];
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
                for (var y = 9; y >= 0; y--)
                {
                    for (var x = 0; x < 9; x++)
                    {
                        if (board[y, x] == "")
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
                            fen += board[y, x];
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


        string[,] getEmptyBoard()
        {
            string[,] board = new string[9, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int x = 0; x < 9; x++)
                {
                    board[x, i] = "";
                }
            }
            return board;
        }

        string[,] fenToBoard(string fen)
        {
            string[] args = fen.Split(' ');
            string[] board = args[0].Split('/');
            string turn = args[1];
            string[,] newBoard = this.getEmptyBoard();
            string chessPieces = "rnbakRNBAKCPcp";
            string numbers = "123456789";
            for (int row_i = 0; row_i < board.Length; row_i++)
            {
                int col = 0;
                string row = board[row_i];
                for (int i = 0; i < row.Length; i++)
                {
                    if (chessPieces.Contains(row[i]))
                    {
                        newBoard[col, row_i] = row[i].ToString();
                        col++;
                    }
                    else if (numbers.Contains(row[i]))
                    {
                        col += int.Parse(row[i].ToString());
                    }
                }
            }
            this.board = newBoard;
            this.turn = turn;
            return newBoard;
        }
    }
}
