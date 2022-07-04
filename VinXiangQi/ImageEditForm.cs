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
    public partial class ImageEditForm : Form
    {
        public Bitmap RawImage;
        public Bitmap DisplayBitmap;
        Graphics DisplayGDI;
        public Point TopLeft;
        public Point BottomRight;
        State ActionState = State.Idle;
        public string FilePath = "";
        public string FileName = "";
        Brush TransparentGreen = new SolidBrush(Color.FromArgb(100, 0, 255, 0));

        public enum State
        {
            Idle,
            Selected1,
            Done
        }
        
        public ImageEditForm(Bitmap rawImage, string fileName, string filePath)
        {
            RawImage = rawImage;
            FilePath = filePath;
            FileName = fileName;
            InitializeComponent();
        }

        private void ImageEditForm_Load(object sender, EventArgs e)
        {
            InitImage();
            InitImageList();
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            
            richTextBox_name.Text = FileName;
        }

        void InitImage()
        {
            DisplayBitmap = (Bitmap)RawImage.Clone();
            DisplayGDI = Graphics.FromImage(DisplayBitmap);
            pictureBox.Image = DisplayBitmap;
            ActionState = State.Idle;
            label_status.Text = "请点击裁切区域左上角的点";
        }

        void InitImageList()
        {
            string[] imgExtensions = new string[] { "png", "jpg", "bmp", "jpeg" };
            listView_images.Items.Clear();
            string[] files = Directory.GetFiles(FilePath);
            ImageList imageList = new ImageList();
            listView_images.LargeImageList = imageList;
            foreach (string file in files)
            {
                if (imgExtensions.Contains(file.Split('.').Last()))
                {
                    Bitmap tmpImg = new Bitmap(file);
                    Bitmap img = new Bitmap(tmpImg);
                    tmpImg.Dispose();
                    imageList.Images.Add(img);
                    listView_images.Items.Add(Path.GetFileName(file), imageList.Images.Count - 1);
                }
            }
        }

        void RenderDisplay()
        {
            DisplayGDI.Clear(Color.White);
            DisplayGDI.DrawImage(RawImage, 0, 0);
            if (ActionState == State.Selected1)
            {
                DisplayGDI.FillEllipse(TransparentGreen, TopLeft.X - 5, TopLeft.Y - 5, 10, 10);
                DisplayGDI.FillEllipse(Brushes.Red, TopLeft.X - 2, TopLeft.Y - 2, 4, 4);                
            }
            else if (ActionState == State.Done)
            {
                DisplayGDI.FillRectangle(TransparentGreen, TopLeft.X, TopLeft.Y, BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y);
                DisplayGDI.DrawRectangle(Pens.Red, TopLeft.X, TopLeft.Y, BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y);
            }
            pictureBox.Refresh();
        }

        Point ToImagePosition(MouseEventArgs e)
        {
            bool MouseOnImage = false;
            Point MousePositionOnImage = new Point(-1, -1);
            double img_ratio = (double)DisplayBitmap.Width / (double)DisplayBitmap.Height;
            double box_ratio = (double)pictureBox.Width / (double)pictureBox.Height;
            double display_ratio;
            int display_x, display_y, display_width, display_height;
            if (box_ratio >= img_ratio)
            {
                display_y = 0;
                display_height = pictureBox.Height;
                display_x = (int)(pictureBox.Width - display_height * img_ratio) / 2;
                display_width = (int)(display_height * img_ratio);
                display_ratio = (double)DisplayBitmap.Height / display_height;
            }
            else
            {
                display_x = 0;
                display_width = pictureBox.Width;
                display_y = (int)(pictureBox.Height - display_width / img_ratio) / 2;
                display_height = (int)(display_width / img_ratio);
                display_ratio = (double)DisplayBitmap.Width / display_width;
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
                img_x = DisplayBitmap.Width - 1;
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
                img_y = DisplayBitmap.Height - 1;
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
                return MousePositionOnImage;
            }
            else
            {
                return new Point(-1, -1);
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (ActionState == State.Idle)
            {
                Point MousePositionOnImage = ToImagePosition(e);
                if (MousePositionOnImage.X != -1)
                {
                    ActionState = State.Selected1;
                    TopLeft = MousePositionOnImage;
                    label_status.Text = "请点击裁切区域右下角的点";
                }
            }
            else if (ActionState == State.Selected1)
            {
                Point MousePositionOnImage = ToImagePosition(e);
                if (MousePositionOnImage.X != -1)
                {
                    ActionState = State.Done;
                    BottomRight = MousePositionOnImage;
                    label_status.Text = "区域选择完成";
                }
            }
            else if (ActionState == State.Done)
            {
                ActionState = State.Idle;
                label_status.Text = "请点击裁切区域左上角的点";
            }
            RenderDisplay();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void button_reset_Click(object sender, EventArgs e)
        {
            InitImage();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            try
            {
                string fullPath = FilePath;
                if (!FilePath.EndsWith("\\"))
                {
                    fullPath += "\\";
                }
                fullPath += FileName;
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                Bitmap cropped = RawImage.Clone(new Rectangle(TopLeft, new Size(BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y)), RawImage.PixelFormat);
                cropped.Save(fullPath + ".png");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("在保存图片时发生错误:" + ex.ToString());
            }
        }

        private void richTextBox_name_TextChanged(object sender, EventArgs e)
        {
            FileName = richTextBox_name.Text;
        }

        private void button_delete_image_Click(object sender, EventArgs e)
        {
            if (listView_images.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in listView_images.SelectedItems)
                {
                    string fullPath = FilePath;
                    if (!FilePath.EndsWith("\\"))
                    {
                        fullPath += "\\";
                    }
                    fullPath += item.Text;
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            File.Delete(fullPath);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("在删除图片时发生错误:" + ex.ToString());
                        }
                    }
                    listView_images.Items.Remove(item);
                }
            }
        }
    }
}
