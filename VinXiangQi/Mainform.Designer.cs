namespace VinXiangQi
{
    partial class Mainform
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            this.timer_set_window_hwnd = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel_main = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_detection = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox_picturebox = new System.Windows.Forms.GroupBox();
            this.pictureBox_show_result = new System.Windows.Forms.PictureBox();
            this.groupBox_result_board = new System.Windows.Forms.GroupBox();
            this.pictureBox_board = new System.Windows.Forms.PictureBox();
            this.groupBox_engine_settings = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel_engine = new System.Windows.Forms.TableLayoutPanel();
            this.checkBox_auto_scroll = new System.Windows.Forms.CheckBox();
            this.groupBox_engine_basic = new System.Windows.Forms.GroupBox();
            this.button_useless_btn = new System.Windows.Forms.Button();
            this.button_openbook_settings = new System.Windows.Forms.Button();
            this.button_go_immediately = new System.Windows.Forms.Button();
            this.button_engine_settings = new System.Windows.Forms.Button();
            this.button_engine_delete = new System.Windows.Forms.Button();
            this.button_engine_add = new System.Windows.Forms.Button();
            this.label_thread_count = new System.Windows.Forms.Label();
            this.numericUpDown_thread_count = new System.Windows.Forms.NumericUpDown();
            this.label_engine = new System.Windows.Forms.Label();
            this.label_step_time = new System.Windows.Forms.Label();
            this.numericUpDown_step_time = new System.Windows.Forms.NumericUpDown();
            this.comboBox_engine = new System.Windows.Forms.ComboBox();
            this.groupBox_side = new System.Windows.Forms.GroupBox();
            this.button_save_as_solution = new System.Windows.Forms.Button();
            this.checkBox_stop_when_mate = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_go = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_click = new System.Windows.Forms.CheckBox();
            this.button_screenshot = new System.Windows.Forms.Button();
            this.checkBox_universal_mouse = new System.Windows.Forms.CheckBox();
            this.comboBox_yolo_models = new System.Windows.Forms.ComboBox();
            this.checkBox_analyze_mode = new System.Windows.Forms.CheckBox();
            this.comboBox_solution = new System.Windows.Forms.ComboBox();
            this.checkBox_debug = new System.Windows.Forms.CheckBox();
            this.checkBox_universal_mode = new System.Windows.Forms.CheckBox();
            this.checkBox_keep_detect = new System.Windows.Forms.CheckBox();
            this.button_get_hwnd = new System.Windows.Forms.Button();
            this.numericUpDown_scale_factor = new System.Windows.Forms.NumericUpDown();
            this.button_redetect = new System.Windows.Forms.Button();
            this.textBox_engine_log = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.byVincentzyxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_copy_fen = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_about = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.label_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_pad = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_info = new System.Windows.Forms.ToolStripStatusLabel();
            this.label_detect_mode = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog_engine = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label_detection_status = new System.Windows.Forms.Label();
            this.button_start_from_self = new System.Windows.Forms.Button();
            this.button_start_from_oppo = new System.Windows.Forms.Button();
            this.button_stop_detection = new System.Windows.Forms.Button();
            this.checkBox_background_analysis = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel_main.SuspendLayout();
            this.tableLayoutPanel_detection.SuspendLayout();
            this.groupBox_picturebox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_show_result)).BeginInit();
            this.groupBox_result_board.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_board)).BeginInit();
            this.groupBox_engine_settings.SuspendLayout();
            this.tableLayoutPanel_engine.SuspendLayout();
            this.groupBox_engine_basic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_thread_count)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_step_time)).BeginInit();
            this.groupBox_side.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_scale_factor)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer_set_window_hwnd
            // 
            this.timer_set_window_hwnd.Enabled = true;
            this.timer_set_window_hwnd.Interval = 200;
            this.timer_set_window_hwnd.Tick += new System.EventHandler(this.timer_set_window_hwnd_Tick);
            // 
            // tableLayoutPanel_main
            // 
            this.tableLayoutPanel_main.ColumnCount = 2;
            this.tableLayoutPanel_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 800F));
            this.tableLayoutPanel_main.Controls.Add(this.tableLayoutPanel_detection, 0, 0);
            this.tableLayoutPanel_main.Controls.Add(this.groupBox_engine_settings, 1, 0);
            this.tableLayoutPanel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_main.Location = new System.Drawing.Point(0, 30);
            this.tableLayoutPanel_main.Name = "tableLayoutPanel_main";
            this.tableLayoutPanel_main.RowCount = 2;
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel_main.Size = new System.Drawing.Size(1161, 720);
            this.tableLayoutPanel_main.TabIndex = 2;
            // 
            // tableLayoutPanel_detection
            // 
            this.tableLayoutPanel_detection.ColumnCount = 1;
            this.tableLayoutPanel_detection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_detection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_detection.Controls.Add(this.groupBox_picturebox, 0, 0);
            this.tableLayoutPanel_detection.Controls.Add(this.groupBox_result_board, 0, 1);
            this.tableLayoutPanel_detection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_detection.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel_detection.Name = "tableLayoutPanel_detection";
            this.tableLayoutPanel_detection.RowCount = 2;
            this.tableLayoutPanel_detection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_detection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_detection.Size = new System.Drawing.Size(355, 688);
            this.tableLayoutPanel_detection.TabIndex = 9;
            // 
            // groupBox_picturebox
            // 
            this.groupBox_picturebox.Controls.Add(this.pictureBox_show_result);
            this.groupBox_picturebox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_picturebox.Location = new System.Drawing.Point(3, 3);
            this.groupBox_picturebox.Name = "groupBox_picturebox";
            this.groupBox_picturebox.Size = new System.Drawing.Size(349, 338);
            this.groupBox_picturebox.TabIndex = 6;
            this.groupBox_picturebox.TabStop = false;
            this.groupBox_picturebox.Text = "识别画面";
            // 
            // pictureBox_show_result
            // 
            this.pictureBox_show_result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_show_result.Location = new System.Drawing.Point(3, 21);
            this.pictureBox_show_result.Name = "pictureBox_show_result";
            this.pictureBox_show_result.Size = new System.Drawing.Size(343, 314);
            this.pictureBox_show_result.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_show_result.TabIndex = 5;
            this.pictureBox_show_result.TabStop = false;
            // 
            // groupBox_result_board
            // 
            this.groupBox_result_board.Controls.Add(this.pictureBox_board);
            this.groupBox_result_board.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_result_board.Location = new System.Drawing.Point(3, 347);
            this.groupBox_result_board.Name = "groupBox_result_board";
            this.groupBox_result_board.Size = new System.Drawing.Size(349, 338);
            this.groupBox_result_board.TabIndex = 8;
            this.groupBox_result_board.TabStop = false;
            this.groupBox_result_board.Text = "识别结果";
            // 
            // pictureBox_board
            // 
            this.pictureBox_board.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_board.Location = new System.Drawing.Point(3, 21);
            this.pictureBox_board.Name = "pictureBox_board";
            this.pictureBox_board.Size = new System.Drawing.Size(343, 314);
            this.pictureBox_board.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_board.TabIndex = 8;
            this.pictureBox_board.TabStop = false;
            this.pictureBox_board.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_board_MouseDown);
            // 
            // groupBox_engine_settings
            // 
            this.groupBox_engine_settings.Controls.Add(this.tableLayoutPanel_engine);
            this.groupBox_engine_settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_engine_settings.Location = new System.Drawing.Point(364, 3);
            this.groupBox_engine_settings.Name = "groupBox_engine_settings";
            this.groupBox_engine_settings.Size = new System.Drawing.Size(794, 688);
            this.groupBox_engine_settings.TabIndex = 9;
            this.groupBox_engine_settings.TabStop = false;
            this.groupBox_engine_settings.Text = "基本设置";
            // 
            // tableLayoutPanel_engine
            // 
            this.tableLayoutPanel_engine.ColumnCount = 1;
            this.tableLayoutPanel_engine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_engine.Controls.Add(this.checkBox_auto_scroll, 0, 2);
            this.tableLayoutPanel_engine.Controls.Add(this.groupBox_engine_basic, 0, 0);
            this.tableLayoutPanel_engine.Controls.Add(this.groupBox_side, 0, 1);
            this.tableLayoutPanel_engine.Controls.Add(this.textBox_engine_log, 0, 3);
            this.tableLayoutPanel_engine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_engine.Location = new System.Drawing.Point(3, 21);
            this.tableLayoutPanel_engine.Name = "tableLayoutPanel_engine";
            this.tableLayoutPanel_engine.RowCount = 4;
            this.tableLayoutPanel_engine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel_engine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tableLayoutPanel_engine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel_engine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_engine.Size = new System.Drawing.Size(788, 664);
            this.tableLayoutPanel_engine.TabIndex = 0;
            // 
            // checkBox_auto_scroll
            // 
            this.checkBox_auto_scroll.AutoSize = true;
            this.checkBox_auto_scroll.Checked = true;
            this.checkBox_auto_scroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_scroll.Location = new System.Drawing.Point(3, 373);
            this.checkBox_auto_scroll.Name = "checkBox_auto_scroll";
            this.checkBox_auto_scroll.Size = new System.Drawing.Size(89, 19);
            this.checkBox_auto_scroll.TabIndex = 29;
            this.checkBox_auto_scroll.Text = "自动滚动";
            this.checkBox_auto_scroll.UseVisualStyleBackColor = true;
            // 
            // groupBox_engine_basic
            // 
            this.groupBox_engine_basic.Controls.Add(this.checkBox_background_analysis);
            this.groupBox_engine_basic.Controls.Add(this.button_useless_btn);
            this.groupBox_engine_basic.Controls.Add(this.button_openbook_settings);
            this.groupBox_engine_basic.Controls.Add(this.button_go_immediately);
            this.groupBox_engine_basic.Controls.Add(this.button_engine_settings);
            this.groupBox_engine_basic.Controls.Add(this.button_engine_delete);
            this.groupBox_engine_basic.Controls.Add(this.button_engine_add);
            this.groupBox_engine_basic.Controls.Add(this.label_thread_count);
            this.groupBox_engine_basic.Controls.Add(this.numericUpDown_thread_count);
            this.groupBox_engine_basic.Controls.Add(this.label_engine);
            this.groupBox_engine_basic.Controls.Add(this.label_step_time);
            this.groupBox_engine_basic.Controls.Add(this.numericUpDown_step_time);
            this.groupBox_engine_basic.Controls.Add(this.comboBox_engine);
            this.groupBox_engine_basic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_engine_basic.Location = new System.Drawing.Point(3, 3);
            this.groupBox_engine_basic.Name = "groupBox_engine_basic";
            this.groupBox_engine_basic.Size = new System.Drawing.Size(782, 134);
            this.groupBox_engine_basic.TabIndex = 0;
            this.groupBox_engine_basic.TabStop = false;
            this.groupBox_engine_basic.Text = "引擎设置";
            // 
            // button_useless_btn
            // 
            this.button_useless_btn.Location = new System.Drawing.Point(415, 89);
            this.button_useless_btn.Name = "button_useless_btn";
            this.button_useless_btn.Size = new System.Drawing.Size(84, 28);
            this.button_useless_btn.TabIndex = 29;
            this.button_useless_btn.Text = "保存设置";
            this.button_useless_btn.UseVisualStyleBackColor = true;
            // 
            // button_openbook_settings
            // 
            this.button_openbook_settings.Location = new System.Drawing.Point(291, 89);
            this.button_openbook_settings.Name = "button_openbook_settings";
            this.button_openbook_settings.Size = new System.Drawing.Size(118, 28);
            this.button_openbook_settings.TabIndex = 28;
            this.button_openbook_settings.Text = "开局库设置";
            this.button_openbook_settings.UseVisualStyleBackColor = true;
            this.button_openbook_settings.Click += new System.EventHandler(this.button_openbook_settings_Click);
            // 
            // button_go_immediately
            // 
            this.button_go_immediately.Location = new System.Drawing.Point(681, 89);
            this.button_go_immediately.Name = "button_go_immediately";
            this.button_go_immediately.Size = new System.Drawing.Size(84, 28);
            this.button_go_immediately.TabIndex = 14;
            this.button_go_immediately.Text = "立即出招";
            this.button_go_immediately.UseVisualStyleBackColor = true;
            this.button_go_immediately.Click += new System.EventHandler(this.button_go_immediately_Click);
            // 
            // button_engine_settings
            // 
            this.button_engine_settings.Location = new System.Drawing.Point(201, 89);
            this.button_engine_settings.Name = "button_engine_settings";
            this.button_engine_settings.Size = new System.Drawing.Size(84, 28);
            this.button_engine_settings.TabIndex = 13;
            this.button_engine_settings.Text = "引擎设置";
            this.button_engine_settings.UseVisualStyleBackColor = true;
            this.button_engine_settings.Click += new System.EventHandler(this.button_engine_settings_Click);
            // 
            // button_engine_delete
            // 
            this.button_engine_delete.Location = new System.Drawing.Point(111, 89);
            this.button_engine_delete.Name = "button_engine_delete";
            this.button_engine_delete.Size = new System.Drawing.Size(84, 28);
            this.button_engine_delete.TabIndex = 12;
            this.button_engine_delete.Text = "删除引擎";
            this.button_engine_delete.UseVisualStyleBackColor = true;
            this.button_engine_delete.Click += new System.EventHandler(this.button_engine_delete_Click);
            // 
            // button_engine_add
            // 
            this.button_engine_add.Location = new System.Drawing.Point(21, 89);
            this.button_engine_add.Name = "button_engine_add";
            this.button_engine_add.Size = new System.Drawing.Size(84, 28);
            this.button_engine_add.TabIndex = 11;
            this.button_engine_add.Text = "添加引擎";
            this.button_engine_add.UseVisualStyleBackColor = true;
            this.button_engine_add.Click += new System.EventHandler(this.button_engine_add_Click);
            // 
            // label_thread_count
            // 
            this.label_thread_count.AutoSize = true;
            this.label_thread_count.Location = new System.Drawing.Point(169, 55);
            this.label_thread_count.Name = "label_thread_count";
            this.label_thread_count.Size = new System.Drawing.Size(60, 15);
            this.label_thread_count.TabIndex = 9;
            this.label_thread_count.Text = "线程数:";
            // 
            // numericUpDown_thread_count
            // 
            this.numericUpDown_thread_count.Location = new System.Drawing.Point(235, 53);
            this.numericUpDown_thread_count.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.numericUpDown_thread_count.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_thread_count.Name = "numericUpDown_thread_count";
            this.numericUpDown_thread_count.Size = new System.Drawing.Size(70, 25);
            this.numericUpDown_thread_count.TabIndex = 8;
            this.numericUpDown_thread_count.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDown_thread_count.ValueChanged += new System.EventHandler(this.numericUpDown_thread_count_ValueChanged);
            // 
            // label_engine
            // 
            this.label_engine.AutoSize = true;
            this.label_engine.Location = new System.Drawing.Point(18, 27);
            this.label_engine.Name = "label_engine";
            this.label_engine.Size = new System.Drawing.Size(45, 15);
            this.label_engine.TabIndex = 4;
            this.label_engine.Text = "引擎:";
            // 
            // label_step_time
            // 
            this.label_step_time.AutoSize = true;
            this.label_step_time.Location = new System.Drawing.Point(18, 55);
            this.label_step_time.Name = "label_step_time";
            this.label_step_time.Size = new System.Drawing.Size(45, 15);
            this.label_step_time.TabIndex = 3;
            this.label_step_time.Text = "步时:";
            // 
            // numericUpDown_step_time
            // 
            this.numericUpDown_step_time.DecimalPlaces = 1;
            this.numericUpDown_step_time.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown_step_time.Location = new System.Drawing.Point(69, 53);
            this.numericUpDown_step_time.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown_step_time.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDown_step_time.Name = "numericUpDown_step_time";
            this.numericUpDown_step_time.Size = new System.Drawing.Size(79, 25);
            this.numericUpDown_step_time.TabIndex = 2;
            this.numericUpDown_step_time.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_step_time.ValueChanged += new System.EventHandler(this.numericUpDown_step_time_ValueChanged);
            // 
            // comboBox_engine
            // 
            this.comboBox_engine.FormattingEnabled = true;
            this.comboBox_engine.Location = new System.Drawing.Point(69, 24);
            this.comboBox_engine.Name = "comboBox_engine";
            this.comboBox_engine.Size = new System.Drawing.Size(457, 23);
            this.comboBox_engine.TabIndex = 1;
            this.comboBox_engine.SelectedIndexChanged += new System.EventHandler(this.comboBox_engine_SelectedIndexChanged);
            // 
            // groupBox_side
            // 
            this.groupBox_side.Controls.Add(this.groupBox5);
            this.groupBox_side.Controls.Add(this.groupBox4);
            this.groupBox_side.Controls.Add(this.groupBox3);
            this.groupBox_side.Controls.Add(this.groupBox2);
            this.groupBox_side.Controls.Add(this.groupBox1);
            this.groupBox_side.Controls.Add(this.checkBox_keep_detect);
            this.groupBox_side.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_side.Location = new System.Drawing.Point(3, 143);
            this.groupBox_side.Name = "groupBox_side";
            this.groupBox_side.Size = new System.Drawing.Size(782, 224);
            this.groupBox_side.TabIndex = 6;
            this.groupBox_side.TabStop = false;
            this.groupBox_side.Text = "识别设置";
            // 
            // button_save_as_solution
            // 
            this.button_save_as_solution.Location = new System.Drawing.Point(16, 56);
            this.button_save_as_solution.Name = "button_save_as_solution";
            this.button_save_as_solution.Size = new System.Drawing.Size(157, 28);
            this.button_save_as_solution.TabIndex = 28;
            this.button_save_as_solution.Text = "保存当前方案";
            this.button_save_as_solution.UseVisualStyleBackColor = true;
            this.button_save_as_solution.Click += new System.EventHandler(this.button_save_as_solution_Click);
            // 
            // checkBox_stop_when_mate
            // 
            this.checkBox_stop_when_mate.AutoSize = true;
            this.checkBox_stop_when_mate.Location = new System.Drawing.Point(15, 110);
            this.checkBox_stop_when_mate.Name = "checkBox_stop_when_mate";
            this.checkBox_stop_when_mate.Size = new System.Drawing.Size(119, 19);
            this.checkBox_stop_when_mate.TabIndex = 27;
            this.checkBox_stop_when_mate.Text = "绝杀立即出招";
            this.checkBox_stop_when_mate.UseVisualStyleBackColor = true;
            this.checkBox_stop_when_mate.CheckedChanged += new System.EventHandler(this.checkBox_stop_when_mate_CheckedChanged);
            // 
            // checkBox_auto_go
            // 
            this.checkBox_auto_go.AutoSize = true;
            this.checkBox_auto_go.Checked = true;
            this.checkBox_auto_go.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_go.Location = new System.Drawing.Point(110, 35);
            this.checkBox_auto_go.Name = "checkBox_auto_go";
            this.checkBox_auto_go.Size = new System.Drawing.Size(89, 19);
            this.checkBox_auto_go.TabIndex = 7;
            this.checkBox_auto_go.Text = "自动走棋";
            this.checkBox_auto_go.UseVisualStyleBackColor = true;
            this.checkBox_auto_go.CheckedChanged += new System.EventHandler(this.checkBox_auto_go_CheckedChanged);
            // 
            // checkBox_auto_click
            // 
            this.checkBox_auto_click.AutoSize = true;
            this.checkBox_auto_click.Location = new System.Drawing.Point(110, 85);
            this.checkBox_auto_click.Name = "checkBox_auto_click";
            this.checkBox_auto_click.Size = new System.Drawing.Size(89, 19);
            this.checkBox_auto_click.TabIndex = 26;
            this.checkBox_auto_click.Text = "自动点击";
            this.checkBox_auto_click.UseVisualStyleBackColor = true;
            this.checkBox_auto_click.CheckedChanged += new System.EventHandler(this.checkBox_auto_click_CheckedChanged);
            // 
            // button_screenshot
            // 
            this.button_screenshot.Location = new System.Drawing.Point(16, 123);
            this.button_screenshot.Name = "button_screenshot";
            this.button_screenshot.Size = new System.Drawing.Size(157, 28);
            this.button_screenshot.TabIndex = 25;
            this.button_screenshot.Text = "自动点击管理";
            this.button_screenshot.UseVisualStyleBackColor = true;
            this.button_screenshot.Click += new System.EventHandler(this.button_screenshot_Click);
            // 
            // checkBox_universal_mouse
            // 
            this.checkBox_universal_mouse.AccessibleDescription = "";
            this.checkBox_universal_mouse.AutoSize = true;
            this.checkBox_universal_mouse.Location = new System.Drawing.Point(15, 85);
            this.checkBox_universal_mouse.Name = "checkBox_universal_mouse";
            this.checkBox_universal_mouse.Size = new System.Drawing.Size(89, 19);
            this.checkBox_universal_mouse.TabIndex = 24;
            this.checkBox_universal_mouse.Tag = "";
            this.checkBox_universal_mouse.Text = "前台鼠标";
            this.checkBox_universal_mouse.UseVisualStyleBackColor = true;
            this.checkBox_universal_mouse.CheckedChanged += new System.EventHandler(this.checkBox_universal_mouse_CheckedChanged);
            // 
            // comboBox_yolo_models
            // 
            this.comboBox_yolo_models.FormattingEnabled = true;
            this.comboBox_yolo_models.Location = new System.Drawing.Point(13, 27);
            this.comboBox_yolo_models.Name = "comboBox_yolo_models";
            this.comboBox_yolo_models.Size = new System.Drawing.Size(118, 23);
            this.comboBox_yolo_models.TabIndex = 22;
            this.comboBox_yolo_models.SelectedIndexChanged += new System.EventHandler(this.comboBox_yolo_models_SelectedIndexChanged);
            // 
            // checkBox_analyze_mode
            // 
            this.checkBox_analyze_mode.AutoSize = true;
            this.checkBox_analyze_mode.Location = new System.Drawing.Point(110, 60);
            this.checkBox_analyze_mode.Name = "checkBox_analyze_mode";
            this.checkBox_analyze_mode.Size = new System.Drawing.Size(89, 19);
            this.checkBox_analyze_mode.TabIndex = 21;
            this.checkBox_analyze_mode.Text = "分析模式";
            this.checkBox_analyze_mode.UseVisualStyleBackColor = true;
            this.checkBox_analyze_mode.CheckedChanged += new System.EventHandler(this.checkBox_analyze_mode_CheckedChanged);
            // 
            // comboBox_solution
            // 
            this.comboBox_solution.FormattingEnabled = true;
            this.comboBox_solution.Location = new System.Drawing.Point(16, 27);
            this.comboBox_solution.Name = "comboBox_solution";
            this.comboBox_solution.Size = new System.Drawing.Size(157, 23);
            this.comboBox_solution.TabIndex = 14;
            this.comboBox_solution.SelectedIndexChanged += new System.EventHandler(this.comboBox_solution_SelectedIndexChanged);
            // 
            // checkBox_debug
            // 
            this.checkBox_debug.AccessibleDescription = "";
            this.checkBox_debug.AutoSize = true;
            this.checkBox_debug.Location = new System.Drawing.Point(15, 35);
            this.checkBox_debug.Name = "checkBox_debug";
            this.checkBox_debug.Size = new System.Drawing.Size(89, 19);
            this.checkBox_debug.TabIndex = 18;
            this.checkBox_debug.Tag = "";
            this.checkBox_debug.Text = "调试状态";
            this.checkBox_debug.UseVisualStyleBackColor = true;
            this.checkBox_debug.CheckedChanged += new System.EventHandler(this.checkBox_debug_CheckedChanged);
            // 
            // checkBox_universal_mode
            // 
            this.checkBox_universal_mode.AccessibleDescription = "";
            this.checkBox_universal_mode.AutoSize = true;
            this.checkBox_universal_mode.Location = new System.Drawing.Point(15, 60);
            this.checkBox_universal_mode.Name = "checkBox_universal_mode";
            this.checkBox_universal_mode.Size = new System.Drawing.Size(89, 19);
            this.checkBox_universal_mode.TabIndex = 16;
            this.checkBox_universal_mode.Tag = "";
            this.checkBox_universal_mode.Text = "前台截图";
            this.checkBox_universal_mode.UseVisualStyleBackColor = true;
            this.checkBox_universal_mode.CheckedChanged += new System.EventHandler(this.checkBox_universal_mode_CheckedChanged);
            // 
            // checkBox_keep_detect
            // 
            this.checkBox_keep_detect.AutoSize = true;
            this.checkBox_keep_detect.Location = new System.Drawing.Point(696, 0);
            this.checkBox_keep_detect.Name = "checkBox_keep_detect";
            this.checkBox_keep_detect.Size = new System.Drawing.Size(89, 19);
            this.checkBox_keep_detect.TabIndex = 15;
            this.checkBox_keep_detect.Text = "持续识别";
            this.checkBox_keep_detect.UseVisualStyleBackColor = true;
            this.checkBox_keep_detect.Visible = false;
            this.checkBox_keep_detect.CheckedChanged += new System.EventHandler(this.checkBox_keep_detect_CheckedChanged);
            // 
            // button_get_hwnd
            // 
            this.button_get_hwnd.Location = new System.Drawing.Point(16, 89);
            this.button_get_hwnd.Name = "button_get_hwnd";
            this.button_get_hwnd.Size = new System.Drawing.Size(157, 28);
            this.button_get_hwnd.TabIndex = 9;
            this.button_get_hwnd.Text = "寻找窗口句柄 (F2)";
            this.button_get_hwnd.UseVisualStyleBackColor = true;
            this.button_get_hwnd.Click += new System.EventHandler(this.button_get_hwnd_Click);
            // 
            // numericUpDown_scale_factor
            // 
            this.numericUpDown_scale_factor.DecimalPlaces = 2;
            this.numericUpDown_scale_factor.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numericUpDown_scale_factor.Location = new System.Drawing.Point(12, 24);
            this.numericUpDown_scale_factor.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown_scale_factor.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numericUpDown_scale_factor.Name = "numericUpDown_scale_factor";
            this.numericUpDown_scale_factor.Size = new System.Drawing.Size(119, 25);
            this.numericUpDown_scale_factor.TabIndex = 8;
            this.numericUpDown_scale_factor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_scale_factor.ValueChanged += new System.EventHandler(this.numericUpDown_scale_factor_ValueChanged);
            // 
            // button_redetect
            // 
            this.button_redetect.Location = new System.Drawing.Point(28, 147);
            this.button_redetect.Name = "button_redetect";
            this.button_redetect.Size = new System.Drawing.Size(157, 28);
            this.button_redetect.TabIndex = 7;
            this.button_redetect.Text = "重新检测棋盘";
            this.button_redetect.UseVisualStyleBackColor = true;
            this.button_redetect.Click += new System.EventHandler(this.button_redetect_Click);
            // 
            // textBox_engine_log
            // 
            this.textBox_engine_log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_engine_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_engine_log.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_engine_log.Location = new System.Drawing.Point(3, 398);
            this.textBox_engine_log.Name = "textBox_engine_log";
            this.textBox_engine_log.Size = new System.Drawing.Size(782, 263);
            this.textBox_engine_log.TabIndex = 7;
            this.textBox_engine_log.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.byVincentzyxToolStripMenuItem,
            this.ToolStripMenuItem_copy_fen,
            this.ToolStripMenuItem_about});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1161, 30);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // byVincentzyxToolStripMenuItem
            // 
            this.byVincentzyxToolStripMenuItem.Name = "byVincentzyxToolStripMenuItem";
            this.byVincentzyxToolStripMenuItem.Size = new System.Drawing.Size(14, 26);
            // 
            // ToolStripMenuItem_copy_fen
            // 
            this.ToolStripMenuItem_copy_fen.Name = "ToolStripMenuItem_copy_fen";
            this.ToolStripMenuItem_copy_fen.Size = new System.Drawing.Size(111, 26);
            this.ToolStripMenuItem_copy_fen.Text = "复制局面FEN";
            this.ToolStripMenuItem_copy_fen.Click += new System.EventHandler(this.ToolStripMenuItem_copy_fen_Click);
            // 
            // ToolStripMenuItem_about
            // 
            this.ToolStripMenuItem_about.Name = "ToolStripMenuItem_about";
            this.ToolStripMenuItem_about.Size = new System.Drawing.Size(109, 26);
            this.ToolStripMenuItem_about.Text = "关于VIN象棋";
            this.ToolStripMenuItem_about.Click += new System.EventHandler(this.ToolStripMenuItem_about_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.label_status,
            this.toolStripStatusLabel_pad,
            this.toolStripStatusLabel_info,
            this.label_detect_mode});
            this.statusStrip.Location = new System.Drawing.Point(0, 724);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1161, 26);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // label_status
            // 
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(77, 20);
            this.label_status.Text = "识别状态: ";
            // 
            // toolStripStatusLabel_pad
            // 
            this.toolStripStatusLabel_pad.Name = "toolStripStatusLabel_pad";
            this.toolStripStatusLabel_pad.Size = new System.Drawing.Size(572, 20);
            this.toolStripStatusLabel_pad.Spring = true;
            this.toolStripStatusLabel_pad.Text = " ";
            // 
            // toolStripStatusLabel_info
            // 
            this.toolStripStatusLabel_info.Name = "toolStripStatusLabel_info";
            this.toolStripStatusLabel_info.Size = new System.Drawing.Size(424, 20);
            this.toolStripStatusLabel_info.Text = "象棋连线工具 By Vincentzyx 交流群: 755655813                     ";
            // 
            // label_detect_mode
            // 
            this.label_detect_mode.Name = "label_detect_mode";
            this.label_detect_mode.Size = new System.Drawing.Size(73, 20);
            this.label_detect_mode.Text = "识别模式:";
            // 
            // openFileDialog_engine
            // 
            this.openFileDialog_engine.FileName = "Engine";
            this.openFileDialog_engine.Filter = "可执行文件|*.exe";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDown_scale_factor);
            this.groupBox1.Location = new System.Drawing.Point(448, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(138, 66);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "缩放比";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox_yolo_models);
            this.groupBox2.Location = new System.Drawing.Point(448, 24);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(138, 66);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "识别模型";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBox_solution);
            this.groupBox3.Controls.Add(this.button_save_as_solution);
            this.groupBox3.Controls.Add(this.button_get_hwnd);
            this.groupBox3.Controls.Add(this.button_screenshot);
            this.groupBox3.Location = new System.Drawing.Point(592, 24);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(187, 168);
            this.groupBox3.TabIndex = 31;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "方案管理";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBox_auto_go);
            this.groupBox4.Controls.Add(this.checkBox_universal_mode);
            this.groupBox4.Controls.Add(this.checkBox_debug);
            this.groupBox4.Controls.Add(this.checkBox_stop_when_mate);
            this.groupBox4.Controls.Add(this.button_redetect);
            this.groupBox4.Controls.Add(this.checkBox_analyze_mode);
            this.groupBox4.Controls.Add(this.checkBox_universal_mouse);
            this.groupBox4.Controls.Add(this.checkBox_auto_click);
            this.groupBox4.Location = new System.Drawing.Point(6, 24);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(212, 194);
            this.groupBox4.TabIndex = 32;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "连线设置";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button_stop_detection);
            this.groupBox5.Controls.Add(this.button_start_from_oppo);
            this.groupBox5.Controls.Add(this.button_start_from_self);
            this.groupBox5.Controls.Add(this.label_detection_status);
            this.groupBox5.Location = new System.Drawing.Point(224, 24);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(139, 194);
            this.groupBox5.TabIndex = 34;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "连线操作";
            // 
            // label_detection_status
            // 
            this.label_detection_status.AutoSize = true;
            this.label_detection_status.Location = new System.Drawing.Point(20, 35);
            this.label_detection_status.Name = "label_detection_status";
            this.label_detection_status.Size = new System.Drawing.Size(83, 15);
            this.label_detection_status.TabIndex = 0;
            this.label_detection_status.Text = "状态: 空闲";
            // 
            // button_start_from_self
            // 
            this.button_start_from_self.Location = new System.Drawing.Point(23, 67);
            this.button_start_from_self.Name = "button_start_from_self";
            this.button_start_from_self.Size = new System.Drawing.Size(98, 28);
            this.button_start_from_self.TabIndex = 1;
            this.button_start_from_self.Text = "我方开始";
            this.button_start_from_self.UseVisualStyleBackColor = true;
            this.button_start_from_self.Click += new System.EventHandler(this.button_start_from_self_Click);
            // 
            // button_start_from_oppo
            // 
            this.button_start_from_oppo.Location = new System.Drawing.Point(23, 101);
            this.button_start_from_oppo.Name = "button_start_from_oppo";
            this.button_start_from_oppo.Size = new System.Drawing.Size(98, 28);
            this.button_start_from_oppo.TabIndex = 2;
            this.button_start_from_oppo.Text = "对方开始";
            this.button_start_from_oppo.UseVisualStyleBackColor = true;
            this.button_start_from_oppo.Click += new System.EventHandler(this.button_start_from_oppo_Click);
            // 
            // button_stop_detection
            // 
            this.button_stop_detection.Location = new System.Drawing.Point(23, 135);
            this.button_stop_detection.Name = "button_stop_detection";
            this.button_stop_detection.Size = new System.Drawing.Size(98, 28);
            this.button_stop_detection.TabIndex = 3;
            this.button_stop_detection.Text = "停止连线";
            this.button_stop_detection.UseVisualStyleBackColor = true;
            this.button_stop_detection.Click += new System.EventHandler(this.button_stop_detection_Click);
            // 
            // checkBox_background_analysis
            // 
            this.checkBox_background_analysis.AutoSize = true;
            this.checkBox_background_analysis.Location = new System.Drawing.Point(320, 55);
            this.checkBox_background_analysis.Name = "checkBox_background_analysis";
            this.checkBox_background_analysis.Size = new System.Drawing.Size(89, 19);
            this.checkBox_background_analysis.TabIndex = 30;
            this.checkBox_background_analysis.Text = "后台思考";
            this.checkBox_background_analysis.UseVisualStyleBackColor = true;
            this.checkBox_background_analysis.CheckedChanged += new System.EventHandler(this.checkBox_background_analysis_CheckedChanged);
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1161, 750);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.tableLayoutPanel_main);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Mainform";
            this.Text = "VIN 象棋 1.2.4";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mainform_FormClosing);
            this.Load += new System.EventHandler(this.Mainform_Load);
            this.tableLayoutPanel_main.ResumeLayout(false);
            this.tableLayoutPanel_detection.ResumeLayout(false);
            this.groupBox_picturebox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_show_result)).EndInit();
            this.groupBox_result_board.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_board)).EndInit();
            this.groupBox_engine_settings.ResumeLayout(false);
            this.tableLayoutPanel_engine.ResumeLayout(false);
            this.tableLayoutPanel_engine.PerformLayout();
            this.groupBox_engine_basic.ResumeLayout(false);
            this.groupBox_engine_basic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_thread_count)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_step_time)).EndInit();
            this.groupBox_side.ResumeLayout(false);
            this.groupBox_side.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_scale_factor)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer_set_window_hwnd;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_main;
        private System.Windows.Forms.GroupBox groupBox_picturebox;
        private System.Windows.Forms.PictureBox pictureBox_show_result;
        private System.Windows.Forms.GroupBox groupBox_result_board;
        private System.Windows.Forms.PictureBox pictureBox_board;
        private System.Windows.Forms.GroupBox groupBox_engine_settings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_engine;
        private System.Windows.Forms.GroupBox groupBox_engine_basic;
        private System.Windows.Forms.Label label_engine;
        private System.Windows.Forms.Label label_step_time;
        private System.Windows.Forms.NumericUpDown numericUpDown_step_time;
        private System.Windows.Forms.ComboBox comboBox_engine;
        private System.Windows.Forms.CheckBox checkBox_auto_go;
        private System.Windows.Forms.GroupBox groupBox_side;
        private System.Windows.Forms.Button button_redetect;
        private System.Windows.Forms.Label label_thread_count;
        private System.Windows.Forms.NumericUpDown numericUpDown_thread_count;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_detection;
        private System.Windows.Forms.Button button_engine_delete;
        private System.Windows.Forms.Button button_engine_add;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button button_engine_settings;
        private System.Windows.Forms.NumericUpDown numericUpDown_scale_factor;
        private System.Windows.Forms.ToolStripMenuItem byVincentzyxToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel label_status;
        private System.Windows.Forms.OpenFileDialog openFileDialog_engine;
        private System.Windows.Forms.Button button_get_hwnd;
        private System.Windows.Forms.CheckBox checkBox_keep_detect;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_pad;
        private System.Windows.Forms.ToolStripStatusLabel label_detect_mode;
        private System.Windows.Forms.CheckBox checkBox_universal_mode;
        private System.Windows.Forms.CheckBox checkBox_debug;
        private System.Windows.Forms.ComboBox comboBox_solution;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_copy_fen;
        private System.Windows.Forms.CheckBox checkBox_analyze_mode;
        private System.Windows.Forms.ComboBox comboBox_yolo_models;
        private System.Windows.Forms.CheckBox checkBox_universal_mouse;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_info;
        private System.Windows.Forms.Button button_screenshot;
        private System.Windows.Forms.CheckBox checkBox_auto_click;
        private System.Windows.Forms.CheckBox checkBox_stop_when_mate;
        private System.Windows.Forms.Button button_go_immediately;
        private System.Windows.Forms.Button button_openbook_settings;
        private System.Windows.Forms.Button button_save_as_solution;
        private System.Windows.Forms.Button button_useless_btn;
        private System.Windows.Forms.RichTextBox textBox_engine_log;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_about;
        private System.Windows.Forms.CheckBox checkBox_auto_scroll;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button button_start_from_self;
        private System.Windows.Forms.Label label_detection_status;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_stop_detection;
        private System.Windows.Forms.Button button_start_from_oppo;
        private System.Windows.Forms.CheckBox checkBox_background_analysis;
    }
}

