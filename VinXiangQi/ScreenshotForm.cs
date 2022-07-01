using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VinXiangQi
{
    public partial class ScreenshotForm : Form
    {
        public ScreenshotForm()
        {
            InitializeComponent();
        }

        public bool IsShown = false;

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private const int VM_NCLBUTTONDOWN = 0XA1;//定义鼠标左键按下
        private const int HTCAPTION = 2;
        public DateTime LastPlayed = DateTime.Now.AddSeconds(-10);


        private void ScreenshotForm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            //this.TopMost = true; // make the form always on top
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // hidden border
            //this.WindowState = FormWindowState.Maximized; // maximized
            //this.MinimizeBox = this.MaximizeBox = false; // not allowed to be minimized
            //this.MinimumSize = this.MaximumSize = this.Size; // not allowed to be resized
        }

        public Bitmap Screenshot()
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(this.Left, this.Top, 0, 0, this.Size);
            return bmp;
        }

        private void ScreenshotForm_Shown(object sender, EventArgs e)
        {
            IsShown = true;
        }

        private void ScreenshotForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsShown = false;
        }
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        // Set the form click-through
        //        cp.ExStyle |= 0x80000 /* WS_EX_LAYERED */ | 0x20 /* WS_EX_TRANSPARENT */;
        //        return cp;
        //    }
        //}

        
        private void ScreenshotForm_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage((IntPtr)this.Handle, VM_NCLBUTTONDOWN, HTCAPTION, 0);
        }
    }
}
