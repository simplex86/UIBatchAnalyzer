using System;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleX
{
    [DisallowMultipleComponent]
    public class UIMesh : BaseMeshEffect
    {
#if UNITY_EDITOR
        public object what { get; set; } = null;
        public Action<Mesh, object> OnMeshChanged;

        public override void ModifyMesh(VertexHelper vh)
        {
            var mesh = new Mesh();
            vh.FillMesh(mesh);

            OnMeshChanged?.Invoke(mesh, what);
        }

        private void OnDestroy()
        {
            OnMeshChanged = null;
        }
#endif
    }
}
