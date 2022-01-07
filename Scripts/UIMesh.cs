using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    // [HideInInspector]
    [DisallowMultipleComponent]
    public class UIMesh : BaseMeshEffect
    {
        public KMesh mesh { get; private set; } = null;
        public Action<UIMesh> OnMeshChanged;

        private void Awake()
        {
            mesh = new KMesh(transform);
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            var temp = new Mesh();
            vh.FillMesh(temp);

            mesh.Fill(temp);
            OnMeshChanged(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var t in mesh.triangles)
            {
                for(int i=0; i<3; i++)
                {
                    Gizmos.DrawLine(t[i + 0], t[i + 1]);
                }
            }
        }
    }
}
