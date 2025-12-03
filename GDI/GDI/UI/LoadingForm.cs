using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDI.UI
{
    public partial class LoadingForm : Form
    {
        private System.Windows.Forms.Timer _uiTimer;

        public LoadingForm()
        {
            InitializeComponent();

            // 初始化假进度动画的 Timer
            _uiTimer = new System.Windows.Forms.Timer();
            _uiTimer.Interval = 200;
            _uiTimer.Tick += UiTimer_Tick;
        }

        // 窗口加载时，自动开始任务
        private async void LoadingForm_Load(object sender, EventArgs e)
        {
            _uiTimer.Start(); // 1. 进度条开始动

            // 2. 开启后台线程执行那个随机 10s-2min 的任务
            await Task.Run(() => DoComplexWork());

            // 3. 任务完成，关闭自己
            // 因为这是在 await 之后，已经回到了 UI 上下文，直接 Close 即可
            this.Close();
        }

        private void DoComplexWork()
        {
            //while (Calibration.UNfinish_calibration)
            //{

            //}
            // --- 这里模拟你的耗时操作 ---
            Thread.Sleep(5000);
        }

        // 依然使用“渐进式欺骗”算法，让进度条看起来在动
        private void UiTimer_Tick(object sender, EventArgs e)
        {
            // 让进度条卡在 95% 左右，直到任务真的结束
            if (progressBar1.Value < 95)
            {
                int remaining = 100 - progressBar1.Value;
                int step = Math.Max(1, remaining / 30);
                progressBar1.Value += step;
            }

            // 可以让 Label 动一动，增加等待的耐心
            lab_Status.Text = $"正在处理中... {progressBar1.Value}%";
        }
    }
}
