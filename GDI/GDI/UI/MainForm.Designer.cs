namespace GDI
{
    partial class MainForm
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
            this.btn_Detail = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_trans = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_height = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_width = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_length = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.rbt_ON = new System.Windows.Forms.RadioButton();
            this.rbt_OFF = new System.Windows.Forms.RadioButton();
            this.comboBoxTemplate = new System.Windows.Forms.ComboBox();
            this.textBox_volume = new System.Windows.Forms.TextBox();
            this.textBox_tareWeight = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_payLoad = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.radioButton_h1 = new System.Windows.Forms.RadioButton();
            this.radioButton_v1 = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBox_filePicture = new System.Windows.Forms.ComboBox();
            this.btn_Generate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox_Preview = new System.Windows.Forms.PictureBox();
            this.btn_EStop = new System.Windows.Forms.Button();
            this.btn_Wrok = new System.Windows.Forms.Button();
            this.btn_SysInit = new System.Windows.Forms.Button();
            this.tbx_UVheight = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.rbt_PfintOFF = new System.Windows.Forms.RadioButton();
            this.rbt_PrintON = new System.Windows.Forms.RadioButton();
            this.btn_RESEAT = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Preview)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Detail
            // 
            this.btn_Detail.Location = new System.Drawing.Point(28, 519);
            this.btn_Detail.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Detail.Name = "btn_Detail";
            this.btn_Detail.Size = new System.Drawing.Size(222, 58);
            this.btn_Detail.TabIndex = 0;
            this.btn_Detail.Text = "详情";
            this.btn_Detail.UseVisualStyleBackColor = true;
            this.btn_Detail.Click += new System.EventHandler(this.btn_Detail_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.panel1);
            this.groupBox4.Location = new System.Drawing.Point(670, 28);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(883, 577);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "深度相机";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(111, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(758, 546);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.tabControl1);
            this.groupBox9.Location = new System.Drawing.Point(340, 28);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox9.Size = new System.Drawing.Size(291, 536);
            this.groupBox9.TabIndex = 37;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "图片生成";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(4, 19);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(283, 508);
            this.tabControl1.TabIndex = 34;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.textBox_trans);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.textBox_height);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.textBox_width);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.textBox_length);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.comboBoxTemplate);
            this.tabPage1.Controls.Add(this.rbt_ON);
            this.tabPage1.Controls.Add(this.textBox_volume);
            this.tabPage1.Controls.Add(this.textBox_tareWeight);
            this.tabPage1.Controls.Add(this.rbt_OFF);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.textBox_payLoad);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(275, 482);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "文字";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(23, 268);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 19);
            this.label9.TabIndex = 2;
            this.label9.Text = "模版2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(23, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "模版1";
            // 
            // textBox_trans
            // 
            this.textBox_trans.Location = new System.Drawing.Point(24, 380);
            this.textBox_trans.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_trans.Name = "textBox_trans";
            this.textBox_trans.Size = new System.Drawing.Size(51, 21);
            this.textBox_trans.TabIndex = 33;
            this.textBox_trans.Text = "15";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(23, 359);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 19);
            this.label14.TabIndex = 32;
            this.label14.Text = "换长：";
            // 
            // textBox_height
            // 
            this.textBox_height.Location = new System.Drawing.Point(150, 327);
            this.textBox_height.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_height.Name = "textBox_height";
            this.textBox_height.Size = new System.Drawing.Size(47, 21);
            this.textBox_height.TabIndex = 31;
            this.textBox_height.Text = "2.7";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(20, 54);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 19);
            this.label4.TabIndex = 2;
            this.label4.Text = "载重：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(150, 305);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 19);
            this.label11.TabIndex = 29;
            this.label11.Text = "高：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(19, 113);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 19);
            this.label5.TabIndex = 2;
            this.label5.Text = "自重：";
            // 
            // textBox_width
            // 
            this.textBox_width.Location = new System.Drawing.Point(88, 327);
            this.textBox_width.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_width.Name = "textBox_width";
            this.textBox_width.Size = new System.Drawing.Size(47, 21);
            this.textBox_width.TabIndex = 28;
            this.textBox_width.Text = "2.8";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(20, 168);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 19);
            this.label8.TabIndex = 2;
            this.label8.Text = "容积：";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(88, 305);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(47, 19);
            this.label16.TabIndex = 26;
            this.label16.Text = "宽：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(22, 425);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "模版：";
            // 
            // textBox_length
            // 
            this.textBox_length.Location = new System.Drawing.Point(24, 327);
            this.textBox_length.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_length.Name = "textBox_length";
            this.textBox_length.Size = new System.Drawing.Size(47, 21);
            this.textBox_length.TabIndex = 25;
            this.textBox_length.Text = "15.4";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(26, 305);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 19);
            this.label10.TabIndex = 23;
            this.label10.Text = "长：";
            // 
            // rbt_ON
            // 
            this.rbt_ON.AutoSize = true;
            this.rbt_ON.Checked = true;
            this.rbt_ON.Location = new System.Drawing.Point(149, 450);
            this.rbt_ON.Margin = new System.Windows.Forms.Padding(2);
            this.rbt_ON.Name = "rbt_ON";
            this.rbt_ON.Size = new System.Drawing.Size(47, 16);
            this.rbt_ON.TabIndex = 8;
            this.rbt_ON.TabStop = true;
            this.rbt_ON.Text = "横版";
            this.rbt_ON.UseVisualStyleBackColor = true;
            // 
            // rbt_OFF
            // 
            this.rbt_OFF.AutoSize = true;
            this.rbt_OFF.Location = new System.Drawing.Point(210, 450);
            this.rbt_OFF.Margin = new System.Windows.Forms.Padding(2);
            this.rbt_OFF.Name = "rbt_OFF";
            this.rbt_OFF.Size = new System.Drawing.Size(47, 16);
            this.rbt_OFF.TabIndex = 8;
            this.rbt_OFF.TabStop = true;
            this.rbt_OFF.Text = "竖版";
            this.rbt_OFF.UseVisualStyleBackColor = true;
            // 
            // comboBoxTemplate
            // 
            this.comboBoxTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemplate.FormattingEnabled = true;
            this.comboBoxTemplate.Location = new System.Drawing.Point(22, 450);
            this.comboBoxTemplate.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxTemplate.Name = "comboBoxTemplate";
            this.comboBoxTemplate.Size = new System.Drawing.Size(108, 20);
            this.comboBoxTemplate.TabIndex = 4;
            // 
            // textBox_volume
            // 
            this.textBox_volume.Location = new System.Drawing.Point(116, 170);
            this.textBox_volume.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_volume.Name = "textBox_volume";
            this.textBox_volume.Size = new System.Drawing.Size(58, 21);
            this.textBox_volume.TabIndex = 11;
            this.textBox_volume.Text = "120";
            // 
            // textBox_tareWeight
            // 
            this.textBox_tareWeight.Location = new System.Drawing.Point(116, 115);
            this.textBox_tareWeight.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_tareWeight.Name = "textBox_tareWeight";
            this.textBox_tareWeight.Size = new System.Drawing.Size(58, 21);
            this.textBox_tareWeight.TabIndex = 10;
            this.textBox_tareWeight.Text = "22.5";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(177, 171);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 24);
            this.label7.TabIndex = 12;
            this.label7.Text = "m³";
            // 
            // textBox_payLoad
            // 
            this.textBox_payLoad.Location = new System.Drawing.Point(116, 56);
            this.textBox_payLoad.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_payLoad.Name = "textBox_payLoad";
            this.textBox_payLoad.Size = new System.Drawing.Size(58, 21);
            this.textBox_payLoad.TabIndex = 9;
            this.textBox_payLoad.Text = "60";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(174, 56);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 24);
            this.label3.TabIndex = 12;
            this.label3.Text = "t";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(175, 115);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 24);
            this.label6.TabIndex = 12;
            this.label6.Text = "t";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.radioButton_h1);
            this.tabPage2.Controls.Add(this.radioButton_v1);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.comboBox_filePicture);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(275, 482);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "图标";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // radioButton_h1
            // 
            this.radioButton_h1.AutoSize = true;
            this.radioButton_h1.Checked = true;
            this.radioButton_h1.Location = new System.Drawing.Point(26, 323);
            this.radioButton_h1.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_h1.Name = "radioButton_h1";
            this.radioButton_h1.Size = new System.Drawing.Size(47, 16);
            this.radioButton_h1.TabIndex = 9;
            this.radioButton_h1.TabStop = true;
            this.radioButton_h1.Text = "横版";
            this.radioButton_h1.UseVisualStyleBackColor = true;
            // 
            // radioButton_v1
            // 
            this.radioButton_v1.AutoSize = true;
            this.radioButton_v1.Location = new System.Drawing.Point(86, 323);
            this.radioButton_v1.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_v1.Name = "radioButton_v1";
            this.radioButton_v1.Size = new System.Drawing.Size(47, 16);
            this.radioButton_v1.TabIndex = 10;
            this.radioButton_v1.TabStop = true;
            this.radioButton_v1.Text = "竖版";
            this.radioButton_v1.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(17, 57);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 29);
            this.label12.TabIndex = 1;
            this.label12.Text = "图库：";
            // 
            // comboBox_filePicture
            // 
            this.comboBox_filePicture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_filePicture.FormattingEnabled = true;
            this.comboBox_filePicture.Location = new System.Drawing.Point(21, 90);
            this.comboBox_filePicture.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_filePicture.Name = "comboBox_filePicture";
            this.comboBox_filePicture.Size = new System.Drawing.Size(92, 20);
            this.comboBox_filePicture.TabIndex = 0;
            this.comboBox_filePicture.SelectedIndexChanged += new System.EventHandler(this.comboBox_filePicture_SelectedIndexChanged);
            // 
            // btn_Generate
            // 
            this.btn_Generate.Location = new System.Drawing.Point(28, 184);
            this.btn_Generate.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Generate.Name = "btn_Generate";
            this.btn_Generate.Size = new System.Drawing.Size(222, 58);
            this.btn_Generate.TabIndex = 0;
            this.btn_Generate.Text = "喷印效果预览";
            this.btn_Generate.UseVisualStyleBackColor = true;
            this.btn_Generate.Click += new System.EventHandler(this.btn_Generate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox_Preview);
            this.groupBox1.Location = new System.Drawing.Point(697, 641);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(612, 254);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "喷印预览";
            // 
            // pictureBox_Preview
            // 
            this.pictureBox_Preview.Location = new System.Drawing.Point(14, 18);
            this.pictureBox_Preview.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_Preview.Name = "pictureBox_Preview";
            this.pictureBox_Preview.Size = new System.Drawing.Size(583, 208);
            this.pictureBox_Preview.TabIndex = 3;
            this.pictureBox_Preview.TabStop = false;
            // 
            // btn_EStop
            // 
            this.btn_EStop.Location = new System.Drawing.Point(28, 410);
            this.btn_EStop.Margin = new System.Windows.Forms.Padding(2);
            this.btn_EStop.Name = "btn_EStop";
            this.btn_EStop.Size = new System.Drawing.Size(222, 58);
            this.btn_EStop.TabIndex = 0;
            this.btn_EStop.Text = "机械臂急停";
            this.btn_EStop.UseVisualStyleBackColor = true;
            this.btn_EStop.Click += new System.EventHandler(this.btn_EStop_Click);
            // 
            // btn_Wrok
            // 
            this.btn_Wrok.Location = new System.Drawing.Point(28, 286);
            this.btn_Wrok.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Wrok.Name = "btn_Wrok";
            this.btn_Wrok.Size = new System.Drawing.Size(222, 58);
            this.btn_Wrok.TabIndex = 0;
            this.btn_Wrok.Text = "执行";
            this.btn_Wrok.UseVisualStyleBackColor = true;
            this.btn_Wrok.Click += new System.EventHandler(this.btn_Wrok_Click);
            // 
            // btn_SysInit
            // 
            this.btn_SysInit.Location = new System.Drawing.Point(28, 66);
            this.btn_SysInit.Margin = new System.Windows.Forms.Padding(2);
            this.btn_SysInit.Name = "btn_SysInit";
            this.btn_SysInit.Size = new System.Drawing.Size(222, 58);
            this.btn_SysInit.TabIndex = 0;
            this.btn_SysInit.Text = "系统初始化";
            this.btn_SysInit.UseVisualStyleBackColor = true;
            this.btn_SysInit.Click += new System.EventHandler(this.btn_SysInit_Click);
            // 
            // tbx_UVheight
            // 
            this.tbx_UVheight.Location = new System.Drawing.Point(8, 50);
            this.tbx_UVheight.Name = "tbx_UVheight";
            this.tbx_UVheight.Size = new System.Drawing.Size(53, 21);
            this.tbx_UVheight.TabIndex = 37;
            this.tbx_UVheight.Text = "10";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(4, 22);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(142, 19);
            this.label13.TabIndex = 35;
            this.label13.Text = "机械臂工作距离";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("宋体", 11F);
            this.label18.Location = new System.Drawing.Point(66, 56);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(23, 15);
            this.label18.TabIndex = 36;
            this.label18.Text = "mm";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.tbx_UVheight);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Location = new System.Drawing.Point(340, 585);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(291, 94);
            this.groupBox2.TabIndex = 39;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "机械臂设置";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(274, 1289);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(222, 58);
            this.button3.TabIndex = 0;
            this.button3.Text = "io高电平";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btn_test_Click);
            // 
            // rbt_PfintOFF
            // 
            this.rbt_PfintOFF.AutoSize = true;
            this.rbt_PfintOFF.Location = new System.Drawing.Point(135, 635);
            this.rbt_PfintOFF.Margin = new System.Windows.Forms.Padding(2);
            this.rbt_PfintOFF.Name = "rbt_PfintOFF";
            this.rbt_PfintOFF.Size = new System.Drawing.Size(95, 16);
            this.rbt_PfintOFF.TabIndex = 8;
            this.rbt_PfintOFF.TabStop = true;
            this.rbt_PfintOFF.Text = "关闭喷印触发";
            this.rbt_PfintOFF.UseVisualStyleBackColor = true;
            // 
            // rbt_PrintON
            // 
            this.rbt_PrintON.AutoSize = true;
            this.rbt_PrintON.Checked = true;
            this.rbt_PrintON.Location = new System.Drawing.Point(36, 635);
            this.rbt_PrintON.Margin = new System.Windows.Forms.Padding(2);
            this.rbt_PrintON.Name = "rbt_PrintON";
            this.rbt_PrintON.Size = new System.Drawing.Size(95, 16);
            this.rbt_PrintON.TabIndex = 8;
            this.rbt_PrintON.TabStop = true;
            this.rbt_PrintON.Text = "开启喷印触发";
            this.rbt_PrintON.UseVisualStyleBackColor = true;
            // 
            // btn_RESEAT
            // 
            this.btn_RESEAT.Location = new System.Drawing.Point(28, 685);
            this.btn_RESEAT.Margin = new System.Windows.Forms.Padding(2);
            this.btn_RESEAT.Name = "btn_RESEAT";
            this.btn_RESEAT.Size = new System.Drawing.Size(222, 58);
            this.btn_RESEAT.TabIndex = 0;
            this.btn_RESEAT.Text = "重启";
            this.btn_RESEAT.UseVisualStyleBackColor = true;
            this.btn_RESEAT.Click += new System.EventHandler(this.btn_RESEAT_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1583, 754);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Generate);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btn_SysInit);
            this.Controls.Add(this.btn_Wrok);
            this.Controls.Add(this.btn_EStop);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.rbt_PrintON);
            this.Controls.Add(this.btn_RESEAT);
            this.Controls.Add(this.btn_Detail);
            this.Controls.Add(this.rbt_PfintOFF);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Preview)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Detail;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_trans;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_height;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_width;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_length;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton rbt_ON;
        private System.Windows.Forms.RadioButton rbt_OFF;
        private System.Windows.Forms.ComboBox comboBoxTemplate;
        private System.Windows.Forms.TextBox textBox_volume;
        private System.Windows.Forms.TextBox textBox_tareWeight;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_payLoad;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RadioButton radioButton_h1;
        private System.Windows.Forms.RadioButton radioButton_v1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBox_filePicture;
        private System.Windows.Forms.Button btn_Generate;
        private System.Windows.Forms.PictureBox pictureBox_Preview;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_EStop;
        private System.Windows.Forms.Button btn_Wrok;
        private System.Windows.Forms.Button btn_SysInit;
        private System.Windows.Forms.TextBox tbx_UVheight;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RadioButton rbt_PfintOFF;
        private System.Windows.Forms.RadioButton rbt_PrintON;
        private System.Windows.Forms.Button btn_RESEAT;
    }
}