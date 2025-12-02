using GDI.Core;
using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GDI.Services.Arm;


namespace GDI.Services.CameraServices
{
    public class Calibration
    {
        private bool isCalibrating = false;
        private bool isCalibrating2 = false;//wm修改
        private DateTime startTime;
        private Camera cam;

        public void Calibration_subCamEvent(Camera cam)
        {
            if (isCalibrating) return;
            isCalibrating = true;

            this.cam = cam;
            startTime = DateTime.Now;

            // 订阅相机帧事件
            cam.cam_Event += imgProcessor;
        }

        rm_position_t q;
        private void imgProcessor(Bitmap ColorBitmap, Bitmap DepthColorBitmap, DepthFrame depthFrame, Intrinsics intrinsics)
        {
            //if ((DateTime.Now - startTime).TotalSeconds < 10) return;
        
            // 处理帧数据进行标定
            
            //for (int i = 0; i < 5; i++)
            //{
            //    rm_position_t q = AutoCalibrateWorkFrame(ColorBitmap, depthFrame, intrinsics);//wm修改
            //    Console.WriteLine("相机启动超过6秒，第一次标定");
            //    Thread.Sleep(2000);
            //}
            //Init(ColorBitmap, depthFrame, intrinsics);
            rm_change_work_frame(Arm.Instance.robotHandlePtr, "Base");
            rm_current_arm_state_t state = new rm_current_arm_state_t();
            rm_get_current_arm_state(Arm.Instance.robotHandlePtr, ref state);
            state.pose.position.x = q.x + 0.4f;
            state.pose.position.y = q.y + 0.2f;
            state.pose.position.z = q.z - 0.1f;
            rm_movej_p(Arm.Instance.robotHandlePtr, state.pose, 15, 0, 0, 1);

            // 标定完成，取消订阅相机帧事件
            cam.cam_Event -= imgProcessor;

            isCalibrating = false;
        }

        public void Calibration2_subCamEvent(Camera cam)//wm修改
        {
            if (isCalibrating2) return;
            isCalibrating2 = true;

            this.cam = cam;

            // 订阅相机帧事件
            cam.cam_Event += imgProcessor2;
        }
        //wm修改
        private void imgProcessor2(Bitmap ColorBitmap, Bitmap DepthColorBitmap, DepthFrame depthFrame, Intrinsics intrinsics)
        {
            //Console.WriteLine("二次标定");
            // 处理帧数据进行标定
            ColorBitmap.Save(@"D:\img\test.bmp");
            q = Init(ColorBitmap, depthFrame, intrinsics);
            


            // 标定完成，取消订阅相机帧事件
            cam.cam_Event -= imgProcessor2;

            isCalibrating2 = false;
        }

        public  void backToStart()
        {
            rm_change_work_frame(Arm.Instance.robotHandlePtr, "Base");
            rm_movej(Arm.Instance.robotHandlePtr, c_ini, 15, 0, 0, 0);
            rm_change_work_frame(Arm.Instance.robotHandlePtr, "work1");
        }







        // -------------- 提取三个红色 blob 并返回质心（按 Q=左上, P=左下, O=右上） --------------
        private List<PointF> ExtractRedBlobs(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            bool[,] mask = new bool[w, h];

            // 1. 二值化红色区域（根据最新样本调整阈值）
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (c.R >= 140 && c.G <= 160 && c.B <= 160 && c.R>c.G && c.R>c.B)
                    {
                        mask[x, y] = true;
                    }
                }
            }

            // 2. 连通域提取（BFS）
            List<List<Point>> blobs = new List<List<Point>>();
            bool[,] visited = new bool[w, h];
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (mask[x, y] && !visited[x, y])
                    {
                        List<Point> blob = new List<Point>();
                        Queue<Point> q = new Queue<Point>();
                        q.Enqueue(new Point(x, y));
                        visited[x, y] = true;

                        while (q.Count > 0)
                        {
                            var p = q.Dequeue();
                            blob.Add(p);

                            for (int k = 0; k < 4; k++)
                            {
                                int nx = p.X + dx[k];
                                int ny = p.Y + dy[k];
                                if (nx >= 0 && nx < w && ny >= 0 && ny < h &&
                                    mask[nx, ny] && !visited[nx, ny])
                                {
                                    visited[nx, ny] = true;
                                    q.Enqueue(new Point(nx, ny));
                                }
                            }
                        }

                        // 最小面积改成 3 个像素
                        if (blob.Count >= 8)
                            blobs.Add(blob);
                    }
                }
            }

            if (blobs.Count != 3)
                MessageBox.Show($"红色点数量错误，检测到 {blobs.Count} 个（应该是 3 个）。");

            // 3. 计算三个区域的质心
            List<PointF> centers = new List<PointF>();
            foreach (var blob in blobs)
            {
                float sx = 0, sy = 0;
                foreach (var p in blob)
                {
                    sx += p.X;
                    sy += p.Y;
                }
                centers.Add(new PointF(sx / blob.Count, sy / blob.Count));
            }

            // 4. 排序并映射到 Q(左上), P(左下), O(右上)
            var lefts = centers.OrderBy(p => p.X).Take(2).OrderBy(p => p.Y).ToList(); // 左侧两个，按Y升序
            var right = centers.OrderBy(p => p.X).Last();

            PointF Q = lefts[0];  // 左上
            PointF P = lefts[1];  // 左下
            PointF O = right;     // 右上（假设右上比左侧高）

            return new List<PointF>() { Q, P, O };
        }


        // -------------- 2D 图像平面共线性检测（像素平面） --------------
        // 使用面积法：三点共线当且仅当三角形面积接近 0
        private bool IsCollinear2D(PointF a, PointF b, PointF c, double areaEps = 1.0)
        {
            // 三角形面积 = 0.5 * |(b-a) x (c-a)|
            double area2 = Math.Abs((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X));
            // area2 是 2*area（以像素为单位）。当 area2 < areaEps 时视作近似共线
            return area2 < areaEps;
        }

        // -------------- 把像素 + 深度 转成 3D 坐标（相机坐标系） --------------
        private rm_position_t PixelToCameraPoint(PointF p, float depth, Intrinsics intr)
        {
            rm_position_t pt = new rm_position_t();
            pt.z = depth;
            pt.x = (p.X - intr.ppx) * depth / intr.fx;
            pt.y = (p.Y - intr.ppy) * depth / intr.fy;
            return pt;
        }

        // -------------- 3D 共线性检测（在相机坐标系下） --------------
        // 通过向量叉乘模长来判断：|(b-a) x (c-a)| / (|b-a|*|c-a|) < eps -> 共线
        private bool IsCollinear3D(rm_position_t a, rm_position_t b, rm_position_t c, double relEps = 0.01, double absEps = 0.003)
        {
            // 向量 ab, ac
            double abx = b.x - a.x, aby = b.y - a.y, abz = b.z - a.z;
            double acx = c.x - a.x, acy = c.y - a.y, acz = c.z - a.z;

            // 叉乘
            double cx = aby * acz - abz * acy;
            double cy = abz * acx - abx * acz;
            double cz = abx * acy - aby * acx;

            double crossNorm = Math.Sqrt(cx * cx + cy * cy + cz * cz);
            double lab = Math.Sqrt(abx * abx + aby * aby + abz * abz);
            double lac = Math.Sqrt(acx * acx + acy * acy + acz * acz);

            if (lab < 1e-6 || lac < 1e-6)
                return true; // 距离太小，视作退化 -> 共线/无法判定

            double rel = crossNorm / (lab * lac); // sin(theta)
                                                  // 如果相对指标很小或绝对 crossNorm 很小，都可认为共线
            if (rel < relEps || crossNorm < absEps) return true;
            return false;
        }

        // -------------- 局部深度平均（以像素坐标为中心，size 可调） --------------
        private float AvgDepthRegion(DepthFrame d, float cx, float cy, int radius = 3)
        {
            int xi = (int)Math.Round(cx);
            int yi = (int)Math.Round(cy);
            int wLimit = 0; // 如果需要边界检查可用 bmp.Width/bmp.Height 传入
            float sum = 0;
            int cnt = 0;
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    int nx = xi + dx;
                    int ny = yi + dy;
                    // 注意： 调用前确保 nx,ny 在图像内，或在这里做边界保护
                    try
                    {
                        float z = d.GetDistance(nx, ny);
                        if (!float.IsNaN(z) && z > 0.01f) // 过滤无效深度
                        {
                            sum += z;
                            cnt++;
                        }
                    }
                    catch
                    {
                        // 如果超出范围或失败，跳过
                    }
                }
            }
            if (cnt == 0) return 0;
            return sum / cnt;
        }

        // -------------- 主标定函数（整合以上检查） --------------
        public rm_position_t Init(Bitmap Cimg, DepthFrame Dimg, Intrinsics Intt)
        {
            // 1. 提取三个质心（按 Q=左上, P=左下, O=右上）
            List<PointF> pts;
            try
            {
                pts = ExtractRedBlobs(Cimg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("红点提取失败: " + ex.Message);
                throw;
            }
            PointF Qpix = pts[0];
            PointF Ppix = pts[1];
            PointF Opix = pts[2];

            // 2. 先做 2D 共线性检测（以像素为准）
            bool col2d = IsCollinear2D(Qpix, Ppix, Opix, areaEps: 2.0); // areaEps 可调（像素²）
            if (col2d)
            {
                MessageBox.Show("图像平面上三点接近共线，取消本次标定，请检查贴纸或摄像视角。");
                throw new Exception("2D points nearly collinear");
            }

            // 3. 取深度（局部平均）
            float qz = AvgDepthRegion(Dimg, Qpix.X, Qpix.Y, radius: 3);
            float pz = AvgDepthRegion(Dimg, Ppix.X, Ppix.Y, radius: 3);
            float oz = AvgDepthRegion(Dimg, Opix.X, Opix.Y, radius: 3);

            if (qz <= 0 || pz <= 0 || oz <= 0)
            {
                MessageBox.Show("深度读取失败或无效，取消标定。");
                throw new Exception("invalid depth");
            }

            // 4. 生成相机坐标下的 3D 点
            rm_position_t q_tool = PixelToCameraPoint(Qpix, qz, Intt);
            rm_position_t p_tool = PixelToCameraPoint(Ppix, pz, Intt);
            rm_position_t o_tool = PixelToCameraPoint(Opix, oz, Intt);

            // 5. 3D 共线性检测（更严格）
            //bool col3d = IsCollinear3D(q_tool, p_tool, o_tool, relEps: 0.02, absEps: 0.004);
            //if (col3d)
            //{
            //    MessageBox.Show("三维点接近共线（深度方向也一致），取消标定。建议改变摄像角度或使用更多标记点。");
            //    throw new Exception("3D points nearly collinear");
            //}

            // 6. 继续你原有的流程（坐标变换与设定工作系）
            rm_current_arm_state_t state = new rm_current_arm_state_t();
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

            return q_base;
        }









    }
}
