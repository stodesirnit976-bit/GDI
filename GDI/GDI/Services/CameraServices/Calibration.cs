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












    }
}
