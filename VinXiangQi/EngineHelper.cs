using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace VinXiangQi
{
    public class EngineHelper
    {
        // Deal with engines with UCI Protocol for Chess Game.

        public string EnginePath = "";
        public Process Engine;
        public string LastBestMove = "";
        public string LastPonderMove = "";
        public event Action<string, string> BestMoveEvent;
        public event Action<string, Dictionary<string, string>> InfoEvent;
        public event Action EndOfFileEvent;
        public Thread ThreadHandleOutput;
        public Queue<string> OutputQueue = new Queue<string>();
        public Dictionary<string, string> Configs = new Dictionary<string, string>();


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
            if (Engine != null && !Engine.HasExited)
            {
                Engine.Close();
            }
        }

        public void Init()
        {
            Engine = new Process();
            Engine.StartInfo.FileName = EnginePath;
            Engine.StartInfo.UseShellExecute = false;
            Engine.StartInfo.RedirectStandardInput = true;
            Engine.StartInfo.RedirectStandardOutput = true;
            Engine.StartInfo.RedirectStandardError = true;
            Engine.StartInfo.CreateNoWindow = true;
            Engine.OutputDataReceived += Engine_OutputDataReceived;
            Engine.Start();
            ThreadHandleOutput = new Thread(new ThreadStart(HandleOutputLoop));
            ThreadHandleOutput.Start();
            Engine.BeginOutputReadLine();
            Engine.StandardInput.WriteLine("uci");
            foreach (var option in Configs)
            {
                SetOption(option.Key, option.Value);
            }
            Mainform.EngineAnalyzeCount = 0;
        }

        private void Engine_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OutputQueue.Enqueue(e.Data);
            Debug.WriteLine(e.Data);
        }

        public void HandleOutputLoop()
        {
            try
            {
                while (Engine.HasExited == false)
                {
                    if (OutputQueue.Count > 0)
                    {
                        string line = OutputQueue.Dequeue();
                        if (line != null)
                        {
                            string[] args = line.Split(' ');
                            string cmd = args[0];
                            if (cmd == "bestmove")
                            {
                                Thread.Sleep(200);
                                if (OutputQueue.Count == 0)
                                {
                                    HandleOutputLine(line);
                                }
                                else
                                {
                                    Debug.WriteLine("舍弃错误识别的BestMove");
                                }
                            }
                            else
                            {
                                HandleOutputLine(line);
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
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
                if (args.Length > 2)
                {
                    BestMoveEvent?.Invoke(args[1], args[3]);
                }
                else
                {
                    BestMoveEvent?.Invoke(args[1], "");
                }
            }
        }

        public void SetOption(string key, string value)
        {
            Engine.StandardInput.WriteLine("setoption " + key + " " + value);
        }

        public void StartAnalyze(string fen, double time_sec=-1)
        {
            if (Engine.HasExited)
            {
                Init();
            }
            Engine.StandardInput.WriteLine("stop");
            Engine.StandardInput.WriteLine("position fen " + fen);
            if (time_sec != -1)
            {
                Engine.StandardInput.WriteLine("go movetime " + (int)(time_sec * 1000));
            }
            else
            {
                Engine.StandardInput.WriteLine("go infinite");
            }
        }
    }
}
