using GDI.Core;
using GDI.Models;
using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GDI.Services.Arm;


namespace GDI.Services
{
    public class Arm
    {
        public enum rm_thread_mode_e
        {
            RM_SINGLE_MODE_E,       // 单线程模式，单线程非阻塞等待数据返回
            RM_DUAL_MODE_E,     // 双线程模式，增加接收线程监测队列中的数据
            RM_TRIPLE_MODE_E,       // 三线程模式，在双线程模式基础上增加线程监测UDP接口数据
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct rm_quat_t
        {
            public float w;
            public float x;
            public float y;
            public float z;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct rm_position_t
        {
            public float x; // 单位：m  
            public float y;
            public float z;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct rm_euler_t
        {
            public float rx; // 单位：rad  
            public float ry;
            public float rz;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct rm_peripheral_read_write_params_t
        {
            public int port;
            public int address;
            public int device;
            public int num;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)] // 根据需要调整对齐方式  
        public struct rm_current_arm_state_t
        {
            public rm_pose_t pose; // 机械臂当前位姿  
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public float[] joint; // 机械臂当前关节角度  
            public byte arm_err; // 机械臂错误代码  
            public byte sys_err; // 控制器错误代码  
        }

        public struct rm_arm_all_state_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public float[] joint_current;           // 关节电流，单位mA  
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public int[] joint_en_flag;            // 关节使能状态  
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public float[] joint_temperature;       // 关节温度,单位℃  
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public float[] joint_voltage;           // 关节电压，单位V  
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public int[] joint_err_code;            // 关节错误码  
            public int sys_err;                     // 机械臂错误代码  

        }


        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct rm_pose_t
        {
            public rm_position_t position;     // 位置，单位：m  
            public rm_quat_t quaternion;       // 四元数  
            public rm_euler_t euler;           // 欧拉角，单位：rad 

            public override string ToString()
            {
                // 使用StringBuilder来构建字符串，因为它在处理大量字符串连接时更高效  
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendFormat("  position: {0}, {1}, {2}", position.x, position.y, position.z);
                sb.AppendLine(); // AppendFormat 不包含换行，所以需要手动添加  
                sb.AppendFormat("  quaternion: {0}, {1}, {2}, {3}", quaternion.w, quaternion.x, quaternion.y, quaternion.z);
                sb.AppendLine();
                sb.AppendFormat("  euler: {0}, {1}, {2}", euler.rx, euler.ry, euler.rz);
                sb.AppendLine(); // 最后一行也添加换行，以保持格式一致  

                return sb.ToString();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct rm_robot_handle
        {
            public int id;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct rm_frame_name_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] name;

            // 提供一个辅助属性来方便地将byte数组转换为string  
            public string NameAsString
            {
                get
                {
                    return System.Text.Encoding.ASCII.GetString(name).TrimEnd('\0');
                }
                set
                {
                    var bytes = System.Text.Encoding.ASCII.GetBytes(value);
                    Array.Copy(bytes, name, Math.Min(bytes.Length, name.Length));
                    // 填充剩余的空间为null字符（如果需要）  
                    for (int i = bytes.Length; i < name.Length; i++)
                    {
                        name[i] = 0;
                    }
                }
            }
        }

        public enum rm_pos_teach_type_e
        {
            RM_X_DIR_E,        // 位置示教，x轴方向  
            RM_Y_DIR_E,        // 位置示教，y轴方向  
            RM_Z_DIR_E         // 位置示教，z轴方向  
        }






        // 引入 DllImport 特性，并指定 DLL 名称 api_c.dll，
        [DllImport("api_c.dll", EntryPoint = "rm_init", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_init(rm_thread_mode_e mode);

        [DllImport("api_c.dll", EntryPoint = "rm_create_robot_arm", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr rm_create_robot_arm([MarshalAs(UnmanagedType.LPStr)] string ip, int arm_prot);

        [DllImport("api_c.dll", EntryPoint = "rm_movej", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_movej(IntPtr handle, [In, Out] float[] joint, int v, int r, int trajectory_connect, int block);

        [DllImport("api_c.dll", EntryPoint = "rm_movel", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_movel(IntPtr handle, rm_pose_t pose, int v, int r, int trajectory_connect, int block);

        [DllImport("api_c.dll", EntryPoint = "rm_movec", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_movec(IntPtr handle, rm_pose_t pose_via, rm_pose_t pose_to, int v, int r, int loop, int trajectory_connect, int block);

        [DllImport("api_c.dll", EntryPoint = "rm_set_auto_work_frame", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_auto_work_frame(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string workname, int point_num);

        [DllImport("api_c.dll", EntryPoint = "rm_set_manual_work_frame", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_manual_work_frame(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string work_name, rm_pose_t pose);

        [DllImport("api_c.dll", EntryPoint = "rm_change_work_frame", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_change_work_frame(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string work_name);

        [DllImport("api_c.dll", EntryPoint = "rm_delete_work_frame", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_delete_work_frame(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string work_name);

        [DllImport("api_c.dll", EntryPoint = "rm_update_work_frame", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_update_work_frame(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string work_name, rm_pose_t pose);

        [DllImport("api_c.dll", EntryPoint = "rm_set_arm_stop", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_arm_stop(IntPtr handle);

        [DllImport("api_c.dll", EntryPoint = "rm_set_modbus_mode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_modbus_mode(IntPtr handle, int port, int baudrate, int timeout);

        [DllImport("api_c.dll", EntryPoint = "rm_close_modbus_mode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_close_modbus_mode(IntPtr handle, int port);

        [DllImport("api_c.dll", EntryPoint = "rm_read_coils", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_read_coils(IntPtr handle, rm_peripheral_read_write_params_t param, ref int data);

        [DllImport("api_c.dll", EntryPoint = "rm_read_multiple_holding_registers", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_read_multiple_holding_registers(IntPtr handle, rm_peripheral_read_write_params_t param, int[] data);// 修改这里，使用 IntPtr

        [DllImport("api_c.dll", EntryPoint = "rm_read_holding_registers", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_read_holding_registers(IntPtr handle, rm_peripheral_read_write_params_t param, ref int data);

        [DllImport("api_c.dll", EntryPoint = "rm_set_tool_DO_state", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_tool_DO_state(IntPtr handle, int io_num, int state);

        [DllImport("api_c.dll", EntryPoint = "rm_set_tool_IO_mode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_tool_IO_mode(IntPtr handle, int io_num, int io_mode);

        [DllImport("api_c.dll", EntryPoint = "rm_get_tool_IO_state", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_tool_IO_state(IntPtr handle, ref int mode, ref int state);

        [DllImport("api_c.dll", EntryPoint = "rm_set_tool_voltage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_tool_voltage(IntPtr handle, int voltage_type);

        [DllImport("api_c.dll", EntryPoint = "rm_get_tool_voltage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_tool_voltage(IntPtr handle, ref int voltage_type);

        [DllImport("api_c.dll", EntryPoint = "rm_get_controller_state", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_controller_state(IntPtr handle, ref float voltage, ref float current, ref float temperature, ref int err_flag);

        [DllImport("api_c.dll", EntryPoint = "rm_get_current_arm_state", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_current_arm_state(IntPtr handle, ref rm_current_arm_state_t state);

        [DllImport("api_c.dll", EntryPoint = "rm_get_arm_all_state", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_arm_all_state(IntPtr handle, ref rm_arm_all_state_t state);

        [DllImport("api_c.dll", EntryPoint = "rm_set_IO_mode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_IO_mode(IntPtr handle, int io_num, int io_mode);

        [DllImport("api_c.dll", EntryPoint = "rm_set_DO_state", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_DO_state(IntPtr handle, int io_num, int state);

        [DllImport("api_c.dll", EntryPoint = "rm_get_IO_state", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_IO_state(IntPtr handle, int io_num, ref int state, ref int mode);

        [DllImport("api_c.dll", EntryPoint = "rm_get_IO_input", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_IO_input(IntPtr handle, ref int DI_state);

        [DllImport("api_c.dll", EntryPoint = "rm_get_IO_output", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_IO_output(IntPtr handle, ref int DO_state);

        [DllImport("api_c.dll", EntryPoint = "rm_set_voltage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_voltage(IntPtr handle, int voltage_type);

        [DllImport("api_c.dll", EntryPoint = "rm_get_voltage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_voltage(IntPtr handle, ref int voltage_type);

        [DllImport("api_c.dll", EntryPoint = "rm_movej_p", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_movej_p(IntPtr handle, rm_pose_t pose, int v, int r, int trajectory_connect, int block);












        // ===================================================================================================
        // ======================================= 机械臂动作示例 ============================================
        // ===================================================================================================

        // 机械臂使用方法：调用： ArmManager.Instance.Arm.Instance.robotHandlePtr

        private static Arm _instance = new Arm();
        public static Arm Instance => _instance;

        // 机械臂句柄
        public IntPtr robotHandlePtr { get; private set; }
        
        
        // 机械臂结构体
        //public rm_pose_t c_ini;
        public rm_peripheral_read_write_params_t paramsConfig;
        public rm_current_arm_state_t armState;

        // 上锁变量
        //public double distance;
        //private readonly object distanceLock = new object(); 
        //public double Distance
        //{
        //    get
        //    {
        //        lock (distanceLock) { return distance; }
        //    }
        //    set
        //    {
        //        lock (distanceLock) { distance = value; }
        //    }
        //}

        public static float[] c_ini;

        // ==========================================
        private Arm()
        {
            robotHandlePtr = IntPtr.Zero;

            //c_ini.position.x = 0;
            //c_ini.position.y = 0;
            //c_ini.position.z = 0.5f;
            //c_ini.euler.rx = -3.14f;
            //c_ini.euler.ry = (float)(-1.570f + Math.PI / 23);
            //c_ini.euler.rz = 3.14f;

            paramsConfig.port = 1;     // 末端接口
            paramsConfig.device = 1;   // 传感器站号
            paramsConfig.address = 0;  // 起始地址
            paramsConfig.num = 3;      // 读2个寄存器 (对应4个字节)

            c_ini[0] = 0;
            c_ini[1] = 94;
            c_ini[2] = -129.5f;
            c_ini[3] = 0;
            c_ini[4] = -62.4f;
            c_ini[5] = 0;

        }


        private static int arm_Move(float len, float wid, float height, bool N_or_Z, int count, int vol)
        {
            height += 0.26f;
            int ret = 0;
            int r = 0;  // 交融半径
            int trajectory_connect = 0; // 轨迹连接
            int block = 1; // 阻塞

            rm_pose_t c2 = new();   // 运动位姿
            c2.position.x = 0.05f;
            c2.position.y = 0.25f;
            c2.position.z = height;
            c2.euler.rx = (float)Math.PI;
            c2.euler.ry = 0;
            c2.euler.rz = (float)Math.PI / 2;

            rm_pose_t c3 = new();   // 运动位姿
            c3.position.x = 0.05f;
            c3.position.y = 0.25f + len;
            c3.position.z = height;
            c3.euler.rx = (float)Math.PI;
            c3.euler.ry = 0;
            c3.euler.rz = (float)Math.PI / 2;

            rm_pose_t c4 = new();   // 运动位姿
            c4.position.x = 0.05f;
            c4.position.y = 0.05f;
            c4.position.z = height;
            c4.euler.rx = (float)Math.PI;
            c4.euler.ry = 0;
            c4.euler.rz = -(float)Math.PI / 2;

            rm_pose_t c5 = new();   // 运动位姿
            c5.position.x = 0.05f + wid;
            c5.position.y = 0.05f;
            c5.position.z = height;
            c5.euler.rx = (float)Math.PI;
            c5.euler.ry = 0;
            c5.euler.rz = -(float)Math.PI / 2;

            rm_movej_p(Arm.Instance.robotHandlePtr, N_or_Z ? c4 : c2, 20, r, 1, 0);
            for (int i = 0; i < count; i++)
            {
                rm_movel(Arm.Instance.robotHandlePtr, N_or_Z ? c4 : c2, vol, r, 1, 0);

                if (i == count) { trajectory_connect = 0; block = 1; }
                rm_movel(Arm.Instance.robotHandlePtr, N_or_Z ? c5 : c3, vol, r, trajectory_connect, block);

                if (!N_or_Z)
                {
                    c2.position.x += wid;
                    c3.position.x += wid;
                }
                else
                {
                    c4.position.y += len;
                    c5.position.y += len;
                }
            }

            rm_change_work_frame(Arm.Instance.robotHandlePtr, "Base");
            rm_movej(Arm.Instance.robotHandlePtr, c_ini, 20, 0, 0, 1);
            rm_change_work_frame(Arm.Instance.robotHandlePtr, "work1");
            return ret;
        }


        // 机械臂初始化：获取句柄、回到初始位置
        public static void armInit()
        {
            _ = rm_init(rm_thread_mode_e.RM_TRIPLE_MODE_E);
            Arm.Instance.robotHandlePtr = rm_create_robot_arm("192.168.22.18", 8080);
            rm_robot_handle robotHandle = (rm_robot_handle)Marshal.PtrToStructure(Arm.Instance.robotHandlePtr, typeof(rm_robot_handle))!;

            // 移动初始位置
            rm_change_work_frame(Arm.Instance.robotHandlePtr, "Base");
            int ret = rm_movej(Arm.Instance.robotHandlePtr, c_ini, 20, 0, 0, 1);
            if (ret != 0) MessageBox.Show("[rm_move_joint] Error occurred: " + ret);

            if (Arm.Instance.robotHandlePtr != IntPtr.Zero && robotHandle.id > 0)
                MessageBox.Show("Arm Init Success!");
            else
                MessageBox.Show("[rm_create_robot_arm] connect error:" + robotHandle.id);
        }



        public void move(float len, float wid, float height, bool N, int count, int vol)
        {
            // 调用C语言睿尔曼机械臂开发包动态库中的 rm_init 函数  
            // 初始化为三线程模式这
            //int ret = Demo_Move_Cmd(Arm.Instance.robotHandlePtr, 0.08f, 0.02f, 0.075f, 0, 2, 20);
            int ret = arm_Move(len, wid, height, N, count, vol);
        }









        // ===================================================================================================
        // ======================================= 获取机械臂状态参数 =====================================
        // ==================================================================================================
        //rm_get_controller_state
        public static Action<string> Laser;
        public static void test1()
        {
            float v = 0, c = 0, t = 0;
            int err = 0;
            int ret = rm_get_controller_state(Arm.Instance.robotHandlePtr, ref v, ref c, ref t, ref err);

            if (ret == 0)
            {
                Console.WriteLine($"电压: {v}");
                Console.WriteLine($"电流: {c}");
                Console.WriteLine($"温度: {t}");
            }
            else
            {
                Console.WriteLine($"报错: {ret}");
            }
        }
        public static void test2()
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
        // The constructor `ArmManager` was missing a return type. 
        // Since this is intended to be a constructor, the class name should match exactly.
        // Correcting the constructor name to match the class name `Arm`.

        


    }
}
