using GDI.Core;
using GDI.Models;
using GDI.Services;
using Modbus.Device;
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
        string savePath = @"D:\TextImages"; // 完整图片保存文件夹
        string sliceSavePath = @"D:\TextImages\SliceImages"; // 切割图片存放文件夹
        string loadPath = @"D:\img"; // 选取图片文件夹

        int sliceHight = 1178;

        // 生成图片的命名数
        private static int count = 0;
        private string name;

        private TemplateManager templateManager = new TemplateManager();
        List<Template> templates;//结构模版的列表
        private TextRender TextRender = new TextRender();

        // ================= 窗口初始化 ==================
        public Form1()
        {
            count = 0;
            InitializeComponent();

            InitializeClients();

            //Control.CheckForIllegalCrossThreadCalls = false; 这行代码强制关闭了多线程冲突

            pictureBox_Preview.SizeMode = PictureBoxSizeMode.Zoom;// 初始化,让PictureBox适应图片大小

            // 双缓冲防闪烁
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });
        }



        // ================= form加载 =================
        private void Form1_Load(object sender, EventArgs e)
        {
            // 文字的combobox导入
            templates = templateManager.GetAllTemplates();//把所有模版放入结构模版列表
            comboBoxTemplate.DataSource = templates.Where(t => t.Name.Contains("模板")).ToList();
            comboBoxTemplate.DisplayMember = "Name";

            // 图像的combobox导入
            PictureListLoader.Load(loadPath, comboBox_filePicture);
            // 加载ip地址
            GetIP();

            //Task.Run(loop);
            //test_init();

        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            StateReader.Stop();
        }




        // ===================================================================================================
        // ====================================== UI更新/跨线程参数传递 ======================================
        // ===================================================================================================
        private string _distance = "";

        private void M_update(string str)
        {
            // 这里没有通过 while 标志位来实现循环终止，因为 plusss 里是不是无线循环
            this.BeginInvoke(new Action(() =>
            {
                label_test.Text = str;
            }));
        }
        private void D_update(string str)
        {
            // 这里没有通过 while 标志位来实现循环终止，因为 plusss 里是不是无线循环
            this.BeginInvoke(new Action(() =>
            {
                label_Distance.Text = str;// 这里str就是距离值
                _distance = str;
            }));
        }



        // 纯传后台线程参数
        private void State_Update(string v, string c, string t)
        {
            // 这里没有通过 while 标志位来实现循环终止，因为 plusss 里是不是无线循环
            this.BeginInvoke(new Action(() =>
            {
                label_c.Text = c;               
            }));
        }


        



        private void test_init()
        {
            //LaserSensor laserSensor = new LaserSensor();
            //laserSensor.M = M_update;
            //laserSensor.D = D_update;
            //laserSensor.Start();

            
        }



        


        // ===================================================================================================
        // =========================================== 图片生成 ==============================================
        // ===================================================================================================

        // ================= 生成按键回调 ==================
        private void button_Generate_Click(object sender, EventArgs e)
        {

            if (tabControl1.SelectedTab == tabPage1)     // 也可以按 Name 判断
            {
                // 清空文件夹
                Array.ForEach(Directory.GetFiles(sliceSavePath), File.Delete);
                // 针对第一页的逻辑
                // 获取填入数据
                //if (!string.IsNullOrEmpty(textBox_height.Text))
                var info = new trainInfo
                {
                    // 模版1字符串
                    payLoad = textBox_payLoad.Text,
                    tareWeight = textBox_tareWeight.Text,
                    volume = textBox_volume.Text,

                    // 模版2字符串
                    length = textBox_length.Text,
                    width = textBox_width.Text,
                    height = textBox_height.Text,
                    trans = textBox_trans.Text,
                };

                // 获取按键是否旋转
                //bool _Rotate = checkBox_Rotate.Checked;
                bool _Rotate = radioButton_v.Checked;
                // 获取选择的模版选项
                Template tpl = (Template)comboBoxTemplate.SelectedItem;



                // 生成图片
                var bmp = TextRender.BmpRender(info, tpl, _Rotate, sliceHight, sliceSavePath);



                // 显示在UI上预览
                dispose_pictureBox(pictureBox_Preview);
                pictureBox_Preview.Image = bmp;

                // 保存到文件夹
                nameAdd();
                ImageProcessor.SaveImageToDisk(bmp, name, savePath);

            }

            else if (tabControl1.SelectedTab == tabPage2)
            {
                // 清空文件夹
                Array.ForEach(Directory.GetFiles(sliceSavePath), File.Delete);
                // 获取模版3：图片模版
                Template tpl = new TemplateManager()
                    .GetAllTemplates()
                    .Find(t => t.Name == "路徽");
                // 确认是否旋转 true为旋转
                bool _Rotate = radioButton_v1.Checked;

                string fileName = comboBox_filePicture.SelectedItem.ToString();
                string fullPath = Path.Combine(loadPath, fileName);
                if (File.Exists(fullPath))
                {
                    try
                    {
                        dispose_pictureBox(pictureBox_Preview);

                        using (var stream = new MemoryStream(File.ReadAllBytes(fullPath)))
                        {
                            // 从 MemoryStream 创建 Bitmap 对象
                            Bitmap img = (Bitmap)Image.FromStream(stream);

                            // 得到文件夹提前画好的 1bpp 图片
                            // 转换成24bpp
                            img = ImageProcessor.Convert1bppTo24bpp(img);
                            ImageProcessor.RotateImg(img, _Rotate);
                            if (_Rotate) ImageProcessor.v_SliceBmp(img, tpl, sliceHight, sliceSavePath);
                            else ImageProcessor.h_SliceBmp(img, tpl, sliceHight, sliceSavePath);
                            img = ImageProcessor.Convert24bppTo1bpp(img);

                            pictureBox_Preview.Image = img;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("加载图片失败: " + ex.Message);
                    }
                }
            }
            
        }


        // ================= 选择图片回调 ==================
        private void comboBox_filePicture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage2)
            {
                string fileName = comboBox_filePicture.SelectedItem.ToString();
                string fullPath = Path.Combine(loadPath, fileName);

                if (File.Exists(fullPath))
                {
                    try
                    {
                        dispose_pictureBox(pictureBox_Preview);
                        // 使用 MemoryStream 加载图片，避免文件锁定问题
                        using (var stream = new MemoryStream(File.ReadAllBytes(fullPath)))
                        {
                            // 从 MemoryStream 创建 Bitmap 对象
                            Image img = Image.FromStream(stream);

                            // 将新图片赋值给 PictureBox
                            pictureBox_Preview.Image = img;
                            pictureBox_Preview.SizeMode = PictureBoxSizeMode.Zoom; // 设置缩放模式
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("加载图片失败: " + ex.Message);
                    }
                }
            }
        }

        // 辅助函数：释放图片预览资源
        private static void dispose_pictureBox(PictureBox box)
        {
            if (box.Image != null)
            {
                box.Image.Dispose();
                box.Image = null;
            }
        }

        // 辅助函数：文件命名
        private string nameAdd()
        {
            count++;
            name = count.ToString("D2");
            return name;
        }

       



        // ===================================================================================================
        // ============================================= Socket ==============================================
        // ===================================================================================================
        SocketClient client9837;
        SocketClient client8080;

        // ================= 初始化socket ==================
        private void InitializeClients()
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

        // ================== 连接按钮 ==================
        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            string ip = (string)comboBox_ip.SelectedItem;

            client9837.Connect(ip, 9837); // 连控制口
            client8080.Connect(ip, 8080); // 连数据口
            //MessageBox.Show(ip);
        }

        // ================== 指令发送按钮 (发给9837) ==================
        private void btn9837Send_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textInput.Text))
                client9837.Send(textInput.Text);
        }

        // ================== 数据发送按钮 (发给8080) ==================
        private void btn8080Send_Click(object sender, EventArgs e)
        {
            // 获取输入框内容直接发送
            if (!string.IsNullOrEmpty(textInput.Text))
                client8080.Send(textInput.Text);
        }


         private void Show9837(string msg)
        {
            this.BeginInvoke(new Action(() =>
            {
                rtb9837Log.AppendText(msg);
                rtb9837Log.ScrollToCaret();
            }));  
        }

        private void Show8080(string msg)
        {
            this.BeginInvoke(new Action( ()=>
            {
                    rtb8080Log.AppendText(msg); // 显示文本
                    rtb8080Log.ScrollToCaret(); // 滚到底部
            }));
        }

        // ================= 主动关闭socket ==================
        private void btnClose_Click(object sender, EventArgs e)
        {  
            client8080.Close();
            Show8080(DateTime.Now.ToString("yy-MM-dd hh:mm:ss ") + "已关闭连接" + "\r\n");
            client9837.Close();
            Show9837(DateTime.Now.ToString("yy-MM-dd hh:mm:ss ") + "已关闭连接" + "\r\n");
            btnConnect.Enabled = true;
        }
        // ================= 服务端关闭socket ==================
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

        // ================= 清空log ==================
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
  
        // ================= 连接机械臂 ==================
        private void btnArmInit_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Arm.armInit();
            });

        }
        // ================= 机械臂急停 ==================
        private void btnStop_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Arm.rm_set_arm_stop(Arm.Instance.robotHandlePtr);
            });
        }

        // ================= 机械臂开关喷印系统触发 ==================
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
        private void trackBar_speed_Scroll(object sender, EventArgs e)
        {
            label_speed.Text = trackBar_speed.Value.ToString();
        }

        private async void btn_start_Click(object sender, EventArgs e)
        {
            btn_start.Enabled = false;
            if (string.IsNullOrEmpty(textBox_UVheight.Text))
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
            // 
            // 机械臂末端离平面高度，单位 m
            float height = float.Parse(textBox_UVheight.Text) / 100;


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
        private void trackBar_H_speed_Scroll(object sender, EventArgs e)
        {
            label_H_speed.Text = trackBar_H_speed.Value.ToString();
        }

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
        private void btn_laserStop_Click(object sender, EventArgs e)
        {
            laserSensor.Start();
        }
        private void btn_laserStart_Click(object sender, EventArgs e)
        {
            laserSensor.Stop();
        }


        //Arm.test2(Arm.Instance.robotHandlePtr);

        // ===================================================================================================
        // ========================================= 深度相机画面 ============================================
        // ===================================================================================================
        CameraService cam = new CameraService();
        private Bitmap imagecolor;
        private Bitmap imagedepth;
        private float alpha = 0.4f;

        // 相机启动
        private void btn_camSTART_Click(object sender, EventArgs e)
        {
            cam.camAction = Cam_Update;
            cam.cam_Thread_start();
        }
        // 相机关闭
        private void btn_camSTOP_Click(object sender, EventArgs e)
        {
            cam.cam_Thread_stop();
        }

        // 更新相机画面
        private void Cam_Update(Bitmap color, Bitmap depth)
        {
            this.BeginInvoke(new Action(() =>
            {
                imagecolor?.Dispose();
                imagedepth?.Dispose();

                imagecolor = color;
                imagedepth = depth;

                panel1.Invalidate();
            }));
        }
        
        //Bitmap tempIMG;//' 全局变量
        //private void picbox(Bitmap img)
        //{
            
        //        this.BeginInvoke(new Action(() =>
        //        {
        //            pictureBox_Preview.Image = img;
        //            tempIMG = new Bitmap(img);
        //        }));
                       
        //}


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (imagecolor != null)
                g.DrawImage(imagecolor, 0, 0, panel1.Width, panel1.Height);

            if (imagedepth != null)
            {
                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = alpha;

                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                Rectangle rect = new Rectangle(0, 0, panel1.Width, panel1.Height);

                g.DrawImage(imagedepth, rect, 0, 0, imagedepth.Width, imagedepth.Height,
                    GraphicsUnit.Pixel, ia);
            }
        }

        
    }

}


