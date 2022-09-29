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
    public partial class ImageDisplayForm : Form
    {
        Image DisplayBitmap;

        public ImageDisplayForm(Image displayBitmap)
        {
            InitializeComponent();
            DisplayBitmap = displayBitmap;
        }

        private void ImageDisplayForm_Load(object sender, EventArgs e)
        {
            pictureBox_display.Image = DisplayBitmap;
        }

        private void pictureBox_display_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }
    }
}
