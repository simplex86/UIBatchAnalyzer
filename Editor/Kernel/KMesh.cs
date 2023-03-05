using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleX
{
    public class KMesh
    {
        public List<KTriangle> triangles { get; } = new List<KTriangle>();
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