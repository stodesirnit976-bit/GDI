using GDI.Core;
using GDI.Models;
using Intel.RealSense;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static GDI.Services.Arm;

namespace GDI.Services
{
    public class Camera
    {
        private Pipeline pipe;

        private CancellationTokenSource cts;
        private Thread camServ;

        public Action<Bitmap, Bitmap> camAction;
        //public Action<Bitmap> rsAction;
        //public Action<Bitmap, DepthFrame, Intrinsics> CDAction;    //吴名添加

        // 对外输出一帧图像的事件
        public event Action<Bitmap, Bitmap, DepthFrame,Intrinsics> cam_Event;
        PipelineProfile pp;
        
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


                // 高精度预设
                //if (depthSensor.Options.Supports(Option.VisualPreset))
                //{
                //    depthSensor.Options[Option.VisualPreset].Value = 2; // High Accuracy模式

                //}
                //// 关键参数优化
                ////Console.WriteLine($"VisualPreset 范围: {range.Min} 到 {range.Max}");
                //depthSensor.Options[Option.LaserPower].Value = 360;

                //depthSensor.Options[Option.EnableAutoExposure].Value = 1; // 关闭自动曝光
                //var range1 = depthSensor.Options[Option.Exposure];       // 固定曝光值

                ////depthSensor.Options[Option.Exposure].Value = 300;       // 固定曝光值
                ////depthSensor.Options[Option.Gain].Value = 16;            // 固定增益
                ////// 强制开启激光发射器(即使在强光下)
                //var range = depthSensor.Options[Option.EmitterEnabled];
                //depthSensor.Options[Option.EmitterEnabled].Value = 1; // 开启激光
                //depthSensor.Options[Option.LaserPower].Value = 360f; // 最大功率
                //Console.WriteLine($"激光发射器 范围: {range.Min} 到 {range.Max}");
                //Console.WriteLine($"曝光 范围: {range1.Min} 到 {range1.Max}");
                



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

                var sensor = dev.QuerySensors()[0];

                //// 设置高精度
                //if (sensor.Options.Supports(Option.VisualPreset))
                //{
                //    try { sensor.Options[Option.VisualPreset].Value = 3f; } catch { }
                //}

                //if (colorSensor.Options.Supports(Option.AutoExposurePriority))
                //{
                //    try { colorSensor.Options[Option.AutoExposurePriority].Value = 0f; } catch { }
                //}

                //var range = depthSensor.Options[Option.VisualPreset];
                // 示例：检查高精度预设是否设置成功
                float currentPreset = depthSensor.Options[Option.VisualPreset].Value;
                Console.WriteLine($"当前VisualPreset值为: {currentPreset}");

                // 配置相机宽高，帧数
                cfg.EnableStream(Intel.RealSense.Stream.Depth, 1280, 720, Format.Z16, 30);
                // 这里注意 GDI Bitmap 中，像素排列是 BGR 而不是 RGB
                cfg.EnableStream(Intel.RealSense.Stream.Color, 1280, 720, Format.Bgr8, 30);
            }

            // 启动相机     
            pp = pipe.Start(cfg);
            Console.WriteLine("相机已启动");
            
            // 获取相机内参
            //var profile = pp.GetStream(Intel.RealSense.Stream.Depth).As<VideoStreamProfile>();
            //intrinsics = profile.GetIntrinsics();
            var profile = pp.GetStream(Intel.RealSense.Stream.Color).As<VideoStreamProfile>();
            intrinsics = profile.GetIntrinsics();

            // 对齐过滤器
            Align align = new Align(Intel.RealSense.Stream.Color);
            // 过滤器：实间，空间，孔洞填充等
            SpatialFilter spat_filter = new SpatialFilter();
            TemporalFilter temporal = new TemporalFilter();
            HoleFillingFilter holoFillingFilter = new HoleFillingFilter();
            Colorizer color_map = new Colorizer();



            //// 空间滤波器 - 适度平滑
            //spat_filter.Options[Option.FilterMagnitude].Value = 2;    // 中等强度
            //spat_filter.Options[Option.HolesFill].Value = 1;          // 轻微孔洞填充

            // 时间滤波器 - 重要！提高稳定性
            //temporal.Options[Option.FilterSmoothAlpha].Value = 0.4f;  // 适中平滑
            //temporal.Options[Option.FilterSmoothDelta].Value = 20;    // 深度差异阈值

            // 孔洞填充 - 谨慎使用
            //holoFillingFilter.Options[Option.HolesFill].Value = 1;     // 轻度填充
            //temporal.Options[Option.HolesFill].Value = 3f;
            //holoFillingFilter.Options[Option.HolesFill].Value = 2f; // Nearest

            while (!token.IsCancellationRequested)
            {
                using (var frames = pipe.WaitForFrames(5000))
                {
                    // 1.空间滤波
                    Frame aligned = spat_filter.Process(frames).DisposeWith(frames);
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
                    // 抛事件
                    cam_Event?.Invoke(ColorBitmap, DepthColorBitmap, depthFrame, intrinsics);

                    ColorBitmap?.Dispose();
                    DepthColorBitmap?.Dispose();
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
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
            camServ = null;
            pp.Dispose();
            pipe.Dispose();
        }

       

    }
}
