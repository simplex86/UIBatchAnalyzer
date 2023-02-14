using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleX
{
    // [HideInInspector]
    [DisallowMultipleComponent]
    public class UIMesh : BaseMeshEffect
    {
        public KMesh mesh { get; private set; } = null;
        public Action OnMeshChanged;

        private void Awake()
        {
            mesh = new KMesh(transform);
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (mesh != null)
            {
                var temp = new Mesh();
                vh.FillMesh(temp);

                mesh.Fill(temp);
                if (OnMeshChanged != null)
                {
                    OnMeshChanged();
                }
            }
        }

        private void OnDestroy()
        {
            mesh = null;
            OnMeshChanged = null;
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
