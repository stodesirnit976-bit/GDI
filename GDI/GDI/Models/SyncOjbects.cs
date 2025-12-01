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
        public static readonly AutoResetEvent armFinsh = new AutoResetEvent(false);


    }
}
