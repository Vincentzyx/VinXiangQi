using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace VinXiangQi
{
    public class OpenBookHelper
    {
        public static OpenBookUtils OBUtils = new OpenBookUtils();

        public static List<OpenBook.QueryResult> QueryAll(Dictionary<string, OpenBook> openBookList, string fen)
        {
            List<OpenBook.QueryResult> ResultList = new List<OpenBook.QueryResult>();
            foreach (var openbook in openBookList)
            {
                var results = openbook.Value.Query(fen);
                if (results.Count > 0)
                {
                    ResultList.AddRange(results);
                    continue;
                }
                results = openbook.Value.Query(Utils.MirrorFenRedBlack(fen));
                if (results.Count > 0)
                {
                    foreach (var r in results)
                    {
                        var from = Utils.Move2Point(r.Move.Substring(0, 2), true);
                        var to = Utils.Move2Point(r.Move.Substring(2, 2), true);
                        from.Y = 9 - from.Y;
                        to.Y = 9 - to.Y;
                        r.Move = Utils.Point2Move(from, to);
                    }
                    ResultList.AddRange(results);
                    continue;
                }
                results = openbook.Value.Query(Utils.MirrorFenLeftRight(fen));
                if (results.Count > 0)
                {
                    foreach (var r in results)
                    {
                        var from = Utils.Move2Point(r.Move.Substring(0, 2), true);
                        var to = Utils.Move2Point(r.Move.Substring(2, 2), true);
                        from.X = 8 - from.X;
                        to.X = 8 - to.X;
                        r.Move = Utils.Point2Move(from, to);
                    }
                    ResultList.AddRange(results);
                    continue;
                }
                results = openbook.Value.Query(Utils.MirrorFenLeftRight(Utils.MirrorFenRedBlack(fen)));
                if (results.Count > 0)
                {
                    foreach (var r in results)
                    {
                        var from = Utils.Move2Point(r.Move.Substring(0, 2), true);
                        var to = Utils.Move2Point(r.Move.Substring(2, 2), true);
                        from.Y = 9 - from.Y;
                        to.Y = 9 - to.Y;
                        from.X = 8 - from.X;
                        to.X = 8 - to.X;
                        r.Move = Utils.Point2Move(from, to);
                    }
                    ResultList.AddRange(results);
                    continue;
                }
            }
            return ResultList;
        }

        public class OpenBook
        {
            public string FileName;
            public string BookName;
            public SqliteConnection SQLite;

            public OpenBook(string file)
            {
                FileName = file;
                BookName = FileName.Split('\\').Last().Split('.').First();
                SQLite = new SqliteConnection("Data Source=" + file);
                SQLite.Open();
            }

            public class QueryResult
            {
                public string Book;
                public string Move;
                public int Score;
                public int Win;
                public int Draw;
                public int Lose;
                public int Valid;
                public string Memo;
                public int Index;
                public QueryResult(string book, string move, int score, int win, int draw, int lose, int valid, string memo, int index)
                {
                    Book = book;
                    Move = move;
                    Score = score;
                    Win = win;
                    Draw = draw;
                    Lose = lose;
                    Valid = valid;
                    Memo = memo;
                    Index = index;
                }
            }
            
            public List<QueryResult> Query(string fen)
            {
                UInt64 key = OBUtils.GetZobristFromFen(fen);
                string sql = "SELECT id, vkey, vmove, vscore, vwin, vdraw, vlost, vvalid, vmemo, vindex FROM bhobk WHERE vkey like " + key;
                SqliteCommand cmd = new SqliteCommand(sql, SQLite);
                SqliteDataReader reader = cmd.ExecuteReader();
                List<QueryResult> ResultList = new List<QueryResult>();
                while (reader.Read())
                {
                    int vmove = 0, vscore = 0, vwin = 0, vdraw = 0, vlost = 0, vvalid = 0, vindex = 0;
                    int.TryParse(reader["vmove"].ToString(), out vmove);
                    int.TryParse(reader["vscore"].ToString(), out vscore);
                    int.TryParse(reader["vwin"].ToString(), out vwin);
                    int.TryParse(reader["vdraw"].ToString(), out vdraw);
                    int.TryParse(reader["vlost"].ToString(), out vlost);
                    int.TryParse(reader["vvalid"].ToString(), out vvalid);
                    int.TryParse(reader["vindex"].ToString(), out vindex);
                    string vmemo = "";
                    if (reader["vmemo"].GetType() == typeof(byte[]))
                    {
                        vmemo = Encoding.UTF8.GetString((byte[])reader["vmemo"]);
                    }
                    else
                    {
                        vmemo = (string)reader["vmemo"];
                    }
                    string move = OBUtils.ConvertVmoveToCoord(vmove);
                    ResultList.Add(new QueryResult(BookName, move, vscore, vwin, vdraw, vlost, vvalid, vmemo, vindex));
                }
                return ResultList;
            }

            public void Dispose()
            {
                SQLite.Close();
            }
        }
    }
}
