using GDI.Models;
using Intel.RealSense;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GDI.Services
{
    public class CameraService
    {
        private Pipeline pipe;

        private CancellationTokenSource cts;
        private Thread camServ;

        public Action<Bitmap, Bitmap> camAction;
        public Action<Bitmap> rsAction;

        private void cam_Thread(CancellationToken token)
        {
            var cfg = new Config();
            pipe = new Pipeline();
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
                PipelineProfile pp = pipe.Start(cfg);
                Console.WriteLine("相机已启动");
            }

            // 自带过滤器，实间，空间，孔洞填充等
            Align align = new Align(Intel.RealSense.Stream.Color);
            SpatialFilter spat_filter = new SpatialFilter();
            TemporalFilter temporal = new TemporalFilter();
            HoleFillingFilter holoFillingFilter = new HoleFillingFilter();
            Colorizer color_map = new Colorizer();
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

                }
            }
            if (pipe != null)
            {
                try { pipe.Stop(); } catch { } // 防止重复 Stop 报错
                pipe = null; // 清空引用
            }

            Console.WriteLine("相机已停止");
        }

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

        public void cam_Thread_start()
        {
            cts = new CancellationTokenSource();
            camServ = new Thread(()=> cam_Thread(cts.Token));
            camServ.IsBackground = true;
            camServ.Start();
        }

        public void cam_Thread_stop()
        {
            cts?.Cancel();
            
        } 
    }
}
