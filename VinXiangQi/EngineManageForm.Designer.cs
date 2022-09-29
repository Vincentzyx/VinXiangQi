namespace VinXiangQi
{
    partial class EngineManageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EngineManageForm));
            this.button_add_engine = new System.Windows.Forms.Button();
            this.button_delete_engine = new System.Windows.Forms.Button();
            this.button_engine_settings = new System.Windows.Forms.Button();
            this.openFileDialog_engine = new System.Windows.Forms.OpenFileDialog();
            this.listView_engine_list = new System.Windows.Forms.ListView();
            this.columnHeader_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_author = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // button_add_engine
            // 
            this.button_add_engine.Location = new System.Drawing.Point(12, 9);
            this.button_add_engine.Name = "button_add_engine";
            this.button_add_engine.Size = new System.Drawing.Size(100, 34);
            this.button_add_engine.TabIndex = 1;
            this.button_add_engine.Text = "添加引擎";
            this.button_add_engine.UseVisualStyleBackColor = true;
            this.button_add_engine.Click += new System.EventHandler(this.button_add_engine_Click);
            // 
            // button_delete_engine
            // 
            this.button_delete_engine.Location = new System.Drawing.Point(118, 9);
            this.button_delete_engine.Name = "button_delete_engine";
            this.button_delete_engine.Size = new System.Drawing.Size(100, 34);
            this.button_delete_engine.TabIndex = 2;
            this.button_delete_engine.Text = "删除引擎";
            this.button_delete_engine.UseVisualStyleBackColor = true;
            this.button_delete_engine.Click += new System.EventHandler(this.button_delete_engine_Click);
            // 
            // button_engine_settings
            // 
            this.button_engine_settings.Location = new System.Drawing.Point(224, 9);
            this.button_engine_settings.Name = "button_engine_settings";
            this.button_engine_settings.Size = new System.Drawing.Size(100, 34);
            this.button_engine_settings.TabIndex = 3;
            this.button_engine_settings.Text = "引擎设置";
            this.button_engine_settings.UseVisualStyleBackColor = true;
            this.button_engine_settings.Click += new System.EventHandler(this.button_engine_settings_Click);
            // 
            // openFileDialog_engine
            // 
            this.openFileDialog_engine.FileName = "openFileDialog1";
            // 
            // listView_engine_list
            // 
            this.listView_engine_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_name,
            this.columnHeader_author,
            this.columnHeader_path});
            this.listView_engine_list.FullRowSelect = true;
            this.listView_engine_list.HideSelection = false;
            this.listView_engine_list.Location = new System.Drawing.Point(12, 52);
            this.listView_engine_list.Name = "listView_engine_list";
            this.listView_engine_list.Size = new System.Drawing.Size(675, 315);
            this.listView_engine_list.TabIndex = 5;
            this.listView_engine_list.UseCompatibleStateImageBehavior = false;
            this.listView_engine_list.View = System.Windows.Forms.View.Details;
            this.listView_engine_list.DoubleClick += new System.EventHandler(this.listView_engine_list_DoubleClick);
            // 
            // columnHeader_name
            // 
            this.columnHeader_name.Text = "名称";
            this.columnHeader_name.Width = 150;
            // 
            // columnHeader_author
            // 
            this.columnHeader_author.Text = "作者";
            this.columnHeader_author.Width = 140;
            // 
            // columnHeader_path
            // 
            this.columnHeader_path.Text = "路径";
            this.columnHeader_path.Width = 320;
            // 
            // EngineManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 379);
            this.Controls.Add(this.button_add_engine);
            this.Controls.Add(this.listView_engine_list);
            this.Controls.Add(this.button_engine_settings);
            this.Controls.Add(this.button_delete_engine);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EngineManageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "引擎管理";
            this.Load += new System.EventHandler(this.EngineManageForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_add_engine;
        private System.Windows.Forms.Button button_delete_engine;
        private System.Windows.Forms.Button button_engine_settings;
        private System.Windows.Forms.OpenFileDialog openFileDialog_engine;
        private System.Windows.Forms.ListView listView_engine_list;
        private System.Windows.Forms.ColumnHeader columnHeader_name;
        private System.Windows.Forms.ColumnHeader columnHeader_author;
        private System.Windows.Forms.ColumnHeader columnHeader_path;
    }
}