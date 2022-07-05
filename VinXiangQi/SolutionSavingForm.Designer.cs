namespace VinXiangQi
{
    partial class SolutionSavingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SolutionSavingForm));
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_game_title = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_game_class = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_click_title = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_click_class = new System.Windows.Forms.TextBox();
            this.button_save_settings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(104, 30);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(241, 25);
            this.textBox_name.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "方案名:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "截图标题:";
            // 
            // textBox_game_title
            // 
            this.textBox_game_title.Location = new System.Drawing.Point(104, 77);
            this.textBox_game_title.Name = "textBox_game_title";
            this.textBox_game_title.Size = new System.Drawing.Size(241, 25);
            this.textBox_game_title.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "截图类:";
            // 
            // textBox_game_class
            // 
            this.textBox_game_class.Location = new System.Drawing.Point(104, 108);
            this.textBox_game_class.Name = "textBox_game_class";
            this.textBox_game_class.Size = new System.Drawing.Size(241, 25);
            this.textBox_game_class.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "点击标题:";
            // 
            // textBox_click_title
            // 
            this.textBox_click_title.Location = new System.Drawing.Point(104, 139);
            this.textBox_click_title.Name = "textBox_click_title";
            this.textBox_click_title.Size = new System.Drawing.Size(241, 25);
            this.textBox_click_title.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "点击类:";
            // 
            // textBox_click_class
            // 
            this.textBox_click_class.Location = new System.Drawing.Point(104, 170);
            this.textBox_click_class.Name = "textBox_click_class";
            this.textBox_click_class.Size = new System.Drawing.Size(241, 25);
            this.textBox_click_class.TabIndex = 8;
            // 
            // button_save_settings
            // 
            this.button_save_settings.Location = new System.Drawing.Point(140, 216);
            this.button_save_settings.Name = "button_save_settings";
            this.button_save_settings.Size = new System.Drawing.Size(101, 34);
            this.button_save_settings.TabIndex = 10;
            this.button_save_settings.Text = "保存";
            this.button_save_settings.UseVisualStyleBackColor = true;
            this.button_save_settings.Click += new System.EventHandler(this.button_save_settings_Click);
            // 
            // SolutionSavingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 262);
            this.Controls.Add(this.button_save_settings);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_click_class);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_click_title);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_game_class);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_game_title);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_name);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SolutionSavingForm";
            this.Text = "保存方案";
            this.Load += new System.EventHandler(this.SolutionSavingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_game_title;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_game_class;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_click_title;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_click_class;
        private System.Windows.Forms.Button button_save_settings;
    }
}