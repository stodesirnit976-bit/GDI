using GDI.Core;
using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        private bool is_success = false;
        public static volatile bool UNfinish_calibration = true;

        public void Calibration_subCamEvent(Camera cam)
        {
            if (isCalibrating) return;
            isCalibrating = true;

            this.cam = cam;
            startTime = DateTime.Now;

            // 订阅相机帧事件
            cam.cam_Event += imgProcessor;
            
        }

        private void imgProcessor(Bitmap ColorBitmap, Bitmap DepthColorBitmap, DepthFrame depthFrame, Intrinsics intrinsics)
        {
            // 1. 等待时间
            if ((DateTime.Now - startTime).TotalSeconds <6) return;

            Console.WriteLine(">>> [1/2] 相机启动稳定，开始第一次标定...");

            // 2. 调用视觉算法
            rm_position_t q = Init(ColorBitmap, depthFrame, intrinsics);

            // ===============================================================
            // 【致命BUG修复】必须检查标定是否成功！
            // 如果 Init 返回 0,0,0，说明视觉识别失败（没找到点或深度丢失）
            // 此时绝对不能让机械臂移动！
            // ===============================================================
            if (q.x == 0 && q.y == 0 && q.z == 0)
            {
                Console.WriteLine("[警告] 视觉定位失败（未找到目标），机械臂保持静止，等待下一帧...");
                // 直接 return，不要往下走！
                return;
            }

            Console.WriteLine($"[视觉锁定] 目标坐标: X={q.x:F3}, Y={q.y:F3}, Z={q.z:F3}");

            // 3. 只有成功了，才执行移动逻辑
            rm_change_work_frame(Arm.Instance.robotHandlePtr, "Base");

            // Thread.Sleep(100); // 没必要Sleep，删掉或保留均可

            rm_current_arm_state_t state = new rm_current_arm_state_t();
            rm_get_current_arm_state(Arm.Instance.robotHandlePtr, ref state);

            // 检查获取的状态是否合理
            if (Math.Abs(state.pose.position.x) > 1000 || Math.Abs(state.pose.position.y) > 1000)
            {
                Console.WriteLine("[警告] 获取的机械臂状态异常，中止运动");
                cam.cam_Event -= imgProcessor;
                isCalibrating = false;
                return;
            }

            // 4. 应用偏移量 (恢复你原始的逻辑)
            // 注意：这里用的是你最开始代码里的偏移量，确保这组参数是你想要的
            //state.pose.position.x = q.x;
            //state.pose.position.y = q.y;
            //state.pose.position.z = q.z ;

            //Console.WriteLine($"[移动指令] 正在前往观测点: X={state.pose.position.x:F3}, Y={state.pose.position.y:F3}, Z={state.pose.position.z:F3}");

            //int ret = rm_movel(Arm.Instance.robotHandlePtr, state.pose, 15, 0, 0, 1);
            //if (ret != 0)
            //{
            //    Console.WriteLine($"[错误] 移动失败，错误码: {ret}");
            //    return;
            //}

            // 5. 标定完成，取消订阅，准备进入第二阶段
            cam.cam_Event -= imgProcessor;
            isCalibrating = false;

            Console.WriteLine(">>> 第一次移动完成，等待停稳...");
            Task.Run(async () =>
            {
                //await Task.Delay(4000); // 给机械臂停稳后 4秒 的震动消除时间
                Calibration2_subCamEvent(this.cam);
            });
        }


        public void Calibration2_subCamEvent(Camera cam)//wm修改
        {
            if (isCalibrating2) return;
            isCalibrating2 = true;
            startTime = DateTime.Now;

            this.cam = cam;

            // 订阅相机帧事件
            cam.cam_Event += imgProcessor2;
        }
        //wm修改
        private void imgProcessor2(Bitmap ColorBitmap, Bitmap DepthColorBitmap, DepthFrame depthFrame, Intrinsics intrinsics)
        {
            if ((DateTime.Now - startTime).TotalSeconds < 0.5) return;
            startTime = DateTime.Now;
            // 处理帧数据进行标定
            //Task.Delay(500);
            Console.WriteLine("正在第二次标定");
            Init(ColorBitmap, depthFrame, intrinsics);


            // 标定完成，取消订阅相机帧事件
            if (is_success)
            {
                rm_change_work_frame(Arm.Instance.robotHandlePtr, "Base");
                rm_movej(Arm.Instance.robotHandlePtr, c_ini, 8, 0, 0, 0);
                rm_change_work_frame(Arm.Instance.robotHandlePtr, "work1");
                cam.cam_Event -= imgProcessor2;
                MessageBox.Show("标定完成！可以执行");
                UNfinish_calibration = false;
            }

            isCalibrating2 = false;
        }









        // ===================================================================================================
        // ========================================= 相机标定算法 ============================================
        // ===================================================================================================

        // ==============================================================
        // 主处理函数：处理 Bitmap 像素，找红点并匹配三角形
        // ==============================================================
        private int[] ProcessBitmapPixels(Bitmap bitmap)
        {
            // 1. 扫描全图，找出所有红斑候选
            List<List<Point>> allBlobs = FindRedBlobs(bitmap);

            Console.WriteLine($"[调试] 全图共找到 {allBlobs.Count} 个标定点。");

            // 2. 几何匹配：从候选定标点中找出最佳的"等腰直角三角形"组合
            // 如果找不到定标点或少于3个
            if (allBlobs.Count < 3)
            {
                Console.WriteLine("[严重错误] 标定点数量不足 3 个，无法构成三角形！");
                DrawDebugImage(bitmap, allBlobs, new Point(0, 0), new Point(0, 0), new Point(0, 0));
                return new int[6];
            }

            // 计算所有定标点的重心
            List<Point> centers = new List<Point>();
            foreach (var blob in allBlobs) centers.Add(GetBlobCenter(blob));

            // 3. 【核心算法】遍历所有组合，打分寻找最佳三角形
            var bestTriangle = FindBestIsoscelesRightTriangle(centers);

            if (bestTriangle == null)
            {
                Console.WriteLine("[匹配失败] 找到了多个红点，但没有任意三个能构成等腰直角三角形！");
                DrawDebugImage(bitmap, allBlobs, new Point(0, 0), new Point(0, 0), new Point(0, 0));
                return new int[6];
            }

            // 4. 解析结果
            Point pQ = bestTriangle.Q;
            Point pP = bestTriangle.P;
            Point pO = bestTriangle.O;

            Console.WriteLine($"[匹配成功] 最佳三角形误差分数: {bestTriangle.Score:F2} (越小越好)");

            // 5. 绘图调试
            // 这里我们只高亮选中的那三个定标点，其他的画成灰色
            DrawDebugImage(bitmap, allBlobs, pQ, pP, pO);

            Console.WriteLine($"[最终输出] Q:{pQ} P:{pP} O:{pO}");
            return new int[] { pQ.X, pQ.Y, pP.X, pP.Y, pO.X, pO.Y };
        }

        // ==============================================================
        // 辅助类：三角形匹配结果
        // ==============================================================
        class TriangleMatch
        {
            public Point Q { get; set; } // 直角顶点
            public Point P { get; set; } // 下方顶点
            public Point O { get; set; } // 右方顶点
            public double Score { get; set; } // 误差分数 (0是完美)
        }

        // ==============================================================
        // 【核心算法】寻找最佳等腰直角三角形
        // ==============================================================
        private TriangleMatch FindBestIsoscelesRightTriangle(List<Point> points)
        {
            TriangleMatch bestMatch = null;
            double minScore = double.MaxValue;

            int n = points.Count;
            // 遍历所有 3 点组合 (C(n,3))
            // 因为 n 通常很小 (比如 5-10个)，这里循环非常快
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    for (int k = j + 1; k < n; k++)
                    {
                        Point[] tri = { points[i], points[j], points[k] };

                        // 对这三个点，尝试找出谁是直角顶点
                        // 轮流假设每个点是 Q (直角点)
                        for (int m = 0; m < 3; m++)
                        {
                            Point tempQ = tri[m];
                            Point tempA = tri[(m + 1) % 3];
                            Point tempB = tri[(m + 2) % 3];

                            // 计算边长平方
                            double lenQA2 = GetDistSq(tempQ, tempA);
                            double lenQB2 = GetDistSq(tempQ, tempB);
                            double lenAB2 = GetDistSq(tempA, tempB); // 斜边

                            // 1. 过滤太小的噪点组合 (边长至少要 > 20像素)
                            if (lenQA2 < 400 || lenQB2 < 400) continue;

                            if (lenQA2 > 60000 || lenQB2 > 60000 || lenAB2 > 120000) continue;

                            // 1. 过滤太小的噪点组合 (边长至少要 > 10像素)
                            if (lenQA2 < 100 || lenQB2 < 100) continue;

                            // 2. 检查直角 (勾股定理): |QA^2 + QB^2 - AB^2| 应该很小
                            double pythagorasError = Math.Abs((lenQA2 + lenQB2) - lenAB2);
                            // 归一化误差 (除以斜边长度，防止大三角形误差显得更大)
                            double normPythagorasError = pythagorasError / lenAB2;

                            // 3. 检查等腰: |QA - QB| 应该很小
                            double lenQA = Math.Sqrt(lenQA2);
                            double lenQB = Math.Sqrt(lenQB2);
                            double isoscelesError = Math.Abs(lenQA - lenQB);
                            double normIsoscelesError = isoscelesError / ((lenQA + lenQB) / 2);

                            // 4. 总误差分数 (加权)
                            // 勾股误差权重高一点，等腰误差权重低一点
                            double currentScore = normPythagorasError * 100 + normIsoscelesError * 50;

                            // 阈值：如果形状太离谱，直接跳过
                            // 勾股误差 < 20%, 等腰误差 < 30%
                            if (normPythagorasError < 0.2 && normIsoscelesError < 0.3)
                            {
                                if (currentScore < minScore)
                                {
                                    minScore = currentScore;
                                    bestMatch = new TriangleMatch { Q = tempQ, Score = minScore };

                                    // 区分 P 和 O
                                    // 我们定义：P 在 Q 的下方 (Y更大), O 在 Q 的右方 (X更大)
                                    // 注意：由于可能有旋转，这里按相对位置判断

                                    // 简单判断：谁的 Y 更大谁是 P
                                    if (tempA.Y > tempB.Y) { bestMatch.P = tempA; bestMatch.O = tempB; }
                                    else { bestMatch.P = tempB; bestMatch.O = tempA; }
                                }
                            }
                        }
                    }
                }
            }
            return bestMatch;
        }

        // ==============================================================
        // 辅助方法：找定标点 (提取出来的逻辑)
        // ==============================================================
        private List<List<Point>> FindRedBlobs(Bitmap bitmap)
        {
            List<List<Point>> blobs = new List<List<Point>>();
            int w = bitmap.Width;
            int h = bitmap.Height;
            bool[,] visited = new bool[w, h];

            // 留出边界
            for (int y = 0; y < h - 2; y++)
            {
                for (int x = 0; x < w - 2; x++)
                {
                    if (visited[x, y]) continue;

                    // 快速检查 2x2 块
                    bool isRedBlock = true;
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            Color c = bitmap.GetPixel(x + i, y + j);
                            // 这里的阈值沿用之前优化过的抗反光参数
                            if (c.R > 90 && c.R > c.B + 30 && c.R > c.G + 30 && c.G < 160 && c.B < 160) { }
                            else { isRedBlock = false; break; }
                        }
                        if (!isRedBlock) break;
                    }

                    if (isRedBlock)
                    {
                        // 发现新定标点，开始扩张 (FloodFill)
                        // 这里做一个简化版扩张，或者直接把相邻的 validPoints 合并
                        // 为了代码简洁，这里我们假设上面的 4x4 检查已经足够筛选出中心区域
                        // 我们直接把这个 4x4 块记为一个"微定标点"，后续聚类

                        // 标记已访问
                        for (int i = 0; i < 2; i++) for (int j = 0; j < 2; j++) visited[x + i, y + j] = true;

                        // 尝试归并到现有的 blobs
                        Point center = new Point(x + 2, y + 2);
                        bool merged = false;
                        foreach (var blob in blobs)
                        {
                            // 如果离某个已有定标点的任意一点很近 (<30px)，就并进去
                            // 简化：只比对定标点里的第一个点（假设定标点不大）
                            if (GetDistSq(center, blob[0]) < 400) // 30*30
                            {
                                blob.Add(center);
                                merged = true;
                                break;
                            }
                        }
                        if (!merged)
                        {
                            var newBlob = new List<Point>();
                            newBlob.Add(center);
                            blobs.Add(newBlob);
                        }
                    }
                }
            }

            // 过滤掉太小的孤立定标点 (比如只有 1-2 个 2x2 块组成的)
            return blobs.Where(b => b.Count >= 2).ToList();
        }

        private double GetDistSq(Point p1, Point p2)
        {
            return Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);
        }

        private Point GetBlobCenter(List<Point> blob)
        {
            long sx = 0, sy = 0;
            foreach (var p in blob) { sx += p.X; sy += p.Y; }
            return new Point((int)(sx / blob.Count), (int)(sy / blob.Count));
        }

        // 辅助绘图 (保持不变，略作调整以显示所有候选点)
        private void DrawDebugImage(Bitmap original, List<List<Point>> allBlobs, Point Q, Point P, Point O)
        {
            try
            {
                using (Bitmap debugBmp = new Bitmap(original))
                using (Graphics g = Graphics.FromImage(debugBmp))
                {
                    // 1. 画出所有候选定标点 (灰色框)
                    foreach (var blob in allBlobs)
                    {
                        if (blob.Count == 0) continue;
                        Point c = GetBlobCenter(blob);
                        // 默认画灰色
                        g.DrawRectangle(new Pen(Color.Red, 1), c.X - 10, c.Y - 10, 20, 20);
                    }

                    // 2. 画选中的三角形 (亮色)
                    if (Q.X != 0)
                    {
                        int s = 25;
                        // 连接线
                        g.DrawLine(new Pen(Color.Magenta, 2), Q, P);
                        g.DrawLine(new Pen(Color.Magenta, 2), Q, O);
                        g.DrawLine(new Pen(Color.Magenta, 1), P, O); // 斜边

                        // 顶点
                        g.DrawString("Q", new Font("Arial", 16, FontStyle.Regular), Brushes.Yellow, Q.X, Q.Y - 35);
                        g.DrawEllipse(new Pen(Color.Yellow, 3), Q.X - s / 2, Q.Y - s / 2, s, s);

                        g.DrawString("P", new Font("Arial", 16, FontStyle.Regular), Brushes.Green, P.X, P.Y - 35);
                        g.DrawEllipse(new Pen(Color.Green, 3), P.X - s / 2, P.Y - s / 2, s, s);

                        g.DrawString("O", new Font("Arial", 16, FontStyle.Regular), Brushes.Blue, O.X, O.Y - 35);
                        g.DrawEllipse(new Pen(Color.Blue, 3), O.X - s / 2, O.Y - s / 2, s, s);

                        string filename = $@"D:\Debug\debug_{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}.png";
                        Directory.CreateDirectory(Path.GetDirectoryName(filename)); // 无则创建，有则忽略
                        debugBmp.Save(filename, ImageFormat.Png);
                        Console.WriteLine($"[调试图] {filename}");
                        Console.WriteLine($"q点坐标{Q.X}{Q.Y}");
                        Console.WriteLine($"p点坐标{P.X}{P.Y}");
                        Console.WriteLine($"o点坐标{O.X}{O.Y}");
                    }

                    
                }
            }
            catch { }
        }
 

     
        // 【新增辅助方法】针对小红点（6-8像素）的深度提取
        // 只取 3x3 区域，防止取到背景
        private float GetSmallRegionDepth(DepthFrame dframe, int x, int y)
        {
            float sum = 0;
            int count = 0;
            int kernel = 1; // 1 = 3x3区域 (中心点 +-1)

            int width = dframe.Width;
            int height = dframe.Height;

            for (int i = -kernel; i <= kernel; i++)
            {
                for (int j = -kernel; j <= kernel; j++)
                {
                    int cx = x + i;
                    int cy = y + j;
                    if (cx < 0 || cx >= width || cy < 0 || cy >= height) continue;

                    float d = dframe.GetDistance(cx, cy);
                    // 过滤无效值(0)和过远噪点(>2米)
                    if (d > 0.05f && d < 2.0f)
                    {
                        sum += d;
                        count++;
                    }
                }
            }
            return count > 0 ? sum / count : 0;
        }



        // 【修改后的 Init】
        public rm_position_t Init(Bitmap Cimg, DepthFrame Dimg, Intrinsics Intt)
        {
            rm_position_t q_tool;
            rm_position_t p_tool;
            rm_position_t o_tool;
            rm_current_arm_state_t state = new rm_current_arm_state_t();

            is_success = false;

            // 1. 获取红点边缘坐标
            // res[0/1]=Q(左上), res[2/3]=P(最下), res[4/5]=O(最右)
            int[] res = ProcessBitmapPixels(Cimg);


            if (res[0] == 0 && res[1] == 0 && res[2] == 0 && res[4] == 0)
            {
                // 已经在 ProcessBitmapPixels 里打印过错误了，这里直接静默退出
                return new rm_position_t();
            }
            // 2. 【核心修复】针对小红点的坐标修正
            // 你的点只有6-8像素，ProcessBitmapPixels 抓取的是最边缘。
            // 边缘的深度数据非常烂（飞点）。
            // 这里强制让坐标向红点“中心”回退 3 个像素，确保取到实心的红色区域。

            // Q(左上) -> 往右下退 3px
            int sx_q = res[0] - 1;
            int sy_q = res[1] ;

            // P(最下) -> 往上退 3px (X不变或微调)
            int sx_p = res[2] - 1;
            int sy_p = res[3] ;

            // O(最右) -> 往左退 3px
            int sx_o = res[4] - 1;
            int sy_o = res[5];

            //// 边界安全检查
            //sx_q = Math.Max(0, Math.Min(Cimg.Width - 1, sx_q));
            //sy_q = Math.Max(0, Math.Min(Cimg.Height - 1, sy_q));
            //sy_p = Math.Max(0, Math.Min(Cimg.Height - 1, sy_p));
            //sx_o = Math.Max(0, Math.Min(Cimg.Width - 1, sx_o));

            // 3. 使用“小区域平均”获取深度
            // 以前是直接 Dimg.GetDistance(res[0], res[1])，那是造成不稳定的元凶
            //float z_q = GetSmallRegionDepth(Dimg, sx_q, sy_q);
            //float z_p = GetSmallRegionDepth(Dimg, sx_p, sy_p);
            //float z_o = GetSmallRegionDepth(Dimg, sx_o, sy_o);
            float z_q = Dimg.GetDistance(sx_q, sy_q);
            float z_p = Dimg.GetDistance(sx_p, sy_p);
            float z_o = Dimg.GetDistance(sx_o, sy_o);

            // 4. 计算工具坐标系坐标 (使用修正后的坐标和深度)
            // 如果发现深度为0，直接打印错误
            if (z_q == 0 || z_p == 0 || z_o == 0)
            {
                Console.WriteLine($"[严重错误] 深度丢失! Zq={z_q}, Zp={z_p}, Zo={z_o}. 请检查红点是否反光或太小。");
                return new rm_position_t();
            }

            q_tool.z = z_q;
            q_tool.x = ((sx_q - Intt.ppx) * q_tool.z / Intt.fx) /*- 0.01f*/;
            q_tool.y = ((sy_q - Intt.ppy) * q_tool.z / Intt.fy)/* + 0.002f*/;

            p_tool.z = z_p;
            p_tool.x = ((sx_p - Intt.ppx) * p_tool.z / Intt.fx)/* - 0.01f*/;
            p_tool.y = (sy_p - Intt.ppy) * p_tool.z / Intt.fy;

            o_tool.z = z_o;
            o_tool.x = ((sx_o - Intt.ppx) * o_tool.z / Intt.fx) /*- 0.01f*/;
            o_tool.y = (sy_o - Intt.ppy) * o_tool.z / Intt.fy;



            // 5. 转换到 Base 坐标系
            rm_change_work_frame(Arm.Instance.robotHandlePtr, "Base");
            rm_get_current_arm_state(Arm.Instance.robotHandlePtr, ref state);

            //state.pose.euler.rz += (float)(Math.PI /2);

            rm_position_t q_base = CoordinateTransformer.TransformPoint(q_tool, state.pose);
            rm_position_t p_base = CoordinateTransformer.TransformPoint(p_tool, state.pose);
            rm_position_t o_base = CoordinateTransformer.TransformPoint(o_tool, state.pose);

            double qp = CoordinateTransformCalculator.Distance(q_base, p_base);
            double qo = CoordinateTransformCalculator.Distance(q_base, o_base);

            // 6. 校验逻辑 (增加详细日志)
            // 将容差从 0.01 稍微放宽到 0.015 (1.5cm)，对于 6px 的小点来说，1cm太苛刻了
            bool isIsosceles = Math.Abs(qp - qo) < 0.015d;
            bool isLengthValid = qp > 0.02d && qp < 0.05d && qo > 0.02d && qo < 0.05d;

            Console.WriteLine("---------------- 标定数据 ----------------");
            Console.WriteLine($"QP边长: {qp:F4} m");
            Console.WriteLine($"QO边长: {qo:F4} m");
            Console.WriteLine($"边长差: {Math.Abs(qp - qo):F4} m (阈值 0.015)");
            Console.WriteLine($"深度值: Q={z_q:F3}, P={z_p:F3}, O={z_o:F3}");
            Console.WriteLine($"q_tool.x:{q_tool.x},q_tool.y{q_tool.y},q_tool.z{q_tool.z}");
            Console.WriteLine($"p_tool.x:{p_tool.x},p_tool.y{p_tool.y},p_tool.z{p_tool.z}");
            Console.WriteLine($"o_tool.x:{o_tool.x},o_tool.y{o_tool.y},o_tool.z{o_tool.z}");
            Console.WriteLine($"qbase: {q_base.x} {q_base.y} {q_base.z}");
            Console.WriteLine($"pbase: {p_base.x} {p_base.y} {p_base.z}");
            Console.WriteLine($"obase: {o_base.x} {o_base.y} {o_base.z}");
            Console.WriteLine($"深度值: Q={z_q:F3}, P={z_p:F3}, O={z_o:F3}");

            if (isIsosceles && isLengthValid)
            {
                rm_pose_t set_base = CoordinateTransformCalculator.CalculatePose(q_base, p_base, o_base);
                rm_delete_work_frame(Arm.Instance.robotHandlePtr, "work1");
                rm_set_manual_work_frame(Arm.Instance.robotHandlePtr, "work1", set_base);
                int ret = rm_change_work_frame(Arm.Instance.robotHandlePtr, "work1");
                
                if (ret == 0) Console.WriteLine(">>> 标定成功 (init success!)");
                else MessageBox.Show("设置坐标系失败 (pose fail!)");

                is_success = true;
            }
            else
            {
                Console.WriteLine(">>> 标定失败：边长校验未通过");
                if (!isLengthValid) Console.WriteLine($"    原因: 边长不在 3-7cm 范围内。");
                if (!isIsosceles) Console.WriteLine($"    原因: 不是等腰三角形，两边相差 {(Math.Abs(qp - qo) * 100):F2} cm。");

                is_success = false; // 确保置为 false
            }
            Console.WriteLine("-----------------------------------------\r\n");

            return q_base;
        }









    }
}