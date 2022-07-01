using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yolov5Net.Scorer;

namespace VinXiangQi
{
    public partial class ConnectionForm : Form
    {
        public ConnectionForm()
        {
            InitializeComponent();
        }

        Bitmap ResultDisplayBitmap = new Bitmap(400,440);
        Graphics DisplayGDI;

        public void SetState(YoloPrediction[,] board)
        {
            int width = 40;
            int height = 40;
            int xoffset = width / 2;
            int yoffset = height / 2;
            DisplayGDI.Clear(Color.White);
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    DisplayGDI.DrawRectangle(Pens.LightGray, x * width + xoffset, y * height + yoffset, width, height);
                    if (board[x, y] != null)
                    {
                        DisplayGDI.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject(board[x, y].Label.Name), x * width + xoffset, y * height + yoffset, width, height);
                    }
                }
            }
            pictureBox_board.Invoke(new Action(() => pictureBox_board.Refresh()));
        }

        private void ConnectionForm_Load(object sender, EventArgs e)
        {
            pictureBox_board.Image = ResultDisplayBitmap;
            DisplayGDI = Graphics.FromImage(ResultDisplayBitmap);
            SetState(new YoloPrediction[9, 10]);
        }

        private void pictureBox_board_MouseDown(object sender, MouseEventArgs e)
        {
            bool MouseOnImage = false;
            Point MousePositionOnImage = new Point(-1, -1);
            double img_ratio = (double)ResultDisplayBitmap.Width / (double)ResultDisplayBitmap.Height;
            double box_ratio = (double)pictureBox_board.Width / (double)pictureBox_board.Height;
            double display_ratio;
            int display_x, display_y, display_width, display_height;
            if (box_ratio >= img_ratio)
            {
                display_y = 0;
                display_height = pictureBox_board.Height;
                display_x = (int)(pictureBox_board.Width - display_height * img_ratio) / 2;
                display_width = (int)(display_height * img_ratio);
                display_ratio = (double)ResultDisplayBitmap.Height / display_height;
            }
            else
            {
                display_x = 0;
                display_width = pictureBox_board.Width;
                display_y = (int)(pictureBox_board.Height - display_width / img_ratio) / 2;
                display_height = (int)(display_width / img_ratio);
                display_ratio = (double)ResultDisplayBitmap.Width / display_width;
            }
            int img_x, img_y;
            bool mouse_on_x, mouse_on_y;
            if (e.X <= display_x)
            {
                img_x = 0;
                mouse_on_x = false;
            }
            else if (e.X >= display_x + display_width)
            {
                img_x = ResultDisplayBitmap.Width - 1;
                mouse_on_x = false;
            }
            else
            {
                img_x = (int)((e.X - display_x) * display_ratio);
                mouse_on_x = true;
            }
            if (e.Y <= display_y)
            {
                img_y = 0;
                mouse_on_y = false;
            }
            else if (e.Y >= display_y + display_height)
            {
                img_y = ResultDisplayBitmap.Height - 1;
                mouse_on_y = false;
            }
            else
            {
                img_y = (int)((e.Y - display_y) * display_ratio);
                mouse_on_y = true;
            }
            if (mouse_on_x && mouse_on_y)
            {
                MouseOnImage = true;
                MousePositionOnImage = new Point(img_x, img_y);
            }
            else
            {
                MouseOnImage = false;
            }
            if (MouseOnImage)
            {
                int width = 40;
                int height = 40;
                int xoffset = width / 2;
                int yoffset = height / 2;
                Point pos = new Point(-1, -1);
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        if (MousePositionOnImage.X >= x * width + xoffset && MousePositionOnImage.X <= x * width + xoffset + width &&
                            MousePositionOnImage.Y >= y * height + yoffset && MousePositionOnImage.Y <= y * height + yoffset + height)
                        {
                            pos = new Point(x, y);
                            break;
                        }
                    }
                    if (pos.X != -1) break;
                }
                if (pos.X != -1)
                {
                    Point p = Mainform.PositionMap[pos.X, pos.Y];
                    MouseHelper.MouseLeftClick(Mainform.GameHandle, p.X, p.Y);
                    this.Text = "Click On: " + pos.ToString() + " → " + p.ToString();
                }
            }
        }

        private void ConnectionForm_Shown(object sender, EventArgs e)
        {
            this.TopMost = true;
            timer_stop_topmost.Enabled = true;
        }

        private void timer_stop_topmost_Tick(object sender, EventArgs e)
        {
            this.TopMost = false;
            timer_stop_topmost.Enabled = false;
        }
    }
}
