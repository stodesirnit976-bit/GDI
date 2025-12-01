using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GDI.Models
{
    public static class SyncOjbects
    {
        // 机械臂运动结束通知9837关闭喷印软件通知
        public static readonly AutoResetEvent armFinsh = new AutoResetEvent(false);

        // 

    }
}
