namespace VinXiangQi
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.pictureBox_about = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_about)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_about
            // 
            this.pictureBox_about.BackColor = System.Drawing.Color.White;
            this.pictureBox_about.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_about.Image = global::VinXiangQi.Properties.Resources.about_vinxiangqi;
            this.pictureBox_about.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_about.Name = "pictureBox_about";
            this.pictureBox_about.Size = new System.Drawing.Size(800, 450);
            this.pictureBox_about.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_about.TabIndex = 0;
            this.pictureBox_about.TabStop = false;
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pictureBox_about);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AboutForm";
            this.Text = "关于 Vin象棋";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_about)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_about;
    }
}