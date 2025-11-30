using GDI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace GDI.Core
{
    internal class ImageProcessor
    {
        /// <summary>
        /// 转换图片，传入24Bpp图片返回1Bpp图片
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Bitmap Convert24bppTo1bpp(Bitmap source)
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
        public static Bitmap Convert1bppTo24bpp(Bitmap src)
        {
            if (src.PixelFormat != PixelFormat.Format1bppIndexed)
                throw new ArgumentException("不是 1bpp 的图");

            // 克隆换格式，内部用 GDI+ 原生加速
            return src.Clone(new Rectangle(0, 0, src.Width, src.Height),
                             PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// 旋转图片，内部会转换成32bpp，所以旋转对象最好为24bpp
        public static void RotateImg(Bitmap bmp, bool rotate)
        {
            if (rotate)
                bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }


        /// 横图切24Bpp图片并保存为1Bpp，原图未做额外处理
        public static void h_SliceBmp(Bitmap rawbitmap, Template tpl, int sliceHight, string path)
        {
            // 每张新图切割从01开始
            int count = 0;
            string name;
            // 一共要切几片
            int cnt = (tpl.Height + sliceHight-1) / sliceHight;

            for (int i = 0; i < cnt; i++)
            {
                int y = i * sliceHight;
                // 最后一张切割图片可能高度不够，如230-50*4 = 30
                int realHight = Math.Min(sliceHight, tpl.Height - y);

                using (Bitmap bmp = new Bitmap(tpl.Width, realHight, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);

                        g.DrawImage(rawbitmap, new Rectangle(0, 0, tpl.Width, realHight), new Rectangle(0, y, tpl.Width, realHight), GraphicsUnit.Pixel);
                        // 转换成1Bpp
                        Bitmap slicebmp = Convert24bppTo1bpp(bmp);

                        count++;
                        name = count.ToString("D2");

                        SaveImageToDisk(slicebmp, name, path);
                        slicebmp.Dispose();
                    }
                }
            }
        }
        /// 竖图切24Bpp图片并保存为1Bpp，原图未做额外处理
        public static void v_SliceBmp(Bitmap rawbitmap, Template tpl, int sliceHight, string path)
        {
            // 每张新图切割从01开始
            int count = 0;
            string name;
            // 一共要切几片
            int cnt = (tpl.Width + sliceHight - 1) / sliceHight;

            for (int i = 0; i < cnt; i++)
            {
                int y = i * sliceHight;
                // 最后一张切割图片可能高度不够，如230-50*4 = 30
                int realHight = Math.Min(sliceHight, tpl.Width - y);

                using (Bitmap bmp = new Bitmap(tpl.Height, sliceHight, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);

                        g.DrawImage(rawbitmap, new Rectangle(0, 0, tpl.Height, realHight), new Rectangle(0, y, tpl.Height, realHight), GraphicsUnit.Pixel);
                       
                        // 转换成1Bpp
                        Bitmap slicebmp = Convert24bppTo1bpp(bmp);

                        count++;
                        name = count.ToString("D2");

                        SaveImageToDisk(slicebmp, name, path);
                        slicebmp.Dispose();
                    }
                }
            }
        }



        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="fileName"></param>
        /// <param name="dirPath"></param>
        public static void SaveImageToDisk(Bitmap bmp, string fileName, string dirPath)
        {
            // 检查文件夹是否存在，不存在则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            // 拼接文件名，防止文件名包含非法字符
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            string fullPath = Path.Combine(dirPath, fileName + ".bmp");

            // 保存为bmp格式
            bmp.Save(fullPath, ImageFormat.Bmp);

            if(!dirPath.Contains("SliceImages"))
                MessageBox.Show($"图片已生成:\n{fullPath}");
        }
     
    }

}

/*

       // ======= 绘制模版 ======
       public static Bitmap drawTemplate1(string payload, string tareweight, string volume, Template tpl)
       {
           float currentX = 0;
           float currentY = 0;

           // 1. 创建画布(Bitmap)
           Bitmap Bmp = new Bitmap(tpl.Width, tpl.Height, PixelFormat.Format24bppRgb);
           // 2. 创建画笔并绘制(Graphics)
           using (Graphics g = Graphics.FromImage(Bmp))
           {
               // 设置绘图质量，抗锯齿
               g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
               // 去除前端自动留白，定格0,0
               StringFormat sf = StringFormat.GenericTypographic;
               // 确保末尾空格也被计算宽度
               sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
               // 填充背景色 (白色背景)
               g.Clear(Color.White);
               // 定义画笔颜色 (黑色文字)
               Brush brush = Brushes.Black;

               Font font = new Font("宋体", tpl.BigFont, GraphicsUnit.Pixel);


               // ========== 先画模版 ==========



               // 绘制当前一行的字符串


                   g.DrawString("载", font, brush, 0, 0, sf);
                   g.DrawString("重", font, brush, 1649 + 375, 0, sf);

                   g.DrawString("自", font, brush, 0, 1649, sf);
                   g.DrawString("重", font, brush, 1649 + 375, 1649, sf);

                   g.DrawString("容", font, brush, 0, 1649 * 2, sf);
                   g.DrawString("积", font, brush, 1649 + 375, 1649*2, sf);



               //g.DrawString();
               // 算出当前这个字号基线到顶部的距离 
               // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
               // 为了保证对齐，需要实现的是文字底边对齐而不是字框底边对齐，算出"大字"的基线位置作为标准,

               // 调用 ParseText 根据规则确定字体font

               //.Concat(TextRender.ParseText(tareweight, tpl.BigFont, tpl.SmallFont))
               //.Concat(TextRender.ParseText(volume, tpl.BigFont, tpl.SmallFont))
               //.ToList();

               FontFamily f = new FontFamily("宋体");
               float bigAscent = tpl.BigFont * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);

               //float smallAscent = tpl.SmallFont * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);

               //float ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
               // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
               //float Big_BaseY = currentY + (bigAscent - bigAscent);
               //float Small_BaseY = currentY + (bigAscent - smallAscent);

               float ascent = 0;
               float yBaseline = 0;
               float small_ascent = tpl.SmallFont * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);

               SizeF wsf = g.MeasureString(payload, font);


               payload += "t";
               tareweight += "t";
               volume += "m³";

               List<TextChunk> chunks = TextRender.ParseText(payload, tpl.BigFont, tpl.SmallFont);
               currentX = 1649 * 2 + 6400;
               foreach (var chunk in chunks)
               {
                   using (Font font1 = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
                   {
                       // 可以把这个底部baseline写成一个static工具函数
                       ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                       // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                       yBaseline = currentY + (bigAscent - ascent);
                       g.DrawString(chunk.Text, font1, brush, currentX, yBaseline, sf);//没考虑数字之间的间距还有数字和单位之间的间距
                       // 移动x位置
                       SizeF size = g.MeasureString(chunk.Text, font1, PointF.Empty, sf);
                       currentX += (size.Width + tpl.CharSpacing);
                   }   
               }
               // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
               //float ySmall_Baseline = currentY + (bigAscent - small_ascent);
               //g.DrawString("t", new Font("宋体", tpl.SmallFont, GraphicsUnit.Pixel), brush, currentX, ySmall_Baseline, sf);
               //这是法1很麻烦，可以考虑在获得了payload后先把单位t拼进payload:  payload += "t";

               currentY += (tpl.BigFont + tpl.LineSpacing);

               chunks = TextRender.ParseText(tareweight, tpl.BigFont, tpl.SmallFont);
               currentX = 1649 * 2 + 6400;
               foreach (var chunk in chunks)
               {
                   using (Font font2 = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
                   {
                       ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                       // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                       yBaseline = currentY + (bigAscent - ascent);
                       g.DrawString(chunk.Text, font2, brush, currentX, yBaseline, sf);//没考虑数字之间的间距还有数字和单位之间的间距
                       // 移动x位置
                       SizeF size = g.MeasureString(chunk.Text, font2, PointF.Empty, sf);
                       currentX += (size.Width + tpl.CharSpacing);
                   }
               }


               currentY += (tpl.BigFont + tpl.LineSpacing);

               chunks = TextRender.ParseText(volume, tpl.BigFont, tpl.SmallFont);
               currentX = 1649 * 2 + 6400;
               foreach (var chunk in chunks)
               {
                   using (Font font2 = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
                   {
                       ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                       // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                       yBaseline = currentY + (bigAscent - ascent);
                       g.DrawString(chunk.Text, font2, brush, currentX, yBaseline, sf);//没考虑数字之间的间距还有数字和单位之间的间距
                       // 移动x位置
                       SizeF size = g.MeasureString(chunk.Text, font2, PointF.Empty, sf);
                       currentX += (size.Width + tpl.CharSpacing);
                   }
               }

               return Bmp;
           }
       }

       /// <summary>
       /// ======= 绘制 ======
       /// </summary>
       /// <param name="info"></param>
       /// <param name="tpl"></param>
       /// <returns></returns>
       public static Bitmap drawTemplate(trainInfo info, Template tpl)
       {
           float currentX = 0;
           float currentY = 0;

           // 1. 创建画布(Bitmap)
           Bitmap Bmp = new Bitmap(tpl.Width, tpl.Height, PixelFormat.Format24bppRgb);
           // 2. 创建画笔并绘制(Graphics)
           using (Graphics g = Graphics.FromImage(Bmp))
           {
               // 设置绘图质量，抗锯齿
               // 去除前端自动留白，定格0,0
               // 确保末尾空格也被计算宽度
               // 填充背景色 (白色背景)
               // 定义画笔颜色 (黑色文字)
               g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;               
               StringFormat sf = StringFormat.GenericTypographic;               
               sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;                
               g.Clear(Color.White);               
               Brush brush = Brushes.Black;

               Font font = new Font("宋体", tpl.BigFont, GraphicsUnit.Pixel);
               FontFamily f = new FontFamily("宋体");
               float bigAscent = tpl.BigFont * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);

               // =========== 以下由于两种模版固定的汉字不同，需要手动drawString
               // ========== 先画模版 ==========
               switch (tpl.Name)
               {
                   case "模板1":
                       {
                           g.DrawString("载", font, brush, 0, 0, sf);
                           g.DrawString("重", font, brush, 1649 + 353, 0, sf);

                           g.DrawString("自", font, brush, 0, 1649 + tpl.LineSpacing, sf);
                           g.DrawString("重", font, brush, 1649 + 353, 1649+tpl.LineSpacing, sf);

                           g.DrawString("容", font, brush, 0, (1649 + tpl.LineSpacing)*2, sf);
                           g.DrawString("积", font, brush, 1649 + 353, (1649 + tpl.LineSpacing) * 2, sf);

                           float ascent = 0;//Font对应的实际像素高度
                           float yBaseline = 0;
                           float small_ascent = tpl.SmallFont * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);

                           info.payLoad += "t";
                           info.tareWeight += "t";
                           info.volume += "m³";

                           //第一行
                           List<TextChunk> chunks = TextRender.ParseText(info.payLoad, tpl.BigFont, tpl.SmallFont);
                           currentX = 1649 * 2 + 6400;
                           foreach (var chunk in chunks)
                           {
                               using (Font font1 = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
                               {
                                   // 可以把这个底部baseline写成一个static工具函数
                                   ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                                   // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                                   yBaseline = currentY + (bigAscent - ascent);
                                   g.DrawString(chunk.Text, font1, brush, currentX, yBaseline, sf);//没考虑数字之间的间距还有数字和单位之间的间距
                                                                                                   // 移动x位置
                                   SizeF size = g.MeasureString(chunk.Text, font1, PointF.Empty, sf);
                                   currentX += (size.Width + tpl.CharSpacing);
                               }
                           }
                           //第二行
                           currentY += (tpl.BigFont + tpl.LineSpacing);
                           chunks = TextRender.ParseText(info.tareWeight, tpl.BigFont, tpl.SmallFont);
                           currentX = 1649 * 2 + 6400;
                           foreach (var chunk in chunks)
                           {
                               using (Font font2 = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
                               {
                                   ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                                   // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                                   yBaseline = currentY + (bigAscent - ascent);
                                   g.DrawString(chunk.Text, font2, brush, currentX, yBaseline, sf);//没考虑数字之间的间距还有数字和单位之间的间距
                                                                                                   // 移动x位置
                                   SizeF size = g.MeasureString(chunk.Text, font2, PointF.Empty, sf);
                                   currentX += (size.Width + tpl.CharSpacing);
                               }
                           }
                           //第三行
                           currentY += (tpl.BigFont + tpl.LineSpacing);
                           chunks = TextRender.ParseText(info.volume, tpl.BigFont, tpl.SmallFont);
                           currentX = 1649 * 2 + 6400;
                           foreach (var chunk in chunks)
                           {
                               using (Font font2 = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
                               {
                                   ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                                   // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                                   yBaseline = currentY + (bigAscent - ascent);
                                   g.DrawString(chunk.Text, font2, brush, currentX, yBaseline, sf);//没考虑数字之间的间距还有数字和单位之间的间距
                                                                                                   // 移动x位置
                                   SizeF size = g.MeasureString(chunk.Text, font2, PointF.Empty, sf);
                                   currentX += (size.Width + tpl.CharSpacing);
                               }
                           }

                           return Bmp;
                       }

                   case "模板2":
                       {
                           g.DrawString("换", font, brush, 0, (1649 + tpl.LineSpacing), sf);
                           g.DrawString("长", font, brush, 1649 + tpl.CharSpacing, (1649 + tpl.LineSpacing), sf);
                           info.length += "×";
                           info.width += "×";
                           //info.height += "×";
                           string trainSize = info.length + info.width + info.height;

                           float ascent = 0;//Font对应的实际像素高度
                           float yBaseline = 0; //不同大小的文字、数字像素基线对齐

                           //第一行
                           List<TextChunk> chunks = TextRender.ParseText(trainSize, tpl.BigFont, tpl.SmallFont);
                           currentX = 0;
                           foreach (var chunk in chunks)
                           {
                               using (Font font1 = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
                               {
                                   // 可以把这个底部baseline写成一个static工具函数
                                   ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                                   // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                                   yBaseline = currentY + (bigAscent - ascent);
                                   g.DrawString(chunk.Text, font1, brush, currentX, yBaseline, sf);//没考虑数字之间的间距还有数字和单位之间的间距
                                                                                                   // 移动x位置
                                   SizeF size = g.MeasureString(chunk.Text, font1, PointF.Empty, sf);
                                   currentX += (size.Width + tpl.CharSpacing);
                               }
                           }
                           //第二行
                           currentY += (tpl.BigFont + tpl.LineSpacing);
                           chunks = TextRender.ParseText(info.trans, tpl.BigFont, tpl.SmallFont);
                           currentX = 1649 * 2 + 6400;
                           foreach (var chunk in chunks)
                           {
                               using (Font font2 = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
                               {
                                   ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                                   // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                                   yBaseline = currentY + (bigAscent - ascent);
                                   g.DrawString(chunk.Text, font2, brush, currentX, yBaseline, sf);//没考虑数字之间的间距还有数字和单位之间的间距
                                                                                                   // 移动x位置
                                   SizeF size = g.MeasureString(chunk.Text, font2, PointF.Empty, sf);
                                   currentX += (size.Width + tpl.CharSpacing);
                               }
                           }
                           return Bmp;
                       }
               }                
           }
           return null;
       }

       /*
       private static void draw(List<TextChunk> chunks, string str, Template tpl, float ascent, FontFamily f, float currentY, float yBaseline, )
       {
           chunks = TextRender.ParseText(str, tpl.BigFont, tpl.SmallFont);
           currentX = 1649 * 2 + 6400;
           foreach (var chunk in chunks)
           {
               using (Font font2 = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
               {
                   ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                   // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                   yBaseline = currentY + (bigAscent - ascent);
                   g.DrawString(chunk.Text, font2, brush, currentX, yBaseline, sf);//没考虑数字之间的间距还有数字和单位之间的间距
                                                                                   // 移动x位置
                   SizeF size = g.MeasureString(chunk.Text, font2, PointF.Empty, sf);
                   currentX += (size.Width + tpl.CharSpacing);
               }
           }
       }
       */