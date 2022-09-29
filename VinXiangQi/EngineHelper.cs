using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace VinXiangQi
{
    public class EngineHelper
    {
        // Deal with engines with UCCI Protocol for Chess Game.
        public string EngineType = "uci";
        public string EnginePath = "";
        public string EngineName = "";
        public string EngineAuthor = "";
        public Process Engine;
        public string LastBestMove = "";
        public string LastPonderMove = "";
        public string BanMoves = "";
        public event Action<string, string, string> BestMoveEvent;
        public event Action<string, Dictionary<string, string>> InfoEvent;
        public List<string> OptionList = new List<string>();
        public Thread ThreadHandleOutput;
        public Dictionary<string, string> Configs = new Dictionary<string, string>();
        public Queue<string> AnalyzeQueue = new Queue<string>();
        public List<string> SkipList = new List<string>();
        public DateTime LastOutputTime = new DateTime();
        public string IgnoreMove = "";
        public bool Initialized = false;


        public EngineHelper(string enginePath, Dictionary<string, string> configs = null)
        {
            EnginePath = enginePath;
            if (configs != null)
            {
                Configs = configs;
            }
        }

        public void Stop()
        {
            try
            {
                if (Engine != null)
                {
                    Engine.Kill();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void Init()
        {
            Initialized = false;
            Debug.WriteLine("Init Engine");
            Engine = new Process();
            EnginePath = EnginePath.Replace("/", "\\");
            if (!File.Exists(EnginePath))
            {
                MessageBox.Show("找不到引擎 " + EnginePath + " !");
                return;
            }
            Engine.StartInfo.FileName = EnginePath;
            Engine.StartInfo.UseShellExecute = false;
            Engine.StartInfo.RedirectStandardInput = true;
            Engine.StartInfo.RedirectStandardOutput = true;
            Engine.StartInfo.RedirectStandardError = true;
            Engine.StartInfo.CreateNoWindow = true;
            string[] pathParams = EnginePath.Split('\\');
            Engine.StartInfo.WorkingDirectory = string.Join("\\", pathParams.Take(pathParams.Length - 1));
            Engine.OutputDataReceived += Engine_OutputDataReceived;
            Engine.Start();
            int tryCount = 10;
            bool success = false;
            while (!success && tryCount > 0)
            {
                tryCount--;
                try
                {
                    Engine.PriorityClass = ProcessPriorityClass.BelowNormal;
                    success = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("加载引擎时失败: " + ex.ToString());
                    Thread.Sleep(500);
                }
            }
            ThreadHandleOutput = new Thread(new ThreadStart(WaitForExit));
            ThreadHandleOutput.Start();
            Engine.BeginOutputReadLine();
            OptionList.Clear();
            SendCommand("uci");
            SendCommand("ucci");
            tryCount = 100;
            while (tryCount > 0)
            {
                if (OptionList.Count > 0)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(300);
                }
            }
            if (OptionList.Count < 0)
            {
                throw new Exception("引擎 " + EnginePath + " 加载超时！");
            }
            foreach (var option in Configs)
            {
                SetOption(option.Key, option.Value);
            }
            AnalyzeQueue.Clear();
            Initialized = true;
        }

        void WaitForExit()
        {
            Engine.WaitForExit();
        }

        private void Engine_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine("Engine Output: " + e.Data);
            LastOutputTime = DateTime.Now;
            HandleOutputLine(e.Data);
        }

        public void HandleOutputLine(string line)
        {
            if (line == null) return;
            string[] args = line.Split(' ');
            string cmd = args[0];
            string[] info_types = "depth seldepth time nodes pv multipv score currmove currmovenumber hashfull nps tbhits cpuload string refutation currline".Split(' ');
            if (cmd == "info")
            {
                Dictionary<string, string> Infos = new Dictionary<string, string>();
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i] == "pv")
                    {
                        Infos.Add(args[i], string.Join(" ", args.Skip(i + 1)));
                        break;
                    }
                    else if (args[i] == "string")
                    {
                        Infos.Add(args[i], string.Join(" ", args.Skip(i + 1)));
                        break;
                    }
                    else if (args[i] == "score")
                    {
                        if (args[i+1] == "cp")
                        {
                            Infos.Add(args[i], args[i + 2]);
                            i += 2;
                        }
                        else if (args[i+1] == "mate")
                        {
                            Infos.Add(args[i], "绝杀 (" + args[i + 2] + ")");
                            i += 2;
                        }
                        else
                        {
                            Infos.Add(args[i], args[i+1]);
                            i++;
                        }
                    }
                    else
                    {
                        if (args.Length > i + 1 && !info_types.Contains(args[i + 1]))
                        {
                            Infos.Add(args[i], args[i + 1]);
                            i++;
                        }
                        else
                        {
                            Infos.Add(args[i], "");
                        }
                    }
                }
                InfoEvent?.Invoke(cmd, Infos);
            }
            else if (cmd == "bestmove")
            {
                string sourceFen = "";
                if (AnalyzeQueue.Count > 0)
                {
                     sourceFen = AnalyzeQueue.Dequeue();
                }
                if (SkipList.Contains(sourceFen))
                {
                    SkipList.Remove(sourceFen);
                    return;
                }
                if (args.Length > 2)
                {
                    BestMoveEvent?.Invoke(sourceFen, args[1], args[3]);
                }
                else
                {
                    BestMoveEvent?.Invoke(sourceFen, args[1], "");
                }
            }
            else if (cmd == "option")
            {
                OptionList.Add(line);
            }
            else if (cmd == "id")
            {
                if (args.Length >= 3)
                {
                    string type = args[1].ToLower();
                    if (type == "name")
                    {
                        EngineName = string.Join(" ", args.Skip(2));
                    }
                    else if (type == "author")
                    {
                        EngineAuthor = string.Join(" ", args.Skip(2));
                    }
                }
            }
            else if (cmd == "ucciok")
            {
                EngineType = "ucci";
            }
        }

        public void SetOption(string key, string value)
        {
            if (EngineType == "ucci")
            {
                SendCommand("setoption " + key + " " + value);
            }
            else
            {
                SendCommand("setoption name " + key + " value " + value);
            }
        }

        public void StopAnalyze()
        {
            SendCommand("stop");
        }

        public async void StopAnalyzeAndSkip(string skipFen)
        {
            await Task.Run(new Action(() =>
            {
                SendCommand("stop");
                SkipList.Add(skipFen);
            }));
        }

        public void PonderMiss(string skipFen)
        {
            SendCommand("stop");
            SkipList.Add(skipFen);
        }

        public void PonderHit()
        {
            Debug.WriteLine("Ponder hit");
            SendCommand("ponderhit");
        }

        public void SendCommand(string cmd)
        {
            Debug.WriteLine("Engine Input: " + cmd);
            if (Engine.StandardInput != null)
            {
                Engine.StandardInput.WriteLine(cmd);
            }
            else
            {
                Debug.WriteLine("Stardard Input is null");
            }
        }

        public void StartAnalyzePonder(string fen, double time_sec, int depth)
        {
            if (Engine.HasExited)
            {
                Init();
            }
            AnalyzeQueue.Enqueue(fen);
            Debug.WriteLine("Start Analyzing Ponder: \n" + fen);
            SendCommand("position fen " + fen);
            if (BanMoves != "")
            {
                SendCommand("banmoves " + BanMoves);
                BanMoves = "";
            }
            if (time_sec > 0)
            {
                SendCommand("go ponder movetime " + (int)(time_sec * 1000) + " depth " + depth);
            }
            else
            {
                SendCommand("go ponder " + "depth " + depth);
            }
        }

        public void StartAnalyze(string fen, double time_sec, int depth)
        {
            if (Engine == null || Engine.Handle == IntPtr.Zero || Engine.HasExited)
            {
                Init();
            }
            AnalyzeQueue.Enqueue(fen);
            Debug.WriteLine("Start Analyzing: \n" + fen);
            SendCommand("position fen " + fen);
            if (BanMoves != "")
            {
                SendCommand("banmoves " + BanMoves);
                BanMoves = "";
            }
            if (time_sec > 0)
            {
                SendCommand("go movetime " + (int)(time_sec * 1000) + " depth " + depth);
            }
            else
            {
                SendCommand("go " + "depth " + depth);
            }
        }
    }
}
