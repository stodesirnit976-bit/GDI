using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDI.Models
{
    public class Template
    {
        public string Name { get; set; } = "Default";

        public int Width { get; set; } 
        public int Height { get; set; }

        public int BigFont { get; set; } 
        public int SmallFont { get; set; }

        public float LineSpacing { get; set; } = 0;
        public float CharSpacing { get; set; } = 0;

        public float RotateAngle { get; set; } = 0f; // 度，正数顺时针
    }

    public class trainInfo
    {
        public string payLoad { get; set; }      // 载重 t
        public string tareWeight { get; set; }   // 自重 t
        public string volume { get; set; }       // 容积 m³
        public string length { get; set; }       // mm
        public string width { get; set; }        // mm
        public string height { get; set; }       // mm
        public string trans { get; set; }    // 换长 mm
    }

}
