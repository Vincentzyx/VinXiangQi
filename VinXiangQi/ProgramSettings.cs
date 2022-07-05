using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinXiangQi
{
    public class ProgramSettings
    {
        public Dictionary<string, EngineSettings> EngineList = new Dictionary<string, EngineSettings>();
        public string SelectedEngine = "";
        public float ScaleFactor = 1.0f;
        public bool AutoGo = true;
        public double StepTime = 2.0;
        public bool RedSide = true;
        public int ThreadCount = 4;
        public bool KeepDetecting = false;
        public bool UniversalMode = false;
        public bool UniversalMouse = false;
        public string SelectedSolution = "天天象棋";
        public bool AnalyzingMode = false;
        public bool AutoClick = false;
        public bool StopWhenMate = false;
        public string YoloModel = "nano.onnx";
        public bool UseOpenBook = false;
        public bool UseChessDB = false;
        public OpenBookMode OpenbookMode = OpenBookMode.HighestScore;
       
        public enum OpenBookMode
        {
            HighestScore,
            Random
        }

        public EngineSettings CurrentEngine
        {
            get
            {
                if (EngineList.ContainsKey(SelectedEngine))
                {
                    return EngineList[SelectedEngine];
                }
                return null;
            }
        }
    }

    public class EngineSettings
    {
        public string ExePath = "";
        public Dictionary<string, string> Configs = new Dictionary<string, string>();
    }
}
