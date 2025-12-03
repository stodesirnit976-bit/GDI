using GDI.Core;
using GDI.Models;
using GDI.Services;
using GDI.Services.CameraServices;
using GDI.UI;
using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace GDI
{
    public partial class MainForm : Form
    {
        private Form1 detailForm = null;

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



        public MainForm()
        {
            InitializeComponent();

            // 初始化,让PictureBox适应图片大小
            pictureBox_Preview.SizeMode = PictureBoxSizeMode.Zoom;

            // panel开启双缓冲防闪烁
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });

            // 初始化命名数
            count = 0;

            // 初始化详情页面
            detailForm = new Form1();

            Console.WriteLine($"{Arg.laserDistance} || {Arg.c2PositionX} || {Arg.c2PositionY}");
        }

            

        // -------- 窗口加载事件：初始化下拉框 ------------
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 文字的combobox导入
            templates = templateManager.GetAllTemplates();//把所有模版放入结构模版列表
            comboBoxTemplate.DataSource = templates.Where(t => t.Name.Contains("模板")).ToList();
            comboBoxTemplate.DisplayMember = "Name";

            // 图像的combobox导入
            PictureListLoader.Load(loadPath, comboBox_filePicture);

           
        }

        // ----------- 窗口关闭事件：关闭服务 ------------
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 可以加个判断逻辑：如果机械臂没有回到初始位置， 
            
            // 以下的关闭需要确认是否有句柄、实例对象的存在，如果没有直接跳过
            
            // 关闭socket服务
            detailForm.socket_close();
            // 关闭相机服务
            cam_stop();
            // 关闭激光测距传感器
            detailForm.laser_Stop();
            // 断开机械臂并恢复初始化姿态

            // 关闭喷印系统触发io?要看这里具体怎么触发，调整


            detailForm.Dispose();

            Thread.Sleep(500);
            MessageBox.Show("系统已安全关闭！");
        }

        // -------- 机械臂急停按钮 --------
        private void btn_EStop_Click(object sender, EventArgs e)
        {
            //detailForm.arm_EStop();
            Arm.rm_set_arm_pause(Arm.Instance.robotHandlePtr);
            Arm.rm_set_arm_delete_trajectory(Arm.Instance.robotHandlePtr);

            Arm.backTOInitState();
            Console.WriteLine("机械臂手动急停完成");
        }
        


        // ===================================================================================================
        // =========================================== TEST ==================================================
        // ===================================================================================================
        
        

        private void btn_Wrok_Click(object sender, EventArgs e)
        {
            arm_Start();
        }
        
        // ===================================================================================================
        // =========================================== 后台详情 ==============================================
        // ===================================================================================================
        private void btn_Detail_Click(object sender, EventArgs e)
        {
            if (detailForm == null || detailForm.IsDisposed)
            {
                detailForm = new Form1();
            }
            detailForm.Show(); 
            detailForm.BringToFront();
        }




        // ===================================================================================================
        // =========================================== 系统初始化 ============================================
        // ===================================================================================================
        private async void btn_SysInit_Click(object sender, EventArgs e)
        {
            LoadingForm loadingForm = new LoadingForm();

            if (detailForm == null || detailForm.IsDisposed)
            {
                detailForm = new Form1();
                var handle = detailForm.Handle; // 强制创建句柄
            }

            // 初始化进度条
            btn_SysInit.Enabled = false;
           


            // 初始化机械臂并恢复初始化姿态
            int ret = detailForm.arm_Init();

            // 机械臂IO控制器初始化
            Arm.rm_set_voltage(Arm.Instance.robotHandlePtr, 3);
            Arm.rm_set_IO_mode(Arm.Instance.robotHandlePtr, 1, 1);// 设置为通用输出模式,初始化为低电平
            int a = Arm.rm_set_DO_state(Arm.Instance.robotHandlePtr, 1, 0);// 设置1号端口输出低电平

            //Console.WriteLine($"2号口设置为输出模式低电平{a}");

            // 启动socket服务
            // 默认初始化只连接9837端口，8080靠9837发送命令启动/关闭
            //detailForm.socket9837_Connect();
            //detailForm.socket8080_Connect();
            // 启动激光测距传感器
            //detailForm.laser_Start();
            // 等待机械臂初始化完成
            // 启动相机服务并标定
            if (ret == 6)
                cam_Start();

            // 显示加载进度条窗口
            //loadingForm.ShowDialog(this);
            //MessageBox.Show("系统初始化完成！");  

            btn_SysInit.Enabled = true;
        }


        private void btn_RESEAT_Click(object sender, EventArgs e)
        {
            //detailForm.socket_close();
            // 关闭相机服务
            cam_stop();
            // 关闭激光测距传感器
            //detailForm.laser_Stop();
            // 断开机械臂并恢复初始化姿态
            MessageBox.Show("系统已重置！");
            cam_Start();

        }


        // ===================================================================================================
        // =========================================== 图片生成 ==============================================
        // ===================================================================================================

        // -------- 生成按键回调 --------
        private void btn_Generate_Click(object sender, EventArgs e)
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
                bool _Rotate = rbt_OFF.Checked;
                // 获取选择的模版选项
                Template tpl = (Template)comboBoxTemplate.SelectedItem;

                // 生成图片
                Bitmap bmp = TextRender.BmpRender(info, tpl, _Rotate, sliceHight, sliceSavePath);

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

                        Bitmap img = TextRender.fileImgRender(fullPath, tpl, _Rotate, sliceHight, sliceSavePath);

                        pictureBox_Preview.Image = img;                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("加载图片失败: " + ex.Message);
                    }
                }
            }


            // 图片生成完成后？
            // 9837发送指令打开喷印系统->连接8080端口->
            // （socket自动发送文件路径）
            // 机械臂启动 ->机械臂运动到位，io触发喷印
            // 喷印结束，9837发送指令关闭喷印系统
            // 喷印下一张图片，重复上述过程

        }

        private void logic_func()
        {
            detailForm.socket9837_Send("@StartPrint@");

            detailForm.socket8080_Connect();
            
            // 机械臂启动
            arm_Start();

            // 怎么确定喷印结束？阻塞？不好急停不靠谱；有没有类似信号量任务通知的机制？autoreseevent
            
                SyncOjbects.armFinsh.WaitOne();
                detailForm.socket9837_Send("@StopPrint@");
                      
        }


        // -------- tab2里选择图片回调 --------
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
        // ========================================= 深度相机画面 ============================================
        // ===================================================================================================
        Camera cam = new Camera();
        Calibration calib = new Calibration();

        private Bitmap imagecolor;
        private Bitmap imagedepth;
        private float alpha = 0.25f;

        // -------- 相机启动 --------
        private async Task cam_Start()
        {
            cam.cam_Thread_start();
            // 订阅相机事件，获取画面
            cam.cam_Event += panel_Update;

            await Task.Run(async () =>
            {
                // 订阅相机事件，进行标定
                calib.Calibration_subCamEvent(cam);
                //await Task.Delay(20000);
                //Thread.Sleep(23000);//wm修改
                //calib.Calibration2_subCamEvent(cam);      //wm修改
            });
        }
        // -------- 相机关闭 --------
        private void cam_stop()
        {
            cam.cam_Thread_stop();
        }

        // -------- 委托回调更新相机画面 --------
        private void panel_Update(Bitmap color, Bitmap depth, DepthFrame a, Intrinsics b)
        {
            Bitmap colorCopy = new Bitmap(color);
            Bitmap depthCopy = new Bitmap(depth);
            this.BeginInvoke(new Action(() =>
            {
                imagecolor?.Dispose();
                imagedepth?.Dispose();

                imagecolor = colorCopy;
                imagedepth = depthCopy;

                panel1.Invalidate();
            }));
        }

        // -------- 面板重绘 --------
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



        // ===================================================================================================
        // ========================================= 机械臂启动 ==============================================
        // ===================================================================================================

        // -------- 机械臂启动 --------
        public async void arm_Start()
        {
            var p = GetArmParams.Params(sliceSavePath, tbx_UVheight.Text, rbt_OFF.Checked, rbt_PrintON.Checked);

            if (p == null)
            {
                MessageBox.Show("请输入机械臂工作高度！");
                return;
            }
            else
                Console.WriteLine($"机械臂运动参数获取成功：len={p.Len}, wid={p.Wid}, height={p.Height}, N={p.N}, count={p.Count}, vol={p.Vol}, Print={p.ON}");

            await Task.Run(() =>
            {
                try
                {
                    // 机械臂执行操作
                    Arm.Instance.move(p.Len, p.Wid, p.Height, p.N, p.Count, p.Vol, p.ON);///////////这里长度加了10mm抵消延迟机械臂移动距离不够
                    Console.WriteLine("机械臂开始运行");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("机械臂启动失败: " + ex.Message);
                }
            });
            Console.WriteLine("发送运行结束信号");
            SyncOjbects.armFinsh.Set(); // 机械臂运行结束信号
        }

        
    }
}
