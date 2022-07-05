using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace VinXiangQi
{
    public class ChessDBHelper
    {
        public static string API_ROOT = "http://www.chessdb.cn/chessdb.php?action=querypv&board=";

        public class PVResult
        {
            public int Score = -1;
            public int Depth = -1;
            public List<string> PV = new List<string>();

            public PVResult(string text)
            {
                string[] args = text.Split(',');
                foreach (var arg in args)
                {
                    string[] kvp = arg.Split(':');
                    string key = kvp[0];
                    string value = kvp[1];
                    if (key == "score")
                    {
                        Score = int.Parse(value);
                    }
                    else if (key == "depth")
                    {
                        Depth = int.Parse(value);
                    }
                    else if (key == "pv")
                    {
                        string[] pvs = value.Split('|');
                        PV.AddRange(pvs);
                    }
                }
            }
        }

        public static string HTTPGet(string url, int timeout = 3000)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.ContentType = "text/html;charset=UTF-8";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
            webRequest.Timeout = timeout;
            webRequest.KeepAlive = false;
            webRequest.AllowAutoRedirect = true;
            WebResponse webResponse = webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            webResponse.Close();
            return result;
        }

        public static PVResult GetPV(string fen)
        {
            string url = API_ROOT + fen;
            string result = HTTPGet(url);
            if (result.Contains("score") && result.Contains("depth") && result.Contains("pv"))
            {
                return new PVResult(result);
            }
            return null;
        }
    }
}
