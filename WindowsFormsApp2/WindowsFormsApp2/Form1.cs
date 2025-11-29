using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        string inputPath = @"D:\TextImages\1.png";
        string outputPath = @"D:\TextImages\image\2.bmp";
        public Bitmap fileImgRender(string fullpath, bool rotate, int sliceHight, string sliceSavePath)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(fullpath)))
            {
                // 从 MemoryStream 创建 Bitmap 对象
                Bitmap img = (Bitmap)Image.FromStream(stream);

                

                return null;
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            int targetWidth = 3534;
            int targetHeight = 3534;

            // 1. 检查并创建输出目录（防止目录不存在报错）
            string dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Image originalImage = null;

            try
            {
                // 2. 安全读取图片
                // 不直接使用 new Bitmap(path)，因为那样会锁定文件。
                // 改为先读入内存流，这样读完后文件立刻解锁，即使 inputPath == outputPath 也没问题。
                byte[] imageBytes = File.ReadAllBytes(inputPath);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    originalImage = Image.FromStream(ms);

                    using (Bitmap resizedImage = new Bitmap(targetWidth, targetHeight, PixelFormat.Format24bppRgb))
                    {
                        // 设置分辨率（DPI）
                        resizedImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

                        using (Graphics g = Graphics.FromImage(resizedImage))
                        {
                            // 高质量插值设置
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.CompositingQuality = CompositingQuality.HighQuality;

                            // 绘制
                            g.DrawImage(originalImage, new Rectangle(0, 0, targetWidth, targetHeight),
                                        new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                                        GraphicsUnit.Pixel);
                        }

                        // 3. 【关键修复】安全保存
                        // 有时候 GDI+ 直接 Save 到文件路径会因为权限或句柄问题报错
                        // 这里建议先保存到内存流，再用 FileStream 写入
                        using (MemoryStream outStream = new MemoryStream())
                        {
                            Bitmap b = Convert24bppTo1bpp(resizedImage);
                            b.Save(outStream, ImageFormat.Bmp);

                            // 显式写入文件
                            File.WriteAllBytes(outputPath, outStream.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 捕获真正的错误信息
                Console.WriteLine($"处理失败: {ex.Message}");
                throw;
            }
            finally
            {
                // 确保释放原图资源
                if (originalImage != null) originalImage.Dispose();
            }
        
        }


        public  Bitmap Convert24bppTo1bpp(Bitmap source)
        {
            int w = source.Width;
            int h = source.Height;

            // 创建1bpp图
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            // 锁定原图和目标图
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            // lockBits返回的是原生(非托管)内存地址(指针),为了不用unsafe指针，
            // 通过Marshal.Copy把非托管的内存搬运到托管数组中，也就是byte[] srcBuffer
            int srcStride = srcData.Stride;
            byte[] srcBuffer = new byte[srcStride * h];
            System.Runtime.InteropServices.Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBuffer.Length);

            int dstStride = dstData.Stride;
            byte[] dstBuffer = new byte[dstStride * h];

            // 遍历像素
            for (int y = 0; y < h; y++)
            {
                int srcRow = y * srcStride;
                int dstRow = y * dstStride;

                for (int x = 0; x < w; x++)
                {
                    int pixelIndex = srcRow + x * 3;//每行的每个像素的偏移，24rgb，每像素有3字节（brg顺序）
                    byte b = srcBuffer[pixelIndex];
                    byte g = srcBuffer[pixelIndex + 1];
                    byte r = srcBuffer[pixelIndex + 2];
                    // 1bpp情况下1像素等于1bit，最小操作单位为byte = 8bit，
                    // 所以每行像素位++，对每byte里的bit依次设置掩码，实现bit++,掩码右移，每8次byte++
                    // 如果是白色，则把该位设置为 1。
                    if ((r + g + b) / 3 > 128) dstBuffer[dstRow + (x / 8)] |= (byte)(0x80 >> (x % 8));
                }
            }
            // 复制回Bitmap
            System.Runtime.InteropServices.Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
            bmp.UnlockBits(dstData);
            source.UnlockBits(srcData);

            return bmp;
        }


        /// 转换图片，传入 1Bpp 图片返回 24Bpp 图片
        public  Bitmap Convert1bppTo24bpp(Bitmap src)
        {
            //if (src.PixelFormat != PixelFormat.Format1bppIndexed)
                //throw new ArgumentException("不是 1bpp 的图");

            // 克隆换格式，内部用 GDI+ 原生加速
            return src.Clone(new Rectangle(0, 0, src.Width, src.Height),
                             PixelFormat.Format24bppRgb);
        }



    }
}
