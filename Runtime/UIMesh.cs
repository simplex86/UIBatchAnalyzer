using System;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleX
{
    [DisallowMultipleComponent]
    public class UIMesh : BaseMeshEffect
    {
#if UNITY_EDITOR
        public Canvas canvas { get; set; } = null;
        public int renderOrder { get; set; } = 0;
        public Action<Transform, Canvas, int, Mesh> OnMeshChanged;

        public override void ModifyMesh(VertexHelper vh)
        {
            var mesh = new Mesh();
            vh.FillMesh(mesh);

            OnMeshChanged?.Invoke(transform, canvas, renderOrder, mesh);
        }

        private void OnDestroy()
        {
            OnMeshChanged = null;
        }
#endif
    }
}
