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
    public partial class DetectionSettingsForm : Form
    {
        public DetectionSettingsForm()
        {
            InitializeComponent();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numericUpDown_detection_confirm_ValueChanged(object sender, EventArgs e)
        {
            Mainform.Settings.DetectionConfirmCount = (int)numericUpDown_detection_confirm.Value;
            Mainform.SaveSettings();
        }

        private void numericUpDown_detection_interval_ValueChanged(object sender, EventArgs e)
        {
            Mainform.Settings.DetectionInterval = (int)numericUpDown_detection_interval.Value;
            Mainform.SaveSettings();
        }

        private void DetectionSettingsForm_Load(object sender, EventArgs e)
        {
            numericUpDown_detection_confirm.Value = (decimal)Mainform.Settings.DetectionConfirmCount;
            numericUpDown_detection_interval.Value = (decimal)Mainform.Settings.DetectionInterval;
        }
    }
}
