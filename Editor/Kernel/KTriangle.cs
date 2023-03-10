using System;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleX
{
    public class KTriangle
    {
        private Vector3[] vertices = new Vector3[3];

        public KTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        // 是否重叠
        // TODO 算法很朴素，效率比较低
        public bool Overlap(KTriangle other)
        {
            // 边是否相交
            for (int i=0; i<3; i++)
            {
                var a1 = this[i + 0];
                var a2 = this[i + 1];

                for (int j=0; j<3; j++)
                {
                    var b1 = other[j + 0];
                    var b2 = other[j + 1];
                    if (IsIntersectant2(a1, a2, b1, b2)) return true;
                }
            }
            // 点是否包含：
            // 1、先判断三角形A的点是否在三角形B内
            // 2、再判断三角形B的点是否在三角形A内
            for (int i=0; i<3; i++)
            {
                if (this.IsContain2(other[i])) return true;
            }
            for (int i=0; i<3; i++)
            {
                if (other.IsContain2(this[i])) return true;
            }

            return false;
        }

        // 是否在同一平面内
        public bool IsSamePlane(KTriangle other)
        {
            // TODO
            return true;
        }

        public Vector3 this[int index] => vertices[index % 3];

        // 线段(a1,a2)和(b1, b2)是否相交
        // TODO 有bug，暂未解决
        private bool IsIntersectant(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            var crossA = Mathf.Sign(Vector3.Cross(b2 - b1, a1 - b1).z);
            var crossB = Mathf.Sign(Vector3.Cross(b2 - b1, a2 - b1).z);

            if (Mathf.Approximately(crossA, crossB)) return false;

            var crossC = Mathf.Sign(Vector3.Cross(a2 - a1, b1 - a1).z);
            var crossD = Mathf.Sign(Vector3.Cross(a2 - a1, b2 - a1).z);

            if (Mathf.Approximately(crossC, crossD)) return false;

            return true;
        }

        // 线段(a1,a2)和(b1, b2)是否相交
        private bool IsIntersectant2(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            if (Mathf.Abs((a2.y - a1.y) * (b1.x - b2.x) - (a2.x - a1.x) * (b1.y - b2.y)) < float.Epsilon)
            {
                return false; // 线段平行，无交点
            }

            var x = ((a2.x - a1.x) * (b1.x - b2.x) * (b1.y - a1.y) - b1.x * (a2.x - a1.x) * (b1.y - b2.y) + a1.x * (a2.y - a1.y) * (b1.x - b2.x)) / ((a2.y - a1.y) * (b1.x - b2.x) - (a2.x - a1.x) * (b1.y - b2.y));
            var y = ((a2.y - a1.y) * (b1.y - b2.y) * (b1.x - a1.x) - b1.y * (a2.y - a1.y) * (b1.x - b2.x) + a1.y * (a2.x - a1.x) * (b1.y - b2.y)) / ((a2.x - a1.x) * (b1.y - b2.y) - (a2.y - a1.y) * (b1.x - b2.x));

            if ((x - a1.x) * (x - a2.x) <= 0 && (x - b1.x) * (x - b2.x) <= 0 &&
                (y - a1.y) * (y - a2.y) <= 0 && (y - b1.y) * (y - b2.y) <= 0)
            {
                return true; // 相交
            }
            
            return false; // 直线相交但交点不在线段上
        }

        // 点(p)是否在三角形内
        private bool IsContain(Vector3 p)
        {
            var ab = vertices[1] - vertices[0];
            var bc = vertices[2] - vertices[1];
            var ca = vertices[0] - vertices[2];
            
            var ap = p - vertices[0];
            var bp = p - vertices[1];
            var cp = p - vertices[2];

            var c1 = Mathf.Sign(Vector3.Cross(ab, ap).z);
            var c2 = Mathf.Sign(Vector3.Cross(bc, bp).z);
            var c3 = Mathf.Sign(Vector3.Cross(ca, cp).z);

            if (Mathf.Approximately(c1, c2) && Mathf.Approximately(c2, c3) && Mathf.Approximately(c1, c3))
            {
                return true;
            }

            return false;
        }
        
        private const float DEVIATION = 0.05f; // 误差

        // 点(p)是否在三角形内
        // 据说相比IsContains函数的精度高一些
        private bool IsContain2(Vector3 p)
        {
            var d1 = vertices[1] - vertices[0];
            var d2 = vertices[2] - vertices[1];
            var d3 = vertices[0] - vertices[2];

            var c1 = Mathf.Sign(Vector3.Cross(d1, p - vertices[0]).z);
            var c2 = Mathf.Sign(Vector3.Cross(d2, p - vertices[1]).z);
            var c3 = Mathf.Sign(Vector3.Cross(d3, p - vertices[2]).z);

            var d4 = Mathf.Approximately(c1, c2) ? c3 : Mathf.Approximately(c1, c3) ? c2 : c1;

            c1 = Mathf.Sign(Vector3.Cross(d1, p - vertices[0]).z - d4 * DEVIATION);
            c2 = Mathf.Sign(Vector3.Cross(d2, p - vertices[1]).z - d4 * DEVIATION);
            c3 = Mathf.Sign(Vector3.Cross(d3, p - vertices[2]).z - d4 * DEVIATION);

            return Mathf.Approximately(c1, c2) && Mathf.Approximately(c2, c3);
        }
    }
}