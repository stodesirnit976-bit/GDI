using GDI.Core;
using GDI.Models;
using Intel.RealSense;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static GDI.Services.Arm;

namespace GDI.Services
{
    public class CameraService
    {
        private Pipeline pipe;

        private CancellationTokenSource cts;
        private Thread camServ;

        public Action<Bitmap, Bitmap> camAction;
        //public Action<Bitmap> rsAction;
        public Action<Bitmap, DepthFrame, Intrinsics> CDAction;    //吴名添加

        private void cam_Thread(CancellationToken token)
        {
            var cfg = new Config();
            pipe = new Pipeline();
            var intrinsics = new Intrinsics();    //吴名添加

            // 配置相机
            // 这一段是照抄官方的 tutorial
            // https://github.com/realsenseai/librealsense/blob/master/wrappers/csharp/tutorial/capture/Window.xaml.cs
            using (var ctx = new Context())
            {
                // 相机设备
                var devices = ctx.QueryDevices();
                var dev = devices[0];

                var sensors = dev.QuerySensors();
                var depthSensor = sensors[0];
                var colorSensor = sensors[1];

                // 配置深度相机
                var depthProfile = depthSensor.StreamProfiles
                                    .Where(p => p.Stream == Intel.RealSense.Stream.Depth)
                                    .OrderBy(p => p.Framerate)
                                    .Select(p => p.As<VideoStreamProfile>()).First();

                // 配置彩色相机
                var colorProfile = colorSensor.StreamProfiles
                                    .Where(p => p.Stream == Intel.RealSense.Stream.Color)
                                    .OrderBy(p => p.Framerate)
                                    .Select(p => p.As<VideoStreamProfile>()).First();
    
                // 配置相机宽高，帧数
                cfg.EnableStream(Intel.RealSense.Stream.Depth, 640, 480, Format.Z16, 30);
                // 这里注意 GDI Bitmap 中，像素排列是 BGR 而不是 RGB
                cfg.EnableStream(Intel.RealSense.Stream.Color, 640, 480, Format.Bgr8, 30);
                // 启动相机                
            }

            PipelineProfile pp = pipe.Start(cfg);
            Console.WriteLine("相机已启动");
            var profile = pp.GetStream(Intel.RealSense.Stream.Depth).As<VideoStreamProfile>();
            intrinsics = profile.GetIntrinsics();

            // 自带过滤器，实间，空间，孔洞填充等
            Align align = new Align(Intel.RealSense.Stream.Color);
            SpatialFilter spat_filter = new SpatialFilter();
            TemporalFilter temporal = new TemporalFilter();
            HoleFillingFilter holoFillingFilter = new HoleFillingFilter();
            Colorizer color_map = new Colorizer();

            DateTime lastCallbackTime = DateTime.Now;
            bool is_CD = false;                                      //吴名添加

            while (!token.IsCancellationRequested)
            {
                using (var frames = pipe.WaitForFrames(5000))
                {
                    // 创建对齐器
                    //Align align = new Align(Intel.RealSense.Stream.Color).DisposeWith(frames);   
                    // 1.空间滤波
                    Intel.RealSense.Frame aligned = spat_filter.Process(frames).DisposeWith(frames);
                    // 2.孔洞填充
                    aligned = holoFillingFilter.Process(aligned).DisposeWith(frames);
                    // 3.时间滤波
                    aligned = temporal.Process(aligned).DisposeWith(frames);
                    // 4.执行对齐
                    aligned = align.Process(aligned).DisposeWith(frames);
                    // 5.类型转换，把 Frame类 转换成 FrameSet 类
                    FrameSet alignedframeset = aligned.As<FrameSet>().DisposeWith(frames);

                    // 对齐后的深度图像
                    var depthFrame = alignedframeset.DepthFrame.DisposeWith(alignedframeset);
                    // 深度图像上色
                    var colorizedDepth = color_map.Process<VideoFrame>(depthFrame).DisposeWith(alignedframeset);
                    // 对齐后的相机图像
                    var colorFrame = alignedframeset.ColorFrame.DisposeWith(alignedframeset);

                    // 由于 UI线程可能比相机读取慢，这边更新完 UI可能还没更新，会导致 UI画面有缺失
                    // 所以不能在这里 dispose bitmap，在 UI线程中委托回调的函数里执行
                    // FrameSet 转 Bitmap
                    Bitmap ColorBitmap = FrameToBitmap(colorFrame);
                    Bitmap DepthColorBitmap = FrameToBitmap(colorizedDepth);

                    // 转Bitmap
                    // 直接用bitmap复制不安全，因为此时 bitmap 只是指向了原图的内存地址，需要用 bitmap img = new bitmap(rawimg);复制一份
                    // 相机/深度图显示到 UI窗口
                    camAction(ColorBitmap, DepthColorBitmap);

                    if (!is_CD && DateTime.Now - lastCallbackTime >= TimeSpan.FromSeconds(6))
                    {
                        is_CD = true;
                        CDAction(ColorBitmap, depthFrame, intrinsics);          //吴名添加
                    }
                }
            }
            if (pipe != null)
            {
                try { pipe.Stop(); } catch { } // 防止重复 Stop 报错
                pipe = null; // 清空引用
            }
            Console.WriteLine("相机已停止");
        }


        // Frame 转 Bitmap, 防止内存泄漏
        public static Bitmap FrameToBitmap(VideoFrame frame)
        {
            // 检查宽高，防止空帧
            if (frame.Width == 0 || frame.Height == 0) return null;
            // 1. 创建一个临时的 Bitmap，它只是指向 frame 的内存
            using (var tempBitmap = new Bitmap(frame.Width, frame.Height, frame.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, frame.Data))
            {
                var CopyBitmap = new Bitmap(tempBitmap);
                return CopyBitmap;
            }
        }

        // 启动相机线程
        public void cam_Thread_start()
        {
            cts = new CancellationTokenSource();
            camServ = new Thread(()=> cam_Thread(cts.Token));
            camServ.IsBackground = true;
            camServ.Start();
        }
        // 停止相机线程
        public void cam_Thread_stop()
        {
            cts?.Cancel();
            
        }


        //处理彩色图定位
        private int[] ProcessBitmapPixels(Bitmap bitmap)
        {
            Color pixelColor;
            int max_x_p = 0, max_y_p = 0;
            int max_x_o = 0, max_y_o = 0;
            int[] res = new int[6];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixelColor = bitmap.GetPixel(x, y);
                    if ((pixelColor.G > pixelColor.B * 3 / 2) && (pixelColor.R > pixelColor.B * 3 / 2))
                    {
                        if (y > max_y_p)
                        {
                            max_x_p = x;
                            max_y_p = y;
                        }
                        if (x > max_x_o)
                        {
                            max_x_o = x;
                            max_y_o = y;
                        }
                    }
                }
            }
            int max_x_q = max_x_o, max_y_q = max_y_p;
            for (int y = bitmap.Height - 1; y >= 0; y--)
            {
                for (int x = bitmap.Width - 1; x >= 0; x--)
                {
                    pixelColor = bitmap.GetPixel(x, y);
                    if ((pixelColor.G > pixelColor.B * 3 / 2) && (pixelColor.R > pixelColor.B * 3 / 2))
                    {
                        if (x < max_x_q && y < max_y_q)
                        {
                            max_x_q = x;
                            max_y_q = y;
                        }
                    }
                }
            }
            res[0] = max_x_q; res[1] = max_y_q;
            res[2] = max_x_p; res[3] = max_y_p;
            res[4] = max_x_o; res[5] = max_y_o;
            return res;
        }

        //标定方法
        public void Init(Bitmap Cimg, DepthFrame Dimg, Intrinsics Intt)
        {
            rm_position_t q_tool;
            rm_position_t p_tool;
            rm_position_t o_tool;
            rm_current_arm_state_t state = new rm_current_arm_state_t();
            ProcessBitmapPixels(Cimg);
            int[] res = ProcessBitmapPixels(Cimg);
            q_tool.z = Dimg.GetDistance(res[0], res[1]);
            q_tool.x = (res[0] - Intt.ppx) * q_tool.z / Intt.fx;
            q_tool.y = (res[1] - Intt.ppy) * q_tool.z / Intt.fy;
            p_tool.z = Dimg.GetDistance(res[2], res[3]);
            p_tool.x = (res[2] - Intt.ppx) * p_tool.z / Intt.fx;
            p_tool.y = (res[3] - Intt.ppy) * p_tool.z / Intt.fy;
            o_tool.z = Dimg.GetDistance(res[4], res[5]);
            o_tool.x = (res[4] - Intt.ppx) * o_tool.z / Intt.fx;
            o_tool.y = (res[5] - Intt.ppy) * o_tool.z / Intt.fy;
            rm_change_work_frame(Arm.Instance.robotHandlePtr, "Base");
            rm_get_current_arm_state(Arm.Instance.robotHandlePtr, ref state);
            rm_position_t q_base = CoordinateTransformer.TransformPoint(q_tool, state.pose);
            rm_position_t p_base = CoordinateTransformer.TransformPoint(p_tool, state.pose);
            rm_position_t o_base = CoordinateTransformer.TransformPoint(o_tool, state.pose);
            rm_pose_t set_base = CoordinateTransformCalculator.CalculatePose(q_base, p_base, o_base);
            rm_delete_work_frame(Arm.Instance.robotHandlePtr, "work1");
            rm_set_manual_work_frame(Arm.Instance.robotHandlePtr, "work1", set_base);
            int ret = rm_change_work_frame(Arm.Instance.robotHandlePtr, "work1");
            if (ret == 0) MessageBox.Show("ini success!");
            else MessageBox.Show("pose fail!");
        }


    }
}
