using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace SimpleX
{
    public class KWidget
    {
        public int hierarchyIndex { get; private set; } = 0;
        public KWidget bottom { get; private set; } = null;
        public int depth { get; private set; } = 0;
        public GameObject gameObject { get { return graphic.gameObject; }}
        public string name { get { return graphic.gameObject.name; }}
        public Material material { get { return renderAsset.material; }}
        public Texture texture { get { return renderAsset.texture; }}
        public SpriteAtlas spriteAtlas { get { return renderAsset.spriteAtlas; }}
        public int vertexCount { get { return (mesh == null) ? 0 : mesh.vertexCount; }}

        private MaskableGraphic graphic = null;
        private KRenderAsset renderAsset = null;
        private KMesh mesh = null;

        public KWidget(MaskableGraphic graphic, KMesh mesh, int hierarchyIndex)
        {
            this.graphic = graphic;
            this.renderAsset = new KRenderAsset(graphic);
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
            return renderAsset.Equals(widget.renderAsset);
        }
    }
}