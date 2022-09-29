namespace VinXiangQi
{
    partial class ImageDisplayForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageDisplayForm));
            this.pictureBox_display = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_display)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_display
            // 
            this.pictureBox_display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_display.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_display.Name = "pictureBox_display";
            this.pictureBox_display.Size = new System.Drawing.Size(586, 556);
            this.pictureBox_display.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_display.TabIndex = 0;
            this.pictureBox_display.TabStop = false;
            this.pictureBox_display.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_display_MouseDoubleClick);
            // 
            // ImageDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 556);
            this.Controls.Add(this.pictureBox_display);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImageDisplayForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "显示图片";
            this.Load += new System.EventHandler(this.ImageDisplayForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_display)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_display;
    }
}