using GDI.Models;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static GDI.Services.Arm;

namespace GDI.Services
{
    public class LaserSensor
    {
        // 激光传感器数据和机械臂操作之间的后台线程数据传递需要考虑极高的实时性，由于作业高度只有 1-5mm，
        // 并且数据应该保证最新，可以适当牺牲部分数据完整性（丢弃旧数据），因此不用队列传递数据（或者有界队列），使用共享变量的方式进行数据传递？
        public Action<string> M;
        public Action<string> D;
        private Thread Laser_Thread;
        private CancellationTokenSource laser_cts;

        public static double distance = 0.0;
        // ===================================================================================================
        // ======================================= 连接机械臂版本 ============================================
        // ===================================================================================================
        // ================= 用机械臂末端modbus读取3个保持寄存器,只用两个 ==================
        public static void Laser_Loop(CancellationToken token)
        {
            // 末端 RS485 设置为 RTU 主站，波特率 9600，超时时间 10*100ms
            int port = 1;
            int baudrate = 9600;
            int timeout = 10;
            int ret = rm_set_modbus_mode(Arm.Instance.robotHandlePtr, port, baudrate, timeout);
            int[] buffer = new int[20];
            //rm_current_arm_state_t state_1 = new rm_current_arm_state_t();    //wm修改

            while (!token.IsCancellationRequested)
            {
                int a = rm_read_multiple_holding_registers(Arm.Instance.robotHandlePtr, Arm.Instance.paramsConfig, buffer);

                // 数据处理
                if (a == 0)
                {
                    long rawDistance = ((buffer[0] & 0xFF) << 24) |
                                       ((buffer[1] & 0xFF) << 16) |
                                       ((buffer[2] & 0xFF) << 8) |
                                       (buffer[3] & 0xFF);

                    distance = rawDistance / 1000.0;

                    Console.WriteLine($"读取距离: {distance} mm");
                }
                else
                {
                    Console.WriteLine($"读取失败，错误码: {ret} {a}");
                }

                // 机械臂急停,需要注意值究竟是多少,要不要取多次数据融合？
                if (distance != 0 && distance < 216)
                {
                    Task.Run(() =>
                    {
                        Arm.rm_set_arm_stop(Arm.Instance.robotHandlePtr);
                    });
                }
                //\rm_get_current_arm_state(Arm.Instance.robotHandlePtr, ref state_1);
                // 每隔1秒读取一次
                Thread.Sleep(1000);
            }

        }

        public void Start()
        {
            laser_cts = new CancellationTokenSource();
            Laser_Thread = new Thread(() => Laser_Loop(laser_cts.Token));
            Laser_Thread.IsBackground = true;
            Laser_Thread.Start();
        }
        public void Stop()
        {
            laser_cts.Cancel();
        }



        // ===================================================================================================
        // ======================================= 连接电脑版本 ==============================================
        // ===================================================================================================


        public void plusss()
        {
            int a = 0;
            SerialPort port = new SerialPort("COM3");
            port.BaudRate = 9600;      // 默认波特率
            port.DataBits = 8;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.Open(); // 打开串口
            // 2. 创建 Modbus 主站对象
            var master = ModbusSerialMaster.CreateRtu(port);

            while (StateReader._test)
            {
                try
                {
                    a++;
                    M(a.ToString());                   
                    // 等于是创造了一个生命周期 100 秒的线程
                    if (a == 1000)
                        StateReader._test = false;      

                    Console.WriteLine("开始读取传感器数据...");

                    // 3. 发送指令并接收
                    // 会自动发送指令，并解析返回的数据
                    ushort[] registers = master.ReadHoldingRegisters(1, 0, 2);

                    // 4. 数据处理
                    // 高位在前，把两个 16位 寄存器拼成一个 32位 整数
                    // 寄存器[0] 是高位，寄存器[1] 是低位
                    if (registers.Length == 2)
                    {
                        // 高位左移16位 + 低位
                        int rawValue = (registers[0] << 16) | registers[1];

                        // 5. 换算物理量
                        // 返回值除以 1000 =》 mm 
                        double distanceMm = rawValue / 1000.0;
                        //if (distanceMm == 0)
                            //D("超出量程");
                        if(distanceMm != 0)
                            D(distanceMm.ToString());
                        else D("超出量程");
                        Console.WriteLine($"原始数值: {rawValue}");
                        Console.WriteLine($"实测距离: {distanceMm} mm");
                    }

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"读取失败: {ex.Message}");
                }              
            }
            port.Close();
        }

        public void Starttest()
        {
            Thread testThread;
            StateReader._test = true;
            testThread = new Thread(plusss);
            testThread.IsBackground = true;
            testThread.Start();
        }



        



    }
}
