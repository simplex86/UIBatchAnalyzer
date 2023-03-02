using System;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleX
{
    [DisallowMultipleComponent]
    public class UIMesh : BaseMeshEffect
    {
#if UNITY_EDITOR
        public object userData { get; set; } = null;
        public Action<Mesh, object> OnMeshChanged;

        private void Awake()
        {
            hideFlags = HideFlags.HideInInspector;
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            var mesh = new Mesh();
            vh.FillMesh(mesh);

            OnMeshChanged?.Invoke(mesh, userData);
        }

        private void OnDestroy()
        {
            OnMeshChanged = null;
        }
#endif
    }
}
