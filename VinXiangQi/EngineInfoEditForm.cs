using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VinXiangQi
{
    public partial class EngineInfoEditForm : Form
    {
        public EngineInfoEditForm(string engineName, string engineAuthor, string enginePath)
        {
            InitializeComponent();
            EngineName = engineName;
            EngineAuthor = engineAuthor;
            EnginePath = enginePath;
        }

        public string EngineName = "";
        public string EngineAuthor = "";
        public string EnginePath = "";
        public bool Saved = false;

        private void EngineInfoEditForm_Load(object sender, EventArgs e)
        {
            textBox_engine_author.Text = EngineAuthor;
            textBox_engine_name.Text = EngineName;
            textBox_engine_path.Text = EnginePath;
        }

        private void button_confirm_Click(object sender, EventArgs e)
        {
            EngineName = textBox_engine_name.Text;
            EngineAuthor = textBox_engine_author.Text;
            EnginePath = textBox_engine_path.Text;
            Saved = true;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
