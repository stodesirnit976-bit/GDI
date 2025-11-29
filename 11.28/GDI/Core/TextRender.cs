using GDI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;

namespace GDI.Core
{
    public class TextRender
    {


        // ====================================================================
        // ========================  私有解析接口 =============================
        // ====================================================================
        // 状态枚举
        private enum State 
        {
            Start,
            Normal,      // 大字：正常数字、中文
            Alpha,       // 小字：英文单位
            Dot,         // 小字：小数点
            Decimal,     // 小字：小数位数字
            Symbol,      // 小字：乘号×，立方³...
        }
        // 解析函数，根据规则判断每一个字体的大小
        private static List<TextChunk> ParseText(string input, int big, int small)
        {
            List<TextChunk> result = new List<TextChunk>();
            if (string.IsNullOrEmpty(input)) return result;

            string buffer = "";
            State currentState = State.Start;

            // ============== 获取某个状态对应的字号 ==========
            int GetSize(State s)
            {
                if (s == State.Alpha || s == State.Dot || s == State.Decimal || s == State.Symbol) return small;
                else return big; // 开始默认为big
            }

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                State nextState = currentState;

                // =================================================
                // 【状态判断】
                // =================================================
                switch (currentState)
                {
                    case State.Start:
                        // 初始状态
                        if (char.IsDigit(c))        nextState = State.Normal;
                        else if (c == '.')          nextState = State.Dot;
                        else if (IsEnglish(c))      nextState = State.Alpha;
                        else if (IsSpecialUnit(c))  nextState = State.Symbol;
                        else                        nextState = State.Normal;
                        break;

                    case State.Normal:
                        // 当前是大字模式
                        if (char.IsDigit(c))        nextState = State.Normal;
                        else if (c == '.')          nextState = State.Dot;
                        else if (IsEnglish(c))      nextState = State.Alpha;
                        else if (IsSpecialUnit(c))  nextState = State.Symbol;
                        else                        nextState = State.Normal;
                        break;

                    case State.Alpha:
                        // 当前是英文模式
                        if (char.IsDigit(c)) nextState = State.Normal;
                        else if (c == '.') nextState = State.Dot;
                        else if (IsEnglish(c)) nextState = State.Alpha;
                        else if (IsSpecialUnit(c)) nextState = State.Symbol;
                        else nextState = State.Normal;
                        break;

                    case State.Dot:
                        // 当前刚输完小数点
                        if (char.IsDigit(c)) nextState = State.Decimal;
                        else if (c == '.') nextState = State.Dot;
                        else if (IsEnglish(c)) nextState = State.Alpha;
                        else if (IsSpecialUnit(c)) nextState = State.Symbol;
                        else nextState = State.Normal;
                        break;

                    case State.Decimal:
                        // 当前正在输入小数位数字
                        if (char.IsDigit(c)) nextState = State.Decimal;
                        else if (c == '.') nextState = State.Dot;
                        else if (IsEnglish(c)) nextState = State.Alpha;
                        else if (IsSpecialUnit(c)) nextState = State.Symbol;
                        else nextState = State.Normal;
                        break;

                    case State.Symbol:
                        // 当前正在输入特殊符号，暂时只有×和³，默认后续是大字符
                        if (char.IsDigit(c)) nextState = State.Normal;
                        //else if (c == '.')          nextState = State.Dot;
                        //else if (IsEnglish(c))      nextState = State.Alpha;
                        //else if (IsSpecialUnit(c))  nextState = State.Symbol;
                        else nextState = State.Normal;
                        break;
                }

                // =================================================
                // 【根据状态执行】
                // =================================================
                int currentSize = GetSize(currentState);
                int nextSize = GetSize(nextState);
                // 如果下一个字符大小变了，就把上一个字符存入textchunk
                if (currentSize != nextSize)
                {
                    result.Add(new TextChunk { Text = buffer, FontSize = currentSize });
                    buffer = "";//清空buffer
                }
                buffer += c;
                currentState = nextState;
            }
            // 循环结束，把缓冲区剩下的存进去
            if (!string.IsNullOrEmpty(buffer))
            {
                result.Add(new TextChunk { Text = buffer, FontSize = GetSize(currentState) });
            }
            return result;
        }
        // 辅助函数：用于解析函数判断
        private static bool IsEnglish(char c)
        {
            if (c >= 'a' && c <= 'z') return true;
            else return false;
        }
        // 辅助函数：用于解析函数判断
        private static bool IsSpecialUnit(char c)
        {
            return c == '³' || c == '×';
        }

  

        // ====================================================================
        // ========================  私有绘制接口 =============================
        // ====================================================================
        /// <summary>
        /// ======= 只画出24bpp图 ======
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tpl"></param>
        /// <returns></returns>
        private static Bitmap drawTemplate(trainInfo info, Template tpl)
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
                            g.DrawString("重", font, brush, 1649 + 353, 1649 + tpl.LineSpacing, sf);

                            g.DrawString("容", font, brush, 0, (1649 + tpl.LineSpacing) * 2, sf);
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



        // ====================================================================
        // ========================  对外公开接口 =============================
        // ====================================================================
        /// <summary>
        /// 清空文件夹并按序生成图片，切割图片并保存，返回的是1bpp完整图像
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tpl"></param>
        /// <param name="rotate"></param>
        /// <param name="sliceHight"></param>
        /// <param name="sliceSavePath"></param>
        /// <returns></returns>
        public Bitmap BmpRender(trainInfo info, Template tpl, bool rotate, int sliceHight, string sliceSavePath)
        {
            // 放到按键回调里了
            //Array.ForEach(Directory.GetFiles(sliceSavePath), File.Delete);
            
            Bitmap bmp;

            // 生成图片_24bpp
            bmp = drawTemplate(info, tpl);
            // 旋转图片_24bpp
            ImageProcessor.RotateImg(bmp, rotate);
            // 切割图片_24bppTo1bpp保存
            if (rotate)     ImageProcessor.v_SliceBmp(bmp, tpl, sliceHight, sliceSavePath);
            else            ImageProcessor.h_SliceBmp(bmp, tpl, sliceHight, sliceSavePath);
            // 对原图转换成1bpp
            var newBmp = ImageProcessor.Convert24bppTo1bpp(bmp);

            return newBmp;
        }

    }
}



/*
 * 
 *         public Bitmap BmpRender1(string[] lines, Template tpl, bool rotate, int sliceHight, string sliceSavePath)
        {
            float currentX = 0;
            float currentY = 0;


            // 1. 创建画布(Bitmap)
            Bitmap RawBmp = new Bitmap(tpl.Width, tpl.Height, PixelFormat.Format24bppRgb);
            try
            {
                // 2. 创建画笔并绘制(Graphics)
                using (Graphics g = Graphics.FromImage(RawBmp))
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

                    // ========== 逐行绘制 ==========
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            currentY += (tpl.BigFont + tpl.LineSpacing);
                            continue;
                        }
                        // 调用 ParseText 解析当前一行的字体大小片段
                        List<TextChunk> chunks = ParseText(line, tpl.BigFont, tpl.SmallFont);
                        // 为了保证对齐，需要实现的是文字底边对齐而不是字框底边对齐，算出"大字"的基线位置作为标准,
                        FontFamily f = new FontFamily("宋体");
                        float bigAscent = tpl.BigFont * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);

                        // 绘制当前一行的字符串
                        foreach (var chunk in chunks)
                        {
                            using (Font font = new Font("宋体", chunk.FontSize, GraphicsUnit.Pixel))
                            {
                                // 算出当前这个字号基线到顶部的距离
                                float ascent = chunk.FontSize * f.GetCellAscent(FontStyle.Regular) / (float)f.GetEmHeight(FontStyle.Regular);
                                // 行底部对齐：当前Y = 起始Y + （行高-当前字高）
                                float yBaseline = currentY + (bigAscent - ascent);
                                // 绘制
                                g.DrawString(chunk.Text, font, brush, currentX, yBaseline, sf);
                                // 移动x位置
                                SizeF size = g.MeasureString(chunk.Text, font, PointF.Empty, sf);
                                currentX += (size.Width + tpl.CharSpacing);
                            }
                        }
                        currentX = 0;//新一行从头开始
                        currentY += (tpl.BigFont + tpl.LineSpacing);
                    }

                }
                // 切割图片
                ImageProcessor.h_SliceBmp(RawBmp, tpl, sliceHight, sliceSavePath);
                // 旋转图片
                ImageProcessor.RotateImg(RawBmp, rotate);
                // 转换为1bpp图片
                Bitmap bmp = ImageProcessor.Convert24bppTo1bpp(RawBmp); 

                RawBmp.Dispose();
                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成失败: " + ex.Message);
                return null;
            }
        }


// 使用不稳定unsafe指针的位图转换
        /*
        private unsafe Bitmap ConvertTO1Bpp(Bitmap source)
        {
            int w = source.Width;
            int h = source.Height;
            //创建承接的1bit位图
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);

            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;
                int srcStride = srcData.Stride;
                int dstStride = dstData.Stride;

                for (int y = 0; y < h; y++)
                {
                    byte* srcRow = srcPtr + y * srcStride;
                    byte* dstRow = dstPtr + y * dstStride;
                    
                    for (int x = 0; x < w; x++)
                    {
                        // 原图是24bit RGB
                        byte b = srcRow[x * 3 + 0];
                        byte g = srcRow[x * 3 + 1];
                        byte r = srcRow[x * 3 + 2];

                        // 灰度值
                        byte gray = (byte)((r + g + b) / 3);

                        // 白色是1，黑色是0
                        bool isWhite = gray > 128;

                        if (isWhite)
                            dstRow[x / 8] |= (byte)(0x80 >> (x % 8));
                        // 黑色默认是0，不需要操作
                    }
                    //System.Runtime.InteropServices.Marshal.Copy(scan, 0, Outdata.Scan0+y * Outdata.Stride, scan.Length);
                }
            }
            bmp.UnlockBits(dstData);
            source.UnlockBits(srcData);
            return bmp;
        }
        */

