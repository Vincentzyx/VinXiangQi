using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace VinXiangQi
{
    public partial class SolutionSavingForm : Form
    {
        public string GameTitle;
        public string GameClass;
        public string ClickTitle;
        public string ClickClass;
        public SolutionSavingForm()
        {
            InitializeComponent();
        }

        void InitUI()
        {
            GameTitle = ScreenshotHelper.GetWindowTitle(Mainform.GameHandle);
            GameClass = ScreenshotHelper.GetWindowClass(Mainform.GameHandle);
            if (Mainform.GameHandle != Mainform.ClickHandle)
            {
                ClickTitle = ScreenshotHelper.GetWindowTitle(Mainform.ClickHandle);
                ClickClass = ScreenshotHelper.GetWindowClass(Mainform.ClickHandle);
                textBox_click_title.Text = ClickTitle;
                textBox_click_class.Text = ClickClass;
            }
            textBox_game_title.Text = GameTitle;
            textBox_game_class.Text = GameClass;
            textBox_name.Text = GameTitle;
        }

        private void button_save_settings_Click(object sender, EventArgs e)
        {
            string folder = Mainform.SOLUTION_FOLDER + textBox_name.Text;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            File.WriteAllText(folder + "\\" + "window.txt", 
                $"截图标题={textBox_game_title.Text}\r\n" +
                $"截图类={textBox_game_class.Text}\r\n" +
                $"点击标题={textBox_click_title.Text}\r\n" +
                $"点击类={textBox_click_class.Text}"
                );
        }

        private void SolutionSavingForm_Load(object sender, EventArgs e)
        {
            InitUI();
        }
    }
}
