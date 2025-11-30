using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GDI.Services.Arm;

namespace GDI.Core
{
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


    // ============================================================
    // ============================================================
    // ============================================================
    // ============================================================
    // ============================================================
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


    // ============================================================
    // ============================================================
    // ============================================================
    // ============================================================
    // ============================================================
    /// 坐标变换核心类>
    //定平面所用到的一系列方法/类


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












    // ============================================================
    // ============================================================
    
}
