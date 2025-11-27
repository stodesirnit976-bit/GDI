using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Demo1
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
    public struct rm_force_data_t
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] force_data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] zero_force_data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] work_zero_force_data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] tool_zero_force_data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)] // 根据需要调整对齐方式  
    public struct rm_frame_t
    {
        public rm_frame_name_t frame_name;    // 坐标系名称  
        public rm_pose_t pose;              // 坐标系位姿  
        public float payload;             // 坐标系末端负载重量，单位：kg  
        public float x;                   // 坐标系末端负载质心位置，单位：m  
        public float y;                   // 坐标系末端负载质心位置，单位：m  
        public float z;                   // 坐标系末端负载质心位置，单位：m  
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

    [StructLayout(LayoutKind.Sequential, Pack = 8)] // 根据需要调整对齐方式  
    public struct rm_current_arm_state_t
    {
        public rm_pose_t pose; // 机械臂当前位姿  
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public float[] joint; // 机械臂当前关节角度  
        public byte arm_err; // 机械臂错误代码  
        public byte sys_err; // 控制器错误代码  
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct rm_peripheral_read_write_params_t
    {
        public int port;
        public int address;
        public int device;
        public int num;
    }

    public partial class Form1 : Form
    {
        private IntPtr _robotHandlePtr = IntPtr.Zero;
        private rm_pose_t c_ini = new rm_pose_t();

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

        [DllImport("api_c.dll", EntryPoint = "rm_destroy_robot_arm", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_destroy_robot_arm(IntPtr handle);

        [DllImport("api_c.dll", EntryPoint = "rm_cleanup", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_cleanup();

        [DllImport("api_c.dll", EntryPoint = "rm_movej_p", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_movej_p(IntPtr handle, rm_pose_t pose, int v, int r, int trajectory_connect, int block);

        [DllImport("api_c.dll", EntryPoint = "rm_get_force_data", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_force_data(IntPtr handle, ref rm_force_data_t data);

        [DllImport("api_c.dll", EntryPoint = "rm_get_given_tool_frame", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_given_tool_frame(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string name, ref rm_frame_t frame);

        [DllImport("api_c.dll", EntryPoint = "rm_get_current_tool_frame", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_current_tool_frame(IntPtr handle, ref rm_frame_t tool_frame);

        [DllImport("api_c.dll", EntryPoint = "rm_get_current_arm_state", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_current_arm_state(IntPtr handle, ref rm_current_arm_state_t state);

        [DllImport("api_c.dll", EntryPoint = "rm_get_tool_voltage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_get_tool_voltage(IntPtr handle, ref int voltage_type);

        [DllImport("api_c.dll", EntryPoint = "rm_set_tool_voltage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_tool_voltage(IntPtr handle, int voltage_type);

        [DllImport("api_c.dll", EntryPoint = "rm_set_modbus_mode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_set_modbus_mode(IntPtr handle, int port, int baudrate, int timeout);

        [DllImport("api_c.dll", EntryPoint = "rm_read_coils", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_read_coils(IntPtr handle, rm_peripheral_read_write_params_t param, ref int data);

        [DllImport("api_c.dll", EntryPoint = "rm_close_modbus_mode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_close_modbus_mode(IntPtr handle, int port);

        [DllImport("api_c.dll", EntryPoint = "rm_write_coils", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_write_coils(IntPtr handle, rm_peripheral_read_write_params_t param, ref int data);

        [DllImport("api_c.dll", EntryPoint = "rm_read_multiple_holding_registers", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rm_read_multiple_holding_registers(IntPtr handle, rm_peripheral_read_write_params_t param, ref int data);

        private static int Demo_Move_Cmd(IntPtr robotHandlePtr, float len, float wid, float height, bool N_or_Z, int count, int vol)
        {
            height += 0.26f;
            int ret;
            int r = 0;  // 交融半径
            int trajectory_connect = 0; // 轨迹连接
            int block = 1; // 阻塞

            rm_pose_t c2 = new();   // 运动位姿
            c2.position.x = 0.05f;
            c2.position.y = 0.05f;
            c2.position.z = height;
            c2.euler.rx = (float)Math.PI;
            c2.euler.ry = 0;
            c2.euler.rz = (float)Math.PI;

            rm_pose_t c3 = new();   // 运动位姿
            c3.position.x = 0.05f;
            c3.position.y = 0.05f + len;
            c3.position.z = height;
            c3.euler.rx = (float)Math.PI;
            c3.euler.ry = 0;
            c3.euler.rz = (float)Math.PI;

            rm_pose_t c4 = new();   // 运动位姿
            c4.position.x = 0.05f;
            c4.position.y = 0.05f;
            c4.position.z = height;
            c4.euler.rx = (float)Math.PI;
            c4.euler.ry = 0;
            c4.euler.rz = -(float)Math.PI/2;

            rm_pose_t c5 = new();   // 运动位姿
            c5.position.x = 0.05f + wid;
            c5.position.y = 0.05f;
            c5.position.z = height;
            c5.euler.rx = (float)Math.PI;
            c5.euler.ry = 0;
            c5.euler.rz = -(float)Math.PI/2;

            ret = rm_movej_p(robotHandlePtr, N_or_Z ? c4 : c2, 20, r, trajectory_connect, block);
            for (int i = 0; i < count; i++)
            {
                ret = rm_movel(robotHandlePtr, N_or_Z ? c4 : c2, vol, r, trajectory_connect, block);
                if (ret != 0)
                {
                    MessageBox.Show("[rm_movel] result :" + ret);
                    return ret;
                }

                ret = rm_movel(robotHandlePtr, N_or_Z ? c5 : c3, vol, r, trajectory_connect, block);
                if (ret != 0)
                {
                    MessageBox.Show("[rm_movel] result :" + ret);
                    return ret;
                }
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
            return ret;
        }

        private void ProcessBitmapPixels(Bitmap bitmap)
        {
            Color pixelColor;
            int max_p = 0, max_x_p = 0, max_y_p = 0, is_p = 0;
            int max_q = 0, max_x_q = 0, max_y_q = 0;
            int max_o = 0, max_x_o = 0, max_y_o = 0;
            int max = 0, min = 255;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    max = Math.Max(bitmap.GetPixel(x, y).R, max);
                    min = Math.Min(bitmap.GetPixel(x, y).R, min);
                }
            }
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.R >= max-5)
                    {
                        if (is_p == 0)
                        {
                            is_p = 1;
                            max_p = pixelColor.R;
                            max_x_p = x;
                            max_y_p = y;
                        }
                        if (y > max_y_q)
                        {
                            max_q = pixelColor.R;
                            max_x_q = x;
                            max_y_q = y;
                        }
                        if (x > max_x_o)
                        {
                            max_o = pixelColor.R;
                            max_x_o = x;
                            max_y_o = y;
                        }
                    }
                }
            }
            MessageBox.Show(max + "||" + max_x_p + "," + max_y_p + ":" + max_p + "\n"
                            + max_x_q + "," + max_y_q + ":" + max_q + "\n"
                            + max_x_o + "," + max_y_o + ":" + max_o);
        }

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;
        }

        private void stop_Click(object sender, EventArgs e)
        {
            rm_set_arm_stop(_robotHandlePtr);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 1. 销毁机械臂连接
            if (_robotHandlePtr != IntPtr.Zero)
            {
                rm_destroy_robot_arm(_robotHandlePtr);
                _robotHandlePtr = IntPtr.Zero;
                MessageBox.Show("机械臂句柄已释放。");
            }

            // 2. 清理SDK资源
            rm_cleanup();
            MessageBox.Show("SDK资源已清理。");
        }

        private void start_Click(object sender, EventArgs e)
        {
            // 调用C语言睿尔曼机械臂开发包动态库中的 rm_init 函数  
            // 初始化为三线程模式
            _ = rm_init(rm_thread_mode_e.RM_TRIPLE_MODE_E);
            _robotHandlePtr = rm_create_robot_arm("192.168.22.18", 8080);
            rm_robot_handle robotHandle = (rm_robot_handle)Marshal.PtrToStructure(_robotHandlePtr, typeof(rm_robot_handle))!;
            if (_robotHandlePtr != IntPtr.Zero && robotHandle.id > 0)
            {
                MessageBox.Show("[rm_create_robot_arm] connect success, handle id:robotHandle.id\n");
            }
            else
            {
                MessageBox.Show("[rm_create_robot_arm] connect error:robotHandle.id\n");
            }
            c_ini.position.x = 0;
            c_ini.position.y = 0;
            c_ini.position.z = 0.5f;
            c_ini.euler.rx = -3.14f;
            c_ini.euler.ry = (float)(-1.570f + Math.PI / 23);
            c_ini.euler.rz = 3.14f;
            rm_change_work_frame(_robotHandlePtr, "Base");
            int ret = rm_movej_p(_robotHandlePtr, c_ini, 20, 0, 0, 1);
            if (ret != 0) MessageBox.Show("[rm_move_joint] Error occurred: " + ret);
        }

        private void start1_Click(object sender, EventArgs e)
        {
            realsense_Thread();
        }

        void realsense_Thread()
        {
            //配置相机
            var cfg = new Config();

            using (var ctx = new Context())
            {
                //相机设备
                var devices = ctx.QueryDevices();
                var dev = devices[0];

                var sensors = dev.QuerySensors();
                var depthSensor = sensors[0];
                var colorSensor = sensors[1];

                //配置深度相机
                var depthProfile = depthSensor.StreamProfiles
                                    .Where(p => p.Stream == Intel.RealSense.Stream.Depth)
                                    .OrderBy(p => p.Framerate)
                                    .Select(p => p.As<VideoStreamProfile>()).First();

                //配置彩色相机
                var colorProfile = colorSensor.StreamProfiles
                                    .Where(p => p.Stream == Intel.RealSense.Stream.Color)
                                    .OrderBy(p => p.Framerate)
                                    .Select(p => p.As<VideoStreamProfile>()).First();

                //配置相机宽高，帧数
                cfg.EnableStream(Intel.RealSense.Stream.Depth, 640, 480, Format.Z16, 30);
                cfg.EnableStream(Intel.RealSense.Stream.Color, 640, 480, Format.Bgr8, 30);
            }

            var pipe = new Pipeline();

            //启动相机并应用配置
            PipelineProfile pp = pipe.Start(cfg);

            var profile = pp.GetStream(Intel.RealSense.Stream.Depth).As<VideoStreamProfile>();
            var intrinsics = profile.GetIntrinsics();

            using (var frames = pipe.WaitForFrames(5000))
            {
                //对齐深度图像
                Align align = new Align(Intel.RealSense.Stream.Color).DisposeWith(frames);

                //自带过滤器，实间，空间，孔洞填充等
                SpatialFilter spat_filter = new SpatialFilter();
                TemporalFilter temporal = new TemporalFilter();
                HoleFillingFilter holoFillingFilter = new HoleFillingFilter();

                Intel.RealSense.Frame aligned = spat_filter.Process(frames).DisposeWith(frames);
                aligned = holoFillingFilter.Process(aligned).DisposeWith(frames);
                aligned = temporal.Process(aligned).DisposeWith(frames);
                aligned = align.Process(aligned).DisposeWith(frames);

                FrameSet alignedframeset = aligned.As<FrameSet>().DisposeWith(frames);

                //对其后的深度图像
                var depthFrame = alignedframeset.DepthFrame.DisposeWith(alignedframeset);

                //对其后的彩色图像
                var colorFrame = alignedframeset.ColorFrame.DisposeWith(alignedframeset);

                rm_position_t q_tool;
                rm_position_t p_tool;
                rm_position_t o_tool;
                rm_current_arm_state_t state = new rm_current_arm_state_t();
                q_tool.z = depthFrame.GetDistance(180, 20);
                q_tool.x = (180 - intrinsics.ppx) * q_tool.z / intrinsics.fx;
                q_tool.y = (20 - intrinsics.ppy) * q_tool.z / intrinsics.fy;
                p_tool.z = depthFrame.GetDistance(180, 40);
                p_tool.x = (180 - intrinsics.ppx) * p_tool.z / intrinsics.fx;
                p_tool.y = (40 - intrinsics.ppy) * p_tool.z / intrinsics.fy;
                o_tool.z = depthFrame.GetDistance(200, 20);
                o_tool.x = (200 - intrinsics.ppx) * o_tool.z / intrinsics.fx;
                o_tool.y = (20 - intrinsics.ppy) * o_tool.z / intrinsics.fy;
                rm_get_current_arm_state(_robotHandlePtr, ref state);
                rm_position_t q_base = CoordinateTransformer.TransformPoint(q_tool, state.pose);
                rm_position_t p_base = CoordinateTransformer.TransformPoint(p_tool, state.pose);
                rm_position_t o_base = CoordinateTransformer.TransformPoint(o_tool, state.pose);
                rm_pose_t set_base = CoordinateTransformCalculator.CalculatePose(q_base, p_base, o_base);
                rm_delete_work_frame(_robotHandlePtr, "work1");
                rm_set_manual_work_frame(_robotHandlePtr, "work1", set_base);
                int ret = rm_change_work_frame(_robotHandlePtr, "work1");
                if (ret == 0) MessageBox.Show("pose success!");
                else MessageBox.Show("pose fail!");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Demo_Move_Cmd(_robotHandlePtr, 0.5f, 0.05f, 0.02f, false, 2, 30);
            rm_change_work_frame(_robotHandlePtr, "Base");
            rm_movej_p(_robotHandlePtr, c_ini, 20, 0, 0, 1);
            rm_change_work_frame(_robotHandlePtr, "work1");
        }
    }

    /// 矩阵运算工具类（3x3矩阵乘法、4x4矩阵乘法、向量乘法）
    public static class MatrixTool
    {
        /// <summary>
        /// 3x3矩阵乘法：A * B
        /// </summary>
        public static double[,] Multiply3x3(double[,] A, double[,] B)
        {
            double[,] result = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = A[i, 0] * B[0, j] + A[i, 1] * B[1, j] + A[i, 2] * B[2, j];
                }
            }
            return result;
        }

        /// <summary>
        /// 3x3矩阵 × 3x1向量
        /// </summary>
        public static double[] Multiply3x3Vector(double[,] mat, double[] vec)
        {
            double[] result = new double[3];
            for (int i = 0; i < 3; i++)
            {
                result[i] = mat[i, 0] * vec[0] + mat[i, 1] * vec[1] + mat[i, 2] * vec[2];
            }
            return result;
        }

        /// <summary>
        /// 4x4齐次矩阵 × 4x1齐次向量
        /// </summary>
        public static double[] Multiply4x4Vector(double[,] mat4x4, double[] vec4)
        {
            double[] result = new double[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = mat4x4[i, 0] * vec4[0] + mat4x4[i, 1] * vec4[1] +
                            mat4x4[i, 2] * vec4[2] + mat4x4[i, 3] * vec4[3];
            }
            return result;
        }
    }

    /// 坐标变换核心类>
    public static class CoordinateTransformer
    {
        /// <summary>
        /// 计算Q点在B坐标系下的坐标
        /// </summary>
        /// <param name="qInA">Q点在A坐标系下的坐标</param>
        /// <param name="poseAInB">A坐标系相对于B坐标系的位姿（平移x,y,z；姿态rx,ry,rz，单位：弧度）</param>
        /// <returns>Q点在B坐标系下的坐标</returns>
        public static rm_position_t TransformPoint(rm_position_t qInA, rm_pose_t poseAInB)
        {
            // 1. 计算旋转矩阵 R（Z-Y-X 欧拉角）
            double alpha = poseAInB.euler.rx; // 绕X轴旋转角
            double beta = poseAInB.euler.ry - Math.PI / 23;  // 绕Y轴旋转角
            double gamma = poseAInB.euler.rz + (float)Math.PI / 2; // 绕Z轴旋转角

            double cosA = Math.Cos(alpha);
            double sinA = Math.Sin(alpha);
            double cosB = Math.Cos(beta);
            double sinB = Math.Sin(beta);
            double cosG = Math.Cos(gamma);
            double sinG = Math.Sin(gamma);

            // 构造Z-Y-X欧拉角对应的旋转矩阵 R
            double[,] R = new double[3, 3]
            {
                { cosB * cosG,  -cosB * sinG,  sinB },
                { cosA * sinG + sinA * sinB * cosG,  cosA * cosG - sinA * sinB * sinG,  -sinA * cosB },
                { sinA * sinG - cosA * sinB * cosG,  sinA * cosG + cosA * sinB * sinG,  cosA * cosB }
            };

            // 2. 构造4x4齐次变换矩阵 T_B←A
            double[,] T = new double[4, 4]
            {
                { R[0,0], R[0,1], R[0,2], poseAInB.position.x },
                { R[1,0], R[1,1], R[1,2], poseAInB.position.y },
                { R[2,0], R[2,1], R[2,2], poseAInB.position.z },
                { 0,      0,      0,      1          }
            };

            // 3. 将Q点转换为齐次坐标 [x,y,z,1]
            double[] qInA_Homogeneous = new double[] { qInA.x, qInA.y, qInA.z, 1.0 };

            // 4. 矩阵乘法：T * Q_A（齐次坐标）
            double[] qInB_Homogeneous = MatrixTool.Multiply4x4Vector(T, qInA_Homogeneous);

            // 5. 转换回3D坐标（齐次坐标最后一位为1，直接取前三位）
            rm_position_t qInB = new();
            qInB.x = (float)qInB_Homogeneous[0];
            qInB.y = (float)qInB_Homogeneous[1];
            qInB.z = (float)qInB_Homogeneous[2];
            return qInB;
        }
    }

    public class Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Length
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        // 修复运算符重载：至少一个参数是Vector3D类型
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3D operator *(Vector3D v, double scalar)
        {
            return new Vector3D(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Vector3D operator *(double scalar, Vector3D v)
        {
            return new Vector3D(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Vector3D operator /(Vector3D v, double scalar)
        {
            return new Vector3D(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }

        public void Normalize()
        {
            double length = Length;
            if (length > 1e-10)
            {
                X /= length;
                Y /= length;
                Z /= length;
            }
        }

        public Vector3D GetNormalized()
        {
            double length = Length;
            if (length <= 1e-10)
                return new Vector3D(0, 0, 0);
            return new Vector3D(X / length, Y / length, Z / length);
        }

        public static double Dot(Vector3D v1, Vector3D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector3D Cross(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X
            );
        }

        public override string ToString()
        {
            return $"({X:F3}, {Y:F3}, {Z:F3})";
        }
    }

    public class CoordinateTransformCalculator
    {
        /// <summary>
        /// 从两个点创建向量
        /// </summary>
        public static Vector3D CreateVector(rm_position_t from, rm_position_t to)
        {
            return new Vector3D(to.x - from.x, to.y - from.y, to.z - from.z);
        }

        /// <summary>
        /// 计算B坐标系在A坐标系下的位姿
        /// </summary>
        /// <param name="p1">B坐标系原点在A下的坐标</param>
        /// <param name="p2">B坐标系X轴上一点在A下的坐标</param>
        /// <param name="p3">B坐标系XY平面内一点在A下的坐标</param>
        /// <returns>B在A下的位姿</returns>
        public static rm_pose_t CalculatePose(rm_position_t p1, rm_position_t p2, rm_position_t p3)
        {
            // 1. 计算B坐标系的原点（即p1点）
            rm_position_t originB = p1;

            // 2. 计算B坐标系的X轴方向向量
            Vector3D xAxis = CreateVector(p1, p2);
            xAxis = xAxis.GetNormalized();

            // 3. 计算B坐标系的Y轴和Z轴方向向量
            // 首先计算从p1到p3的向量
            Vector3D p1ToP3 = CreateVector(p1, p3);

            // 计算Z轴（通过叉乘）
            Vector3D zAxis = Vector3D.Cross(xAxis, p1ToP3);
            zAxis = zAxis.GetNormalized();

            // 重新计算Y轴（确保正交）
            Vector3D yAxis = Vector3D.Cross(zAxis, xAxis);
            yAxis = yAxis.GetNormalized();

            // 4. 构建旋转矩阵
            double[,] rotationMatrix = new double[3, 3]
            {
                { xAxis.X, yAxis.X, zAxis.X },
                { xAxis.Y, yAxis.Y, zAxis.Y },
                { xAxis.Z, yAxis.Z, zAxis.Z }
            };

            // 5. 将旋转矩阵转换为欧拉角（ZYX顺序）
            double rx, ry, rz;
            RotationMatrixToEulerAngles(rotationMatrix, out rx, out ry, out rz);
            rm_pose_t ret = new rm_pose_t();
            ret.position.x = originB.x;
            ret.position.y = originB.y;
            ret.position.z = originB.z;
            ret.euler.rx = (float)rx;
            ret.euler.ry = (float)ry;
            ret.euler.rz = (float)rz;
            return ret;
        }

        /// <summary>
        /// 将旋转矩阵转换为欧拉角（ZYX顺序）
        /// </summary>
        private static void RotationMatrixToEulerAngles(double[,] m, out double rx, out double ry, out double rz)
        {
            double m11 = m[0, 0], m12 = m[0, 1], m13 = m[0, 2];
            double m21 = m[1, 0], m22 = m[1, 1], m23 = m[1, 2];
            double m31 = m[2, 0], m32 = m[2, 1], m33 = m[2, 2];

            // 计算欧拉角（ZYX顺序 - 先绕Z轴，再绕Y轴，最后绕X轴）
            if (Math.Abs(m31) < 1.0 - 1e-6)
            {
                ry = -Math.Asin(m31);
                rx = Math.Atan2(m32, m33);
                rz = Math.Atan2(m21, m11);
            }
            else
            {
                // 万向锁情况
                rz = 0;
                if (m31 > 0)
                {
                    ry = -Math.PI / 2;
                    rx = Math.Atan2(m12, m13);
                }
                else
                {
                    ry = Math.PI / 2;
                    rx = Math.Atan2(-m12, -m13);
                }
            }
        }

        /// <summary>
        /// 验证计算结果：将B坐标系下的点转换回A坐标系
        /// </summary>
        public static rm_position_t TransformPointFromBToA(rm_position_t pointInB, rm_pose_t poseBInA)
        {
            // 创建旋转矩阵（ZYX顺序）
            double cx = Math.Cos(poseBInA.euler.rx);
            double sx = Math.Sin(poseBInA.euler.rx);
            double cy = Math.Cos(poseBInA.euler.ry);
            double sy = Math.Sin(poseBInA.euler.ry);
            double cz = Math.Cos(poseBInA.euler.rz);
            double sz = Math.Sin(poseBInA.euler.rz);

            // ZYX顺序的旋转矩阵
            double[,] rotMatrix = new double[3, 3]
            {
                { cy * cz, cz * sx * sy - cx * sz, cx * cz * sy + sx * sz },
                { cy * sz, cx * cz + sx * sy * sz, -cz * sx + cx * sy * sz },
                { -sy, cy * sx, cx * cy }
            };

            // 应用旋转和平移
            double x = rotMatrix[0, 0] * pointInB.x + rotMatrix[0, 1] * pointInB.y + rotMatrix[0, 2] * pointInB.z + poseBInA.position.x;
            double y = rotMatrix[1, 0] * pointInB.x + rotMatrix[1, 1] * pointInB.y + rotMatrix[1, 2] * pointInB.z + poseBInA.position.y;
            double z = rotMatrix[2, 0] * pointInB.x + rotMatrix[2, 1] * pointInB.y + rotMatrix[2, 2] * pointInB.z + poseBInA.position.z;

            rm_position_t ret = new rm_position_t();
            ret.x = (float)x;
            ret.y = (float)y;
            ret.z = (float)z;
            return ret;
        }

        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        public static double Distance(rm_position_t p1, rm_position_t p2)
        {
            double dx = p1.x - p2.x;
            double dy = p1.y - p2.y;
            double dz = p1.z - p2.z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }

    
}

