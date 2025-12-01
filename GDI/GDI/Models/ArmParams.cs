using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Drawing;

namespace GDI.Models
{
    public class ArmParams
    {
        public float Len { get; set; }
        public float Wid { get; set; }
        public float Height { get; set; }
        public bool N { get; set; }
        public int Count { get; set; }
        public int Vol { get; set; }
    }

    public class GetArmParams
    {
        public static ArmParams Params(string sliceSavePath, string heightText, bool N)
        {
            if (string.IsNullOrEmpty(heightText))
            {
                MessageBox.Show("请输入高度参数！");
                return null;
            }
            // 以下的策略是只需要手动设置速度、NorZ、高度三个参数
            // 长宽直接取自文件夹文件数值
            // 直接读取 sliceImage 发送文件夹里第一张图片的尺寸
            var firstFile = Directory.EnumerateFiles(sliceSavePath, "*.bmp")
                         .FirstOrDefault();
            if (firstFile == null)
            {
                MessageBox.Show("文件夹里没找到 .bmp 文件");
                return null;
            }
            using var img = Image.FromFile(firstFile);

            float tran = (float)54.36 / 1280; // 1 pixel = tran 米 1p = tran*1000 mm
            float len = (float)img.Width * tran;
            float wid = (float)img.Height * tran;

            float height = float.Parse(heightText);
            int count = Directory.EnumerateFiles(sliceSavePath, "*.bmp").Count();
            int vol = 30;

            return new ArmParams
            {
                Len = len,
                Wid = wid,
                Height = height,
                N = N,
                Count = count,
                Vol = vol,
            };
           
        }
    }
    


}
