using System.Collections.Generic;
using UnityEngine;

namespace SimpleX
{
    /// <summary>
    /// 网格
    /// </summary>
    public class KMesh
    {
        /// <summary>
        /// 三角形列表
        /// </summary>
        public List<KTriangle> triangles { get; } = new List<KTriangle>();
        /// <summary>
        /// 顶点数
        /// </summary>
        public int vertexCount { get; private set; } = 0;

        private Vector3 position = Vector3.zero;
        private Quaternion rotation = Quaternion.identity;
        private Vector3 scale = Vector3.one;

        public KMesh(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
            scale = transform.lossyScale;
        }

        /// <summary>
        /// 填充
        /// </summary>
        /// <param name="mesh"></param>
        public void Fill(Mesh mesh)
        {
            triangles.Clear();

            for (int i=0; i<mesh.triangles.Length; i+=3)
            {
                var k1 = mesh.triangles[i + 0];
                var k2 = mesh.triangles[i + 1];
                var k3 = mesh.triangles[i + 2];

                var v1 = position + rotation * Vector3.Scale(mesh.vertices[k1], scale);
                var v2 = position + rotation * Vector3.Scale(mesh.vertices[k2], scale);
                var v3 = position + rotation * Vector3.Scale(mesh.vertices[k3], scale);

                triangles.Add(new KTriangle(v1, v2, v3));
            }

            vertexCount = mesh.vertexCount;
        }

        /// <summary>
        /// 判断两个网格是否有重叠
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Overlap(KMesh other)
        {
            foreach (var t1 in triangles)
            {
                foreach (var t2 in other.triangles)
                {
                    if (t1.Overlap(t2)) return true;
                }
            }

            return false;
        }
    }
}