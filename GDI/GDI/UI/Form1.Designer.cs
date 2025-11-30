namespace GDI
{
    partial class Form1
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_PrintOpen = new System.Windows.Forms.RadioButton();
            this.btn_noPrintf = new System.Windows.Forms.Button();
            this.radioButton_PrintClose = new System.Windows.Forms.RadioButton();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnArmInit = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tbx_Height = new System.Windows.Forms.TextBox();
            this.radioButton_Z = new System.Windows.Forms.RadioButton();
            this.radioButton_N = new System.Windows.Forms.RadioButton();
            this.btn_start = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.label_speed = new System.Windows.Forms.Label();
            this.trackBar_speed = new System.Windows.Forms.TrackBar();
            this.label15 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.radioButton_H_Z = new System.Windows.Forms.RadioButton();
            this.radioButton_H_N = new System.Windows.Forms.RadioButton();
            this.textBox_H_count = new System.Windows.Forms.TextBox();
            this.textBox_H_wid = new System.Windows.Forms.TextBox();
            this.textBox_H_len = new System.Windows.Forms.TextBox();
            this.textBox_H_UVheight = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.btn_H_start = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label_H_speed = new System.Windows.Forms.Label();
            this.trackBar_H_speed = new System.Windows.Forms.TrackBar();
            this.label21 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_camSTOP = new System.Windows.Forms.Button();
            this.btn_camSTART = new System.Windows.Forms.Button();
            this.btn_laserStop = new System.Windows.Forms.Button();
            this.btn_laser = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label_t = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label_c = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label_v = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label_Distance = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label_test = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.rtb8080Log = new System.Windows.Forms.RichTextBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.textInput = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rtb9837Log = new System.Windows.Forms.RichTextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btn9837Send = new System.Windows.Forms.Button();
            this.btn8080Send = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label27 = new System.Windows.Forms.Label();
            this.comboBox_ip = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_speed)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_H_speed)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_PrintOpen);
            this.groupBox2.Controls.Add(this.btn_noPrintf);
            this.groupBox2.Controls.Add(this.radioButton_PrintClose);
            this.groupBox2.Controls.Add(this.btnStop);
            this.groupBox2.Controls.Add(this.btnArmInit);
            this.groupBox2.Controls.Add(this.tabControl2);
            this.groupBox2.Location = new System.Drawing.Point(527, 454);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(470, 454);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "操作初始化设置/调试";
            // 
            // radioButton_PrintOpen
            // 
            this.radioButton_PrintOpen.AutoSize = true;
            this.radioButton_PrintOpen.Checked = true;
            this.radioButton_PrintOpen.Location = new System.Drawing.Point(345, 18);
            this.radioButton_PrintOpen.Name = "radioButton_PrintOpen";
            this.radioButton_PrintOpen.Size = new System.Drawing.Size(47, 16);
            this.radioButton_PrintOpen.TabIndex = 16;
            this.radioButton_PrintOpen.TabStop = true;
            this.radioButton_PrintOpen.Text = "开启";
            this.radioButton_PrintOpen.UseVisualStyleBackColor = true;
            // 
            // btn_noPrintf
            // 
            this.btn_noPrintf.Location = new System.Drawing.Point(337, 39);
            this.btn_noPrintf.Name = "btn_noPrintf";
            this.btn_noPrintf.Size = new System.Drawing.Size(121, 48);
            this.btn_noPrintf.TabIndex = 5;
            this.btn_noPrintf.Text = "喷印系统是否开启";
            this.btn_noPrintf.UseVisualStyleBackColor = true;
            this.btn_noPrintf.Click += new System.EventHandler(this.btn_noPrintf_Click);
            // 
            // radioButton_PrintClose
            // 
            this.radioButton_PrintClose.AutoSize = true;
            this.radioButton_PrintClose.Location = new System.Drawing.Point(408, 18);
            this.radioButton_PrintClose.Name = "radioButton_PrintClose";
            this.radioButton_PrintClose.Size = new System.Drawing.Size(47, 16);
            this.radioButton_PrintClose.TabIndex = 15;
            this.radioButton_PrintClose.Text = "关闭";
            this.radioButton_PrintClose.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(182, 39);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(121, 48);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "机械臂急停";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnArmInit
            // 
            this.btnArmInit.Location = new System.Drawing.Point(18, 38);
            this.btnArmInit.Name = "btnArmInit";
            this.btnArmInit.Size = new System.Drawing.Size(127, 49);
            this.btnArmInit.TabIndex = 4;
            this.btnArmInit.Text = "连接机械臂";
            this.btnArmInit.UseVisualStyleBackColor = true;
            this.btnArmInit.Click += new System.EventHandler(this.btnArmInit_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Location = new System.Drawing.Point(15, 94);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(442, 352);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tbx_Height);
            this.tabPage3.Controls.Add(this.radioButton_Z);
            this.tabPage3.Controls.Add(this.radioButton_N);
            this.tabPage3.Controls.Add(this.btn_start);
            this.tabPage3.Controls.Add(this.label17);
            this.tabPage3.Controls.Add(this.label_speed);
            this.tabPage3.Controls.Add(this.trackBar_speed);
            this.tabPage3.Controls.Add(this.label15);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage3.Size = new System.Drawing.Size(434, 326);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "模版";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tbx_Height
            // 
            this.tbx_Height.Location = new System.Drawing.Point(59, 151);
            this.tbx_Height.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbx_Height.Name = "tbx_Height";
            this.tbx_Height.Size = new System.Drawing.Size(113, 21);
            this.tbx_Height.TabIndex = 17;
            // 
            // radioButton_Z
            // 
            this.radioButton_Z.AutoSize = true;
            this.radioButton_Z.Checked = true;
            this.radioButton_Z.Location = new System.Drawing.Point(55, 271);
            this.radioButton_Z.Name = "radioButton_Z";
            this.radioButton_Z.Size = new System.Drawing.Size(53, 16);
            this.radioButton_Z.TabIndex = 16;
            this.radioButton_Z.TabStop = true;
            this.radioButton_Z.Text = "Z走线";
            this.radioButton_Z.UseVisualStyleBackColor = true;
            // 
            // radioButton_N
            // 
            this.radioButton_N.AutoSize = true;
            this.radioButton_N.Location = new System.Drawing.Point(118, 271);
            this.radioButton_N.Name = "radioButton_N";
            this.radioButton_N.Size = new System.Drawing.Size(53, 16);
            this.radioButton_N.TabIndex = 15;
            this.radioButton_N.Text = "N走线";
            this.radioButton_N.UseVisualStyleBackColor = true;
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(228, 228);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(138, 59);
            this.btn_start.TabIndex = 13;
            this.btn_start.Text = "启动机械臂";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(10, 161);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(0, 19);
            this.label17.TabIndex = 9;
            // 
            // label_speed
            // 
            this.label_speed.AutoSize = true;
            this.label_speed.Font = new System.Drawing.Font("宋体", 12F);
            this.label_speed.Location = new System.Drawing.Point(208, 48);
            this.label_speed.Name = "label_speed";
            this.label_speed.Size = new System.Drawing.Size(23, 16);
            this.label_speed.TabIndex = 8;
            this.label_speed.Text = "20";
            // 
            // trackBar_speed
            // 
            this.trackBar_speed.Location = new System.Drawing.Point(55, 67);
            this.trackBar_speed.Maximum = 100;
            this.trackBar_speed.Name = "trackBar_speed";
            this.trackBar_speed.Size = new System.Drawing.Size(310, 45);
            this.trackBar_speed.TabIndex = 7;
            this.trackBar_speed.Value = 20;
            this.trackBar_speed.Scroll += new System.EventHandler(this.trackBar_speed_Scroll);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(51, 45);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(142, 19);
            this.label15.TabIndex = 12;
            this.label15.Text = "机械臂移动速度";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.radioButton_H_Z);
            this.tabPage4.Controls.Add(this.radioButton_H_N);
            this.tabPage4.Controls.Add(this.textBox_H_count);
            this.tabPage4.Controls.Add(this.textBox_H_wid);
            this.tabPage4.Controls.Add(this.textBox_H_len);
            this.tabPage4.Controls.Add(this.textBox_H_UVheight);
            this.tabPage4.Controls.Add(this.label26);
            this.tabPage4.Controls.Add(this.btn_H_start);
            this.tabPage4.Controls.Add(this.label25);
            this.tabPage4.Controls.Add(this.label23);
            this.tabPage4.Controls.Add(this.label24);
            this.tabPage4.Controls.Add(this.label19);
            this.tabPage4.Controls.Add(this.label20);
            this.tabPage4.Controls.Add(this.label_H_speed);
            this.tabPage4.Controls.Add(this.trackBar_H_speed);
            this.tabPage4.Controls.Add(this.label21);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage4.Size = new System.Drawing.Size(434, 326);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "高级";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // radioButton_H_Z
            // 
            this.radioButton_H_Z.AutoSize = true;
            this.radioButton_H_Z.Checked = true;
            this.radioButton_H_Z.Location = new System.Drawing.Point(35, 293);
            this.radioButton_H_Z.Name = "radioButton_H_Z";
            this.radioButton_H_Z.Size = new System.Drawing.Size(53, 16);
            this.radioButton_H_Z.TabIndex = 25;
            this.radioButton_H_Z.TabStop = true;
            this.radioButton_H_Z.Text = "Z走线";
            this.radioButton_H_Z.UseVisualStyleBackColor = true;
            // 
            // radioButton_H_N
            // 
            this.radioButton_H_N.AutoSize = true;
            this.radioButton_H_N.Location = new System.Drawing.Point(110, 293);
            this.radioButton_H_N.Name = "radioButton_H_N";
            this.radioButton_H_N.Size = new System.Drawing.Size(53, 16);
            this.radioButton_H_N.TabIndex = 24;
            this.radioButton_H_N.Text = "N走线";
            this.radioButton_H_N.UseVisualStyleBackColor = true;
            // 
            // textBox_H_count
            // 
            this.textBox_H_count.Location = new System.Drawing.Point(231, 236);
            this.textBox_H_count.Name = "textBox_H_count";
            this.textBox_H_count.Size = new System.Drawing.Size(56, 21);
            this.textBox_H_count.TabIndex = 23;
            // 
            // textBox_H_wid
            // 
            this.textBox_H_wid.Location = new System.Drawing.Point(134, 236);
            this.textBox_H_wid.Name = "textBox_H_wid";
            this.textBox_H_wid.Size = new System.Drawing.Size(56, 21);
            this.textBox_H_wid.TabIndex = 23;
            // 
            // textBox_H_len
            // 
            this.textBox_H_len.Location = new System.Drawing.Point(35, 236);
            this.textBox_H_len.Name = "textBox_H_len";
            this.textBox_H_len.Size = new System.Drawing.Size(56, 21);
            this.textBox_H_len.TabIndex = 23;
            // 
            // textBox_H_UVheight
            // 
            this.textBox_H_UVheight.Location = new System.Drawing.Point(35, 148);
            this.textBox_H_UVheight.Name = "textBox_H_UVheight";
            this.textBox_H_UVheight.Size = new System.Drawing.Size(56, 21);
            this.textBox_H_UVheight.TabIndex = 23;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("宋体", 9F);
            this.label26.Location = new System.Drawing.Point(294, 245);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(17, 12);
            this.label26.TabIndex = 20;
            this.label26.Text = "次";
            // 
            // btn_H_start
            // 
            this.btn_H_start.Location = new System.Drawing.Point(281, 270);
            this.btn_H_start.Name = "btn_H_start";
            this.btn_H_start.Size = new System.Drawing.Size(131, 39);
            this.btn_H_start.TabIndex = 22;
            this.btn_H_start.Text = "启动机械臂";
            this.btn_H_start.UseVisualStyleBackColor = true;
            this.btn_H_start.Click += new System.EventHandler(this.btn_H_start_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("宋体", 9F);
            this.label25.Location = new System.Drawing.Point(195, 245);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(29, 12);
            this.label25.TabIndex = 20;
            this.label25.Text = "厘米";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label23.Location = new System.Drawing.Point(31, 207);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(231, 19);
            this.label23.TabIndex = 19;
            this.label23.Text = "移动长度  宽度     次数";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("宋体", 9F);
            this.label24.Location = new System.Drawing.Point(97, 245);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(29, 12);
            this.label24.TabIndex = 20;
            this.label24.Text = "厘米";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label19.Location = new System.Drawing.Point(31, 120);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(275, 19);
            this.label19.TabIndex = 19;
            this.label19.Text = "机械臂末端距离喷印平面的高度";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("宋体", 9F);
            this.label20.Location = new System.Drawing.Point(97, 157);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(29, 12);
            this.label20.TabIndex = 20;
            this.label20.Text = "厘米";
            // 
            // label_H_speed
            // 
            this.label_H_speed.AutoSize = true;
            this.label_H_speed.Font = new System.Drawing.Font("宋体", 12F);
            this.label_H_speed.Location = new System.Drawing.Point(188, 24);
            this.label_H_speed.Name = "label_H_speed";
            this.label_H_speed.Size = new System.Drawing.Size(23, 16);
            this.label_H_speed.TabIndex = 18;
            this.label_H_speed.Text = "20";
            // 
            // trackBar_H_speed
            // 
            this.trackBar_H_speed.Location = new System.Drawing.Point(35, 44);
            this.trackBar_H_speed.Maximum = 100;
            this.trackBar_H_speed.Name = "trackBar_H_speed";
            this.trackBar_H_speed.Size = new System.Drawing.Size(329, 45);
            this.trackBar_H_speed.TabIndex = 17;
            this.trackBar_H_speed.Value = 20;
            this.trackBar_H_speed.Scroll += new System.EventHandler(this.trackBar_H_speed_Scroll);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label21.Location = new System.Drawing.Point(31, 22);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(142, 19);
            this.label21.TabIndex = 21;
            this.label21.Text = "机械臂移动速度";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_camSTOP);
            this.groupBox3.Controls.Add(this.btn_camSTART);
            this.groupBox3.Controls.Add(this.btn_laserStop);
            this.groupBox3.Controls.Add(this.btn_laser);
            this.groupBox3.Controls.Add(this.label28);
            this.groupBox3.Controls.Add(this.label38);
            this.groupBox3.Controls.Add(this.label_t);
            this.groupBox3.Controls.Add(this.label35);
            this.groupBox3.Controls.Add(this.label_c);
            this.groupBox3.Controls.Add(this.label33);
            this.groupBox3.Controls.Add(this.label_v);
            this.groupBox3.Controls.Add(this.label36);
            this.groupBox3.Controls.Add(this.label29);
            this.groupBox3.Controls.Add(this.label32);
            this.groupBox3.Controls.Add(this.label_Distance);
            this.groupBox3.Controls.Add(this.label31);
            this.groupBox3.Controls.Add(this.label_test);
            this.groupBox3.Controls.Add(this.label30);
            this.groupBox3.Location = new System.Drawing.Point(527, 28);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Size = new System.Drawing.Size(470, 409);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "机械臂";
            // 
            // btn_camSTOP
            // 
            this.btn_camSTOP.Location = new System.Drawing.Point(323, 192);
            this.btn_camSTOP.Name = "btn_camSTOP";
            this.btn_camSTOP.Size = new System.Drawing.Size(121, 48);
            this.btn_camSTOP.TabIndex = 5;
            this.btn_camSTOP.Text = "测试关闭";
            this.btn_camSTOP.UseVisualStyleBackColor = true;
            // 
            // btn_camSTART
            // 
            this.btn_camSTART.Location = new System.Drawing.Point(196, 192);
            this.btn_camSTART.Name = "btn_camSTART";
            this.btn_camSTART.Size = new System.Drawing.Size(121, 48);
            this.btn_camSTART.TabIndex = 5;
            this.btn_camSTART.Text = "测试按钮";
            this.btn_camSTART.UseVisualStyleBackColor = true;
            this.btn_camSTART.Click += new System.EventHandler(this.btn_camSTART_Click);
            // 
            // btn_laserStop
            // 
            this.btn_laserStop.Location = new System.Drawing.Point(137, 360);
            this.btn_laserStop.Name = "btn_laserStop";
            this.btn_laserStop.Size = new System.Drawing.Size(95, 32);
            this.btn_laserStop.TabIndex = 5;
            this.btn_laserStop.Text = "关闭激光传感器";
            this.btn_laserStop.UseVisualStyleBackColor = true;
            this.btn_laserStop.Click += new System.EventHandler(this.btn_laserStop_Click);
            // 
            // btn_laser
            // 
            this.btn_laser.Location = new System.Drawing.Point(18, 355);
            this.btn_laser.Name = "btn_laser";
            this.btn_laser.Size = new System.Drawing.Size(97, 42);
            this.btn_laser.TabIndex = 5;
            this.btn_laser.Text = "连接激光传感器";
            this.btn_laser.UseVisualStyleBackColor = true;
            this.btn_laser.Click += new System.EventHandler(this.btn_laserStart_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("宋体", 13F);
            this.label28.Location = new System.Drawing.Point(31, 274);
            this.label28.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(26, 18);
            this.label28.TabIndex = 0;
            this.label28.Text = "秒";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("宋体", 13F);
            this.label38.Location = new System.Drawing.Point(46, 42);
            this.label38.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(44, 18);
            this.label38.TabIndex = 0;
            this.label38.Text = "温度";
            // 
            // label_t
            // 
            this.label_t.AutoSize = true;
            this.label_t.Font = new System.Drawing.Font("宋体", 13F);
            this.label_t.Location = new System.Drawing.Point(90, 42);
            this.label_t.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_t.Name = "label_t";
            this.label_t.Size = new System.Drawing.Size(17, 18);
            this.label_t.TabIndex = 0;
            this.label_t.Text = "0";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("宋体", 13F);
            this.label35.Location = new System.Drawing.Point(46, 69);
            this.label35.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(44, 18);
            this.label35.TabIndex = 0;
            this.label35.Text = "电流";
            // 
            // label_c
            // 
            this.label_c.AutoSize = true;
            this.label_c.Font = new System.Drawing.Font("宋体", 13F);
            this.label_c.Location = new System.Drawing.Point(90, 69);
            this.label_c.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_c.Name = "label_c";
            this.label_c.Size = new System.Drawing.Size(17, 18);
            this.label_c.TabIndex = 0;
            this.label_c.Text = "0";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("宋体", 13F);
            this.label33.Location = new System.Drawing.Point(46, 96);
            this.label33.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(44, 18);
            this.label33.TabIndex = 0;
            this.label33.Text = "电压";
            // 
            // label_v
            // 
            this.label_v.AutoSize = true;
            this.label_v.Font = new System.Drawing.Font("宋体", 13F);
            this.label_v.Location = new System.Drawing.Point(90, 96);
            this.label_v.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_v.Name = "label_v";
            this.label_v.Size = new System.Drawing.Size(17, 18);
            this.label_v.TabIndex = 0;
            this.label_v.Text = "0";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("宋体", 13F);
            this.label36.Location = new System.Drawing.Point(112, 42);
            this.label36.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(26, 18);
            this.label36.TabIndex = 0;
            this.label36.Text = "℃";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("宋体", 13F);
            this.label29.Location = new System.Drawing.Point(31, 327);
            this.label29.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(44, 18);
            this.label29.TabIndex = 0;
            this.label29.Text = "距离";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("宋体", 13F);
            this.label32.Location = new System.Drawing.Point(115, 69);
            this.label32.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(17, 18);
            this.label32.TabIndex = 0;
            this.label32.Text = "A";
            // 
            // label_Distance
            // 
            this.label_Distance.AutoSize = true;
            this.label_Distance.Font = new System.Drawing.Font("宋体", 13F);
            this.label_Distance.Location = new System.Drawing.Point(74, 327);
            this.label_Distance.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Distance.Name = "label_Distance";
            this.label_Distance.Size = new System.Drawing.Size(17, 18);
            this.label_Distance.TabIndex = 0;
            this.label_Distance.Text = "0";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("宋体", 13F);
            this.label31.Location = new System.Drawing.Point(115, 96);
            this.label31.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(17, 18);
            this.label31.TabIndex = 0;
            this.label31.Text = "V";
            // 
            // label_test
            // 
            this.label_test.AutoSize = true;
            this.label_test.Font = new System.Drawing.Font("宋体", 11F);
            this.label_test.Location = new System.Drawing.Point(62, 277);
            this.label_test.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_test.Name = "label_test";
            this.label_test.Size = new System.Drawing.Size(15, 15);
            this.label_test.TabIndex = 0;
            this.label_test.Text = "0";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("宋体", 13F);
            this.label30.Location = new System.Drawing.Point(156, 327);
            this.label30.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(26, 18);
            this.label30.TabIndex = 0;
            this.label30.Text = "mm";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.groupBox7);
            this.groupBox5.Controls.Add(this.groupBox8);
            this.groupBox5.Controls.Add(this.groupBox6);
            this.groupBox5.Controls.Add(this.btnClear);
            this.groupBox5.Controls.Add(this.btnClose);
            this.groupBox5.Controls.Add(this.btn9837Send);
            this.groupBox5.Controls.Add(this.btn8080Send);
            this.groupBox5.Controls.Add(this.btnConnect);
            this.groupBox5.Controls.Add(this.comboBox2);
            this.groupBox5.Controls.Add(this.label27);
            this.groupBox5.Controls.Add(this.comboBox_ip);
            this.groupBox5.Controls.Add(this.label22);
            this.groupBox5.Font = new System.Drawing.Font("宋体", 11F);
            this.groupBox5.Location = new System.Drawing.Point(10, 11);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox5.Size = new System.Drawing.Size(358, 898);
            this.groupBox5.TabIndex = 35;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Socket";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.rtb8080Log);
            this.groupBox7.Location = new System.Drawing.Point(6, 54);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(348, 277);
            this.groupBox7.TabIndex = 2;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "8080数据端口日志";
            // 
            // rtb8080Log
            // 
            this.rtb8080Log.Location = new System.Drawing.Point(10, 26);
            this.rtb8080Log.Name = "rtb8080Log";
            this.rtb8080Log.ReadOnly = true;
            this.rtb8080Log.Size = new System.Drawing.Size(330, 245);
            this.rtb8080Log.TabIndex = 2;
            this.rtb8080Log.Text = "";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.textInput);
            this.groupBox8.Location = new System.Drawing.Point(6, 634);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(346, 118);
            this.groupBox8.TabIndex = 2;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "发送窗口";
            // 
            // textInput
            // 
            this.textInput.Location = new System.Drawing.Point(9, 23);
            this.textInput.Multiline = true;
            this.textInput.Name = "textInput";
            this.textInput.Size = new System.Drawing.Size(337, 75);
            this.textInput.TabIndex = 1;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.rtb9837Log);
            this.groupBox6.Location = new System.Drawing.Point(4, 337);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(348, 288);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "9837指令端口日志";
            // 
            // rtb9837Log
            // 
            this.rtb9837Log.Location = new System.Drawing.Point(13, 23);
            this.rtb9837Log.Name = "rtb9837Log";
            this.rtb9837Log.ReadOnly = true;
            this.rtb9837Log.Size = new System.Drawing.Size(330, 245);
            this.rtb9837Log.TabIndex = 2;
            this.rtb9837Log.Text = "";
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("宋体", 11F);
            this.btnClear.Location = new System.Drawing.Point(15, 766);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(111, 32);
            this.btnClear.TabIndex = 0;
            this.btnClear.Text = "清空";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("宋体", 11F);
            this.btnClose.Location = new System.Drawing.Point(235, 766);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(111, 32);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btn9837Send
            // 
            this.btn9837Send.Font = new System.Drawing.Font("宋体", 11F);
            this.btn9837Send.Location = new System.Drawing.Point(239, 826);
            this.btn9837Send.Name = "btn9837Send";
            this.btn9837Send.Size = new System.Drawing.Size(111, 32);
            this.btn9837Send.TabIndex = 0;
            this.btn9837Send.Text = "指令发送";
            this.btn9837Send.UseVisualStyleBackColor = true;
            this.btn9837Send.Click += new System.EventHandler(this.btn9837Send_Click);
            // 
            // btn8080Send
            // 
            this.btn8080Send.Font = new System.Drawing.Font("宋体", 11F);
            this.btn8080Send.Location = new System.Drawing.Point(122, 826);
            this.btn8080Send.Name = "btn8080Send";
            this.btn8080Send.Size = new System.Drawing.Size(111, 32);
            this.btn8080Send.TabIndex = 0;
            this.btn8080Send.Text = "数据发送";
            this.btn8080Send.UseVisualStyleBackColor = true;
            this.btn8080Send.Click += new System.EventHandler(this.btn8080Send_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("宋体", 11F);
            this.btnConnect.Location = new System.Drawing.Point(2, 826);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(111, 32);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "连接UV喷印";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(269, 17);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(78, 23);
            this.comboBox2.TabIndex = 4;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label27.Location = new System.Drawing.Point(223, 19);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(39, 16);
            this.label27.TabIndex = 6;
            this.label27.Text = "端口";
            // 
            // comboBox_ip
            // 
            this.comboBox_ip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ip.FormattingEnabled = true;
            this.comboBox_ip.Location = new System.Drawing.Point(58, 17);
            this.comboBox_ip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox_ip.Name = "comboBox_ip";
            this.comboBox_ip.Size = new System.Drawing.Size(161, 23);
            this.comboBox_ip.TabIndex = 4;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label22.Location = new System.Drawing.Point(29, 19);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(23, 16);
            this.label22.TabIndex = 6;
            this.label22.Text = "IP";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1035, 922);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_speed)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_H_speed)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RadioButton radioButton_Z;
        private System.Windows.Forms.RadioButton radioButton_N;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label_speed;
        private System.Windows.Forms.TrackBar trackBar_speed;
        private System.Windows.Forms.RadioButton radioButton_H_Z;
        private System.Windows.Forms.RadioButton radioButton_H_N;
        private System.Windows.Forms.TextBox textBox_H_UVheight;
        private System.Windows.Forms.Button btn_H_start;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label_H_speed;
        private System.Windows.Forms.TrackBar trackBar_H_speed;
        private System.Windows.Forms.TextBox textBox_H_count;
        private System.Windows.Forms.TextBox textBox_H_wid;
        private System.Windows.Forms.TextBox textBox_H_len;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button btnArmInit;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label_test;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label_Distance;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Button btn_laser;
        private System.Windows.Forms.Button btn_camSTART;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label_t;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label_c;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label_v;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Button btn_noPrintf;
        private System.Windows.Forms.RadioButton radioButton_PrintClose;
        private System.Windows.Forms.RadioButton radioButton_PrintOpen;
        private System.Windows.Forms.Button btn_camSTOP;
        private System.Windows.Forms.Button btn_laserStop;
        private System.Windows.Forms.TextBox tbx_Height;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RichTextBox rtb8080Log;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TextBox textInput;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RichTextBox rtb9837Log;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btn9837Send;
        private System.Windows.Forms.Button btn8080Send;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.ComboBox comboBox_ip;
        private System.Windows.Forms.Label label22;
    }
}

