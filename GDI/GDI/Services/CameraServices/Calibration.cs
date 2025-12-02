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
        private bool is_success = false;

        public void Calibration_subCamEvent(Camera cam)
        {
            if (isCalibrating) return;
            isCalibrating = true;

            this.cam = cam;
            startTime = DateTime.Now;

            // 订阅相机帧事件
            cam.cam_Event += imgProcessor;
        }

        //rm_position_t q;
        private void imgProcessor(Bitmap ColorBitmap, Bitmap DepthColorBitmap, DepthFrame depthFrame, Intrinsics intrinsics)
        {
            if ((DateTime.Now - startTime).TotalSeconds < 7) return;

            Console.WriteLine("相机启动超过7秒，第一次标定");
            // 处理帧数据进行标定

            rm_position_t q = Init(ColorBitmap, depthFrame, intrinsics);//wm修改
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
            startTime = DateTime.Now;

            this.cam = cam;

            // 订阅相机帧事件
            cam.cam_Event += imgProcessor2;
        }
        //wm修改
        private void imgProcessor2(Bitmap ColorBitmap, Bitmap DepthColorBitmap, DepthFrame depthFrame, Intrinsics intrinsics)
        {
            if ((DateTime.Now - startTime).TotalSeconds < 2) return;
            startTime = DateTime.Now;
            // 处理帧数据进行标定
            Console.WriteLine("正在第二次标定");
            Init(ColorBitmap, depthFrame, intrinsics);


            // 标定完成，取消订阅相机帧事件
            if (is_success)
            {
                rm_change_work_frame(Arm.Instance.robotHandlePtr, "Base");
                rm_movej(Arm.Instance.robotHandlePtr, c_ini, 15, 0, 0, 0);
                rm_change_work_frame(Arm.Instance.robotHandlePtr, "work1");
                cam.cam_Event -= imgProcessor2;
                Console.WriteLine("二次标定完成！");
            }

            isCalibrating2 = false;
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
                    if ((pixelColor.R > pixelColor.B * 3 / 2) && (pixelColor.R > pixelColor.G * 3 / 2) && pixelColor.R > 100)
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
                    if ((pixelColor.R > pixelColor.B * 3 / 2) && (pixelColor.R > pixelColor.G * 3 / 2) && pixelColor.R > 100)
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
        public rm_position_t Init(Bitmap Cimg, DepthFrame Dimg, Intrinsics Intt)//wm修改
        {
            rm_position_t q_tool;
            rm_position_t p_tool;
            rm_position_t o_tool;
            rm_current_arm_state_t state = new rm_current_arm_state_t();
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

            if (Math.Abs(CoordinateTransformCalculator.Distance(q_base, p_base) - CoordinateTransformCalculator.Distance(q_base, o_base)) < 0.015
                && CoordinateTransformCalculator.Distance(q_base, p_base) > 0.05
                && CoordinateTransformCalculator.Distance(q_base, p_base) < 0.07
                && CoordinateTransformCalculator.Distance(q_base, o_base) > 0.05
                && CoordinateTransformCalculator.Distance(q_base, o_base) < 0.07)
            {
                rm_pose_t set_base = CoordinateTransformCalculator.CalculatePose(q_base, p_base, o_base);
                rm_delete_work_frame(Arm.Instance.robotHandlePtr, "work1");
                rm_set_manual_work_frame(Arm.Instance.robotHandlePtr, "work1", set_base);
                int ret = rm_change_work_frame(Arm.Instance.robotHandlePtr, "work1");
                if (ret == 0) Console.WriteLine("ini success!");
                else MessageBox.Show("pose fail!");
                is_success = true;
            }

            //Console.WriteLine(CoordinateTransformCalculator.Distance(q_base, p_base) + ";" + CoordinateTransformCalculator.Distance(q_base, o_base));

            return q_base;
        }












    }
}