using System;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    public class KWidget
    {
        public int hierarchyIndex { get; private set; } = 0;
        public KWidget bottom { get; private set; } = null;
        public int depth { get; private set; } = 0;
        public GameObject gameObject { get { return graphic.gameObject; }}
        public string name { get { return graphic.gameObject.name; }}
        public Material material { get { return graphic.material; }}
        public Texture texture { get { return graphic.mainTexture; }}
        public int vertexCount { get { return (mesh == null) ? 0 : mesh.vertexCount; }}

        private MaskableGraphic graphic = null;
        private KMesh mesh = null;

        public KWidget(MaskableGraphic graphic, KMesh mesh, int hierarchyIndex)
        {
            this.graphic = graphic;
            this.mesh = mesh;
            this.hierarchyIndex = hierarchyIndex;
        }

        // 判断mesh是否有覆盖
        public bool MeshOverlap(KWidget widget)
        {
            return mesh.Overlap(widget.mesh);
        }

        // 设置底层UI
        public void SetBottomWidget(KWidget widget)
        {
            bottom = widget;
            depth = CheckBatch(widget) ? widget.depth : widget.depth + 1;
        }

        // 检查是否可以和另一个UI节点合批
        public bool CheckBatch(KWidget widget)
        {
            var mat1 = material;
            var mat2 = widget.material;
            // 材质不同，不能合批
            if (mat1.GetInstanceID() != mat2.GetInstanceID()) return false;

            var tex1 = texture;
            var tex2 = widget.texture;
            // 都没有纹理，可以合批
            if (tex1 == null && tex2 == null) return true;
            // 纹理不同，不能合批
            if ((tex1 == null && tex2 != null) || (tex1 != null && tex2 == null)) return false;
            // 纹理不同，不能合批
            if (tex1.GetInstanceID() != tex2.GetInstanceID()) return false;

            return true;
        }
    }
}