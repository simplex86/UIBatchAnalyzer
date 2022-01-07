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
        public Texture texture { get { return GetTexture(); }}

        private Graphic graphic = null;
        private KMesh mesh = null;

        public KWidget(Graphic graphic, KMesh mesh, int hierarchyIndex)
        {
            this.graphic = graphic;
            this.mesh = mesh;
            this.hierarchyIndex = hierarchyIndex;
        }

        public bool MeshOverlap(KWidget widget)
        {
            return mesh.Overlap(widget.mesh);
        }

        public void SetBottomWidget(KWidget widget)
        {
            bottom = widget;
            depth = CheckBatch(widget) ? widget.depth : widget.depth + 1;
        }

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

        private Texture GetTexture()
        {
            var go = graphic.gameObject;

            var rawImage = go.GetComponent<RawImage>();
            if (rawImage != null)
            {
                return rawImage.texture;
            }

            var image = go.GetComponent<Image>();
            if (image != null)
            {
                var sprite = image.sprite;
                return (sprite != null) ? sprite.texture : null;
            }

            return null;
        }
    }
}