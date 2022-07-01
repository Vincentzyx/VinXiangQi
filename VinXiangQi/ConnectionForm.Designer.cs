namespace VinXiangQi
{
    partial class ConnectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox_board = new System.Windows.Forms.PictureBox();
            this.timer_stop_topmost = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_board)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_board
            // 
            this.pictureBox_board.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_board.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_board.Name = "pictureBox_board";
            this.pictureBox_board.Size = new System.Drawing.Size(496, 489);
            this.pictureBox_board.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_board.TabIndex = 0;
            this.pictureBox_board.TabStop = false;
            this.pictureBox_board.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_board_MouseDown);
            // 
            // timer_stop_topmost
            // 
            this.timer_stop_topmost.Interval = 2000;
            this.timer_stop_topmost.Tick += new System.EventHandler(this.timer_stop_topmost_Tick);
            // 
            // ConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 489);
            this.Controls.Add(this.pictureBox_board);
            this.Name = "ConnectionForm";
            this.Text = "ConnectionForm";
            this.Load += new System.EventHandler(this.ConnectionForm_Load);
            this.Shown += new System.EventHandler(this.ConnectionForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_board)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_board;
        private System.Windows.Forms.Timer timer_stop_topmost;
    }
}