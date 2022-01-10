using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    public class KMesh
    {
        public Transform transform { get; private set; } = null;
        public List<KTriangle> triangles { get; private set; } = new List<KTriangle>();
        public int vertexCount { get; private set; } = 0;

        public KMesh(Transform transform)
        {
            this.transform = transform;
        }

        public void Fill(Mesh mesh)
        {
            triangles.Clear();

            var pos = transform.position;
            var rot = transform.rotation;
            var sca = transform.lossyScale;

            for (int i=0; i<mesh.triangles.Length; i+=3)
            {
                var k1 = mesh.triangles[i + 0];
                var k2 = mesh.triangles[i + 1];
                var k3 = mesh.triangles[i + 2];

                var v1 = pos + rot * Vector3.Scale(mesh.vertices[k1], sca);
                var v2 = pos + rot * Vector3.Scale(mesh.vertices[k2], sca);
                var v3 = pos + rot * Vector3.Scale(mesh.vertices[k3], sca);

                triangles.Add(new KTriangle(v1, v2, v3));
            }

            vertexCount = mesh.vertexCount;
        }

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