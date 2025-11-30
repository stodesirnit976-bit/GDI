using GDI.Core;
using GDI.Models;
using GDI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GDI.Services.Arm;
//using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;
//using GDI.Func;

namespace GDI
{
    public partial class Form1 : Form
    {
        

        // ================= 窗口初始化 ==================
        public Form1()
        {
            
            InitializeComponent();


            //Control.CheckForIllegalCrossThreadCalls = false; 这行代码强制关闭了多线程冲突

            
        }



        // ================= form加载 =================
        private void Form1_Load(object sender, EventArgs e)
        {
            
            // 加载ip地址
            GetIP();

            //Task.Run(loop);
            //test_init();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            //StateReader.Stop();
        }




        // ===================================================================================================
        // =============================== UI更新/跨线程参数传递/Test按钮 ====================================
        // ===================================================================================================

        private void btn_camSTART_Click(object sender, EventArgs e)
        {

        }





        // ===================================================================================================
        // ============================================= Socket ==============================================
        // ===================================================================================================
        SocketClient client9837;
        SocketClient client8080;


        // ================= 对外接口：给MainForm用 ==================

        // ----------- 开启socket -----------
        public void socket_Start()
        {
            btnConnect.Enabled = false;

            socket_Init();

            //string ip = (string)comboBox_ip.SelectedItem; // 这里等等改成固定ip
            string ip = "192.168.22.62";
            client9837.Connect(ip, 9837); // 连控制口
            client8080.Connect(ip, 8080); // 连数据口

            Console.WriteLine("已调用form的socket连接");
        }

        // ----------- 关闭socket -----------
        public void socket_close()
        {
            client8080.Close();
            Show8080(DateTime.Now.ToString("yy-MM-dd hh:mm:ss ") + "已关闭连接" + "\r\n");
            client9837.Close();
            Show9837(DateTime.Now.ToString("yy-MM-dd hh:mm:ss ") + "已关闭连接" + "\r\n");
            btnConnect.Enabled = true;
        }


        // ================= 对内接口：给MainForm/Form用 ==================

        // ----------- 初始化socket -----------
        private void socket_Init()
        {
            // --- 设置 9837 (控制口) ---
            client9837 = new SocketClient();
            client9837.MsgFunc = Show9837;
            client9837.changeBtn = socketServer_closed;

            // --- 设置 8080 (数据口) ---
            client8080 = new SocketClient();
            client8080.MsgFunc = Show8080;
            client8080.changeBtn = socketServer_closed;
        }

        // ----------- 连接按钮 -----------
        private void btnConnect_Click(object sender, EventArgs e)
        {
            socket_Start();
            Console.WriteLine("已点击form的socket连接按钮");
        }

        // ----------- 指令发送按钮 (发给9837) -----------
        private void btn9837Send_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textInput.Text))
                client9837.Send(textInput.Text);
        }

        // ----------- 数据发送按钮 (发给8080) -----------
        private void btn8080Send_Click(object sender, EventArgs e)
        {
            // 获取输入框内容直接发送
            if (!string.IsNullOrEmpty(textInput.Text))
                client8080.Send(textInput.Text);
        }

        // ----------- 委托回调显示9837接收的数据 -----------
        private void Show9837(string msg)
        {
            this.BeginInvoke(new Action(() =>
            {
                rtb9837Log.AppendText(msg);
                rtb9837Log.ScrollToCaret();
            }));  
        }

        // ----------- 委托回调显示8080接收的数据 -----------
        private void Show8080(string msg)
        {
            this.BeginInvoke(new Action( ()=>
            {
                    rtb8080Log.AppendText(msg); // 显示文本
                    rtb8080Log.ScrollToCaret(); // 滚到底部
            }));
        }

        // ----------- 主动关闭socket -----------
        private void btnClose_Click(object sender, EventArgs e)
        {
            socket_close();
            Console.WriteLine("已点击form的socket关闭按钮");
        }

        // ----------- 服务端关闭socket -----------
        private void socketServer_closed()
        {
            if (btnConnect.InvokeRequired)
            {
                this.BeginInvoke(new Action(socketServer_closed));
            }
            else
            {
                btnConnect.Enabled = true;
            }
        }

        // ----------- 清空log -----------
        private void btnClear_Click(object sender, EventArgs e)
        {
            rtb8080Log.Clear();
            rtb9837Log.Clear();
        }



        // 辅助函数：获取本机ip
        private void GetIP()
        {
            var ipStrings = new List<string> { "192.168.1.165" }; // 至少包含回环地址
            // 添加局域网 IP
            var networkIPs = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
                .Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)
                .Where(ip => !IPAddress.IsLoopback(ip.Address))
                .Select(ip => ip.Address.ToString());

            ipStrings.AddRange(networkIPs);

            // 绑定到 ComboBox
            comboBox_ip.DataSource = ipStrings; // 直接绑定 string 列表
            if (comboBox_ip.Items.Count > 0)
            {
                comboBox_ip.SelectedIndex = 0; // 自动选中第一个
            }
        }




        // ===================================================================================================
        // ======================================= 机械臂初始化设置 ==========================================
        // ===================================================================================================
        string loadPath = @"D:\img"; // 选取图片文件夹


        // =============== 对外接口：给MainForm/form用 ===============

        // -------- 机械臂初始化 --------
        public int arm_Init()
        {
            Arm.armInit(); // 由于机械臂要移动到初始位置，后续相机标定等操作都要在机械臂初始化完成后进行，所以这里不启用新线程
            return 6;
        }

        // -------- 机械臂关闭 --------
        public void arm_Close()
        {
            
        }

        // -------- 机械臂急停 --------
        public void arm_EStop()
        {
            Task.Run(() =>
            {
                Arm.rm_set_arm_stop(Arm.Instance.robotHandlePtr);
            });
        }


        // =============== 对内接口：给form用 ===============

        // -------- 连接机械臂 --------
        private void btnArmInit_Click(object sender, EventArgs e)
        {
            arm_Init();
        }

        // -------- 机械臂急停 --------
        private void btnStop_Click(object sender, EventArgs e)
        {
            arm_EStop();
        }

        // -------- 机械臂开关喷印系统触发 --------
        private void btn_noPrintf_Click(object sender, EventArgs e)
        {
            bool _switch = radioButton_PrintClose.Checked;
            int ret = Arm.rm_set_IO_mode(Arm.Instance.robotHandlePtr, 3, 1);// 设置为通用输出模式
            if (_switch)
            {
                int a = Arm.rm_set_DO_state(Arm.Instance.robotHandlePtr, 3, 1);// 设置1号端口输出低电平
                //Arm.rm_set_IO_mode(Arm.Instance.robotHandlePtr, 2, 0);// 1号 io 端口设置为通用输入模式(即关闭uv触发的io输出)
                Console.WriteLine($"2号口设置为输出模式低电平{ret}{a}");
            }
            else
            {

                int b = Arm.rm_set_DO_state(Arm.Instance.robotHandlePtr, 3, 0);// 设置1号端口输出低电平
                Console.WriteLine($"2号口设置为输出模式高电平{ret}{b}");
            }
            // 以上是高电平2v，低电平1v。并且不知道为什么不管设置哪个io口，接的那根线都会输出变化电平
        }


        // ================= 模版 ==============

        // -------- 机械臂调速滑块 --------
        private void trackBar_speed_Scroll(object sender, EventArgs e)
        {
            label_speed.Text = trackBar_speed.Value.ToString();
        }

        // -------- 机械臂运行 --------
        private async void btn_start_Click(object sender, EventArgs e)
        {
            btn_start.Enabled = false;
            if (string.IsNullOrEmpty(tbx_Height.Text))
            {
                MessageBox.Show("请输入高度参数！");
                return;
            }

            // 以下的策略是只需要手动设置速度、NorZ、高度三个参数
            // 长宽直接取自文件夹文件数值
            // 直接读取 sliceImage 发送文件夹里第一张图片的尺寸
            var firstFile = Directory.EnumerateFiles(loadPath, "*.bmp")
                         .FirstOrDefault();

            if (firstFile == null)
            {
                MessageBox.Show("文件夹里没找到 .bmp 文件");
                return;
            }
            using var img = Image.FromFile(firstFile);

            float tran = (float)54.36 / 1280 / 1000; // 1 pixel = tran 米
            float len = (float)img.Width * tran;
            float wid = (float)img.Height * tran;

            // 假定激光传感器和机械臂末端平齐
            // 假设有一个函数 ArmHeight(work_h) 设定作业平面，得把这个函数加入 Arm.test的形参中，
            // 设定作业距离为 5mm ，也就是 _distance = 5,
            // 即 if(_distance =< 5) 
            // 机械臂末端离平面高度，单位 m
            float height = float.Parse(tbx_Height.Text) / 100;

            // n走线或z走线
            bool N = radioButton_N.Checked;
            // 走线次数
            int count = Directory.EnumerateFiles(loadPath, "*.bmp").Count();
            // 速度
            int vol = int.Parse(label_speed.Text);

            await Task.Run(() =>
            {
                try
                {
                    // 启动机械臂
                    Arm.Instance.move(len + (float)0.2, wid, height, N, count, vol);///////////这里长度加了0.2m抵消延迟机械臂移动距离不够
                }
                catch (Exception ex)
                {
                    MessageBox.Show("机械臂启动失败: " + ex.Message);
                }
            });

            btn_start.Enabled = true;
        }


        // ================= 高级 ==============

        // -------- 机械臂调速滑块 --------
        private void trackBar_H_speed_Scroll(object sender, EventArgs e)
        {
            label_H_speed.Text = trackBar_H_speed.Value.ToString();
        }

        // -------- 机械臂运行 --------
        private async void btn_H_start_Click(object sender, EventArgs e)
        {
            btn_H_start.Enabled = false;

            var inputs = new TextBox[] { textBox_H_len, textBox_H_UVheight, textBox_H_wid, textBox_H_count };
            if (inputs.Any(t => string.IsNullOrEmpty(t.Text)))
            {
                MessageBox.Show("请输入全部参数！");
                return;
            }

            float len = float.Parse(textBox_H_len.Text) / 100;
            float wid = float.Parse(textBox_H_wid.Text) / 100;

            // 机械臂末端离平面高度，单位 m
            float height = float.Parse(textBox_H_UVheight.Text) / 100;
            // n走线或z走线
            bool N = radioButton_H_N.Checked;
            // 走线次数
            int count = int.Parse(textBox_H_count.Text);
            // 速度
            int vol = int.Parse(label_H_speed.Text);

            // 启动机械臂
            await Task.Run(() =>
            {
                // 启动机械臂
                Arm.Instance.move(len, wid, height, N, count, vol);
            });

            btn_H_start.Enabled = true;
        }




        // ===================================================================================================
        // ========================================= 激光传感器 ==============================================
        // ===================================================================================================
        LaserSensor laserSensor = new LaserSensor();
        private string distance = "";


        // =============== 对外接口：给MainForm/form用 ===============

        // -------- 启动激光测距传感器 --------
        public void laser_Start()
        {
            laserSensor.D = D_update;
            laserSensor.Start();
        }
        // -------- 停止激光测距传感器 --------
        public void laser_Stop()
        {
            laserSensor.Stop();
        }


        // =============== 对内接口：给form用 ===============

        // -------- 激光传感器启动按钮 --------
        private void btn_laserStop_Click(object sender, EventArgs e)
        {
            laser_Start();
        }
        // -------- 激光传感器停止按钮 --------
        private void btn_laserStart_Click(object sender, EventArgs e)
        {
            laser_Stop();
        }

        // -------- 激光传感器距离数据更新回调 --------
        private void D_update(string str)
        {
            this.BeginInvoke(new Action(() =>
            {
                label_Distance.Text = str;// 这里str就是距离值
                distance = str;
            }));
        }




        // ===================================================================================================
        // ====================================== 机械比状态数据更新 =========================================
        // ===================================================================================================


        // 纯传后台线程参数
        private void State_Update(string v, string c, string t)
        {
            // 这里没有通过 while 标志位来实现循环终止，因为 plusss 里是不是无线循环
            this.BeginInvoke(new Action(() =>
            {
                label_c.Text = c;
            }));
        }

        
    }

}


