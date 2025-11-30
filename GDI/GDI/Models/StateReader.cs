using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDI.Models
{
    public class StateReader
    {
        public static bool _isConnected = false;
        public static bool _running = false;
        public static bool _test = false;
        public static bool _modbus_laser = false;

        public static void socketStart()
        {
            _isConnected = true;
        }
        public static void socketStop()
        {
            _isConnected = false;
        }
        public static void Stop()
        {
            _running = false;
            _isConnected = false;
            _test = false;
        }
        public static void Start()
        {
            _test = true;
        }


    }
}
