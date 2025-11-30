using GDI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDI.Core
{
    public class TemplateManager
    {
        public List<Template> GetAllTemplates()
        {
            return new List<Template>
            {
                new Template
                {
                    Name = "模板1",
                    Width = 13424,
                    Height = 5890,
                    BigFont = 1649,
                    SmallFont = 1178,
                    CharSpacing = 200,
                    LineSpacing = 400,
                },

                new Template
                {
                    Name = "模板2",
                    Width = 13424,
                    Height = 4712,
                    BigFont = 1649,
                    SmallFont = 1178,
                    CharSpacing = 200,
                    LineSpacing = 200,
                },

                new Template
                {
                    Name = "路徽",
                    Width = 3534,
                    Height = 3534,
                    BigFont = 1649,
                    SmallFont = 1178,
                    CharSpacing = 0,
                }
            };
        }

        public List<trainInfo> getInfo()
        {
            return new List<trainInfo>()
            {
               
            };
        }

    }

}
