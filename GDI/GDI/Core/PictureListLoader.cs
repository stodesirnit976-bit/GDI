using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDI.Core
{
    internal class PictureListLoader
    {
        // form_load加载所有bmp文件到combobox  
            public static void Load(string folderPath, ComboBox combo)
            {
                combo.Items.Clear();

                if (!Directory.Exists(folderPath))
                    return;
                var files = Directory.EnumerateFiles(folderPath, "*.bmp")
                                     .OrderBy(f => f); // 排序

                foreach (var f in files)
                    combo.Items.Add(Path.GetFileName(f));

                if (combo.Items.Count > 0)
                    combo.SelectedIndex = 0;   // 自动选中第一项
            }
    }
}