using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btn_bs_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.StartPosition = FormStartPosition.CenterParent;
            f.Show();
            f.Activate();
        }
    }
}
