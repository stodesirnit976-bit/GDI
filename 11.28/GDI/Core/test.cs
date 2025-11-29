using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static GDI.Services.Arm;

namespace GDI.Core
{
    public class test
    {
        public static void Modbus(IntPtr Arm.Instance.robotHandlePtr)
        {
            // 修改dllimport
            // public static extern int rm_read_multiple_holding_registers(IntPtr handle, rm_peripheral_read_write_params_t param, int[] data);// 修改这里
            // [DllImport("api_c.dll", EntryPoint = "rm_read_holding_registers", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
            //  
            // 末端 RS485 设置为 RTU 主站，波特率 9600，超时时间 10*100ms
            int port = 1;
            int baudrate = 9600;
            int timeout = 10;
            int ret = rm_set_modbus_mode(Arm.Instance.robotHandlePtr, port, baudrate, timeout);

            rm_peripheral_read_write_params_t paramsConfig = new rm_peripheral_read_write_params_t();
            paramsConfig.port = 1;     // 末端接口
            paramsConfig.device = 1;   // 传感器站号
            paramsConfig.address = 0;  // 起始地址
            paramsConfig.num = 3;      // 读3个寄存器

            int[] buffer = new int[10];

            int a = rm_read_multiple_holding_registers(Arm.Instance.robotHandlePtr, paramsConfig, buffer);

            if (a == 0)
            {

                long rawDistance = ((buffer[0] & 0xFF) << 24) |
                                   ((buffer[1] & 0xFF) << 16) |
                                   ((buffer[2] & 0xFF) << 8) |
                                   (buffer[3] & 0xFF);

                double distance = rawDistance / 1000.0;

                Console.WriteLine($"读取距离: {distance} mm");
            }
            else
            {
                Console.WriteLine($"读取失败，错误码: {a}");
            }
        }

        public static void test2(IntPtr Arm.Instance.robotHandlePtr)
        {
            // 实例化结构体
            rm_arm_all_state_t armState = new rm_arm_all_state_t();
            armState.joint_current = new float[7];
            armState.joint_en_flag = new int[7];
            armState.joint_temperature = new float[7];
            armState.joint_voltage = new float[7];
            armState.joint_err_code = new int[7];


            rm_current_arm_state_t armState1 = new rm_current_arm_state_t();
            armState1.joint = new float[7];


            // 调用 API
            int a = rm_get_arm_all_state(Arm.Instance.robotHandlePtr, ref armState);
            int b = rm_get_current_arm_state(Arm.Instance.robotHandlePtr, ref armState1);

            if (a == 0 && b == 0)
            {
                Console.WriteLine($"关节1电流: {armState.joint_current[0]}");
                Console.WriteLine($"关节1使能状态: {armState.joint_en_flag[0]}");
                Console.WriteLine($"关节1温度: {armState.joint_temperature[0]}");
                Console.WriteLine($"关节1电压: {armState.joint_voltage[0]}");

                Console.WriteLine($"关节1角度: {armState1.joint[0]}");
                Console.WriteLine($"机械臂当前位姿: {armState1.pose}");
            }
            else
            {
                Console.WriteLine($"报错: {a}{b}");
            }
        }
    }
}
