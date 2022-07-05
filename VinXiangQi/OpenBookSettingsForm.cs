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
    public partial class OpenBookSettingsForm : Form
    {
        public OpenBookSettingsForm()
        {
            InitializeComponent();
        }

        private void OpenBookSettingsForm_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> translation = new Dictionary<string, string>{ { "HighestScore", "最高分" }, { "Random", "随机" } };
            foreach (var book in Mainform.OpenBookList)
            {
                listBox_openbook_list.Items.Add(book.Key);
            }
            foreach (var item in comboBox_openbook_mode.Items)
            {
                if (translation[Mainform.Settings.OpenbookMode.ToString()] == item.ToString())
                {
                    comboBox_openbook_mode.SelectedItem = item;
                    break;
                }
            }
            checkBox_use_openbook.Checked = Mainform.Settings.UseOpenBook;
            checkBox_enable_chessdb.Checked = Mainform.Settings.UseChessDB;
        }

        private void checkBox_use_openbook_CheckedChanged(object sender, EventArgs e)
        {
            Mainform.Settings.UseOpenBook = checkBox_use_openbook.Checked;
            Mainform.SaveSettings();
        }

        private void comboBox_openbook_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_openbook_mode.SelectedItem.ToString() == "最高分")
            {
                Mainform.Settings.OpenbookMode = ProgramSettings.OpenBookMode.HighestScore;
            }
            else if (comboBox_openbook_mode.SelectedItem.ToString() == "随机")
            {
                Mainform.Settings.OpenbookMode = ProgramSettings.OpenBookMode.Random;
            }
            Mainform.SaveSettings();
        }

        private void button_open_folder_Click(object sender, EventArgs e)
        {
            // Use Explorer To Open The Folder
            System.Diagnostics.Process.Start("explorer.exe", Environment.CurrentDirectory + @"\OpenBooks");
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            Mainform.InitOpenBooks();
            foreach (var book in Mainform.OpenBookList)
            {
                listBox_openbook_list.Items.Add(book.Key);
            }
        }

        private void checkBox_enable_chessdb_CheckedChanged(object sender, EventArgs e)
        {
            Mainform.Settings.UseChessDB = checkBox_enable_chessdb.Checked;
            Mainform.SaveSettings();
        }
    }
}
