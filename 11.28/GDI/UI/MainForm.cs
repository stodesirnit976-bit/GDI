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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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


            pictureBox_Preview.SizeMode = PictureBoxSizeMode.Zoom;// 初始化,让PictureBox适应图片大小

            // 双缓冲防闪烁
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });

            count = 0;
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
            // 可以加个判断逻辑：如果机械臂没有回到初始位置，提示用户先回初始位置再关闭系统
            

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
            detailForm.arm_EStop();
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
        private void btn_SysInit_Click(object sender, EventArgs e)
        {
            // 启动socket服务
            detailForm.socket_Start();
            // 启动相机服务
            cam_Start();
            // 启动激光测距传感器
            detailForm.laser_Start();
            // 初始化机械臂并恢复初始化姿态
            detailForm.arm_Init();           
            // 关闭喷印系统触发io?要看这里具体怎么触发，调整

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


        // -------- 选择图片回调 --------
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
        CameraService cam = new CameraService();
        private Bitmap imagecolor;
        private Bitmap imagedepth;
        private float alpha = 0.4f;

        // -------- 相机启动 --------
        private void cam_Start()
        {
            cam.camAction = Cam_Update;
            cam.cam_Thread_start();
        }
        // -------- 相机关闭 --------
        private void cam_stop()
        {
            cam.cam_Thread_stop();
        }

        // -------- 委托回调更新相机画面 --------
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










        
    }
}
