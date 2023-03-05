using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace SimpleX
{
    public class KInstruction
    {
        public int renderOrder { get; } = 0;
        public KInstruction bottom { get; private set; } = null;
        public int depth { get; private set; } = 0;
        public GameObject gameObject => (graphic == null) ? null : graphic.gameObject;
        public string name => (graphic == null) ? string.Empty : graphic.name;
        public KMesh mesh { get; } = null;
        public Material material { get; } = null;
        public Texture texture => (materialTexture == null) ? graphicTexture : materialTexture;
        public bool isMask { get; } = false;
        public bool isUnmask { get; } = false;
        public RectMask2D rectmask2d { get; } = null;
        public int vertexCount => (mesh == null) ? 0 : mesh.vertexCount;
        
        private MaskableGraphic graphic = null;
        private Texture materialTexture => (material == null) ? null : material.mainTexture;
        private Texture graphicTexture => (graphic == null) ? null : graphic.mainTexture;
        
        public KInstruction(MaskableGraphic graphic, KMesh mesh, int renderOrder)
        {
            this.graphic = graphic;
            this.material = graphic.materialForRendering;
            this.mesh = mesh;
            this.renderOrder = renderOrder;
            this.isMask = false;
            this.isUnmask = false;
        }

        public KInstruction(MaskableGraphic graphic, KMesh mesh, int renderOrder, Mask mask, bool isUnmask = false)
            : this(graphic, mesh, renderOrder)
        {
            this.isMask = true;
            this.isUnmask = isUnmask;
            
            if (isUnmask)
            {
                this.isMask = false;
                material = GetUnmaskMaterial(mask);
            }
        }
        
        public KInstruction(MaskableGraphic graphic, KMesh mesh, int renderOrder, RectMask2D rectmask2d)
            : this(graphic, mesh, renderOrder)
        {
            this.rectmask2d = rectmask2d;
        }

        // 判断mesh是否有覆盖
        public bool MeshOverlap(KInstruction instruction)
        {
            return mesh.Overlap(instruction.mesh);
        }

        // 设置底层UI
        public void SetBottom(KInstruction instruction)
        {
            bottom = instruction;
            depth = CheckBatch(instruction) ? instruction.depth : instruction.depth + 1;
        }

        // 检查是否可以和另一个UI节点合批
        public bool CheckBatch(KInstruction instruction)
        {
            // rectmask2d不同不能合
            if (!CompareRectMask2D(rectmask2d, instruction.rectmask2d))
            {
                return false;
            }
            // 不同材质不能合
            if (material.GetInstanceID() != instruction.material.GetInstanceID())
            {
                return false;
            }
            // 不同纹理不能合
            if (texture.GetInstanceID() != instruction.texture.GetInstanceID())
            {
                return false;
            }
            // 不同平面不能合
            // TODO

            return true;
        }
        
        // 获取UnMask阶段的Material
        private Material GetUnmaskMaterial(Mask mask)
        {
            var unmaskMaterial = mask.GetType().GetField("m_UnmaskMaterial", BindingFlags.NonPublic | BindingFlags.Instance);
            return unmaskMaterial.GetValue(mask) as Material;
        }

        private bool CompareRectMask2D(RectMask2D a, RectMask2D b)
        {
            if ((a != null && b == null) || (a == null && b != null))
            {
                return false;
            }
            if (a != null && b != null)
            {
                if (a.GetInstanceID() == b.GetInstanceID())
                {
                    return true;
                }
                
                var p1 = a.transform.position;
                var p2 = b.transform.position;
                if (!Mathf.Approximately(p1.x, p2.x) ||
                    !Mathf.Approximately(p1.y, p2.y) ||
                    !Mathf.Approximately(p1.z, p2.z))
                {
                    return false;
                }
                var r1 = a.transform.rotation;
                var r2 = b.transform.rotation;
                if (!Mathf.Approximately(r1.x, r2.x) ||
                    !Mathf.Approximately(r1.y, r2.y) ||
                    !Mathf.Approximately(r1.z, r2.z) ||
                    !Mathf.Approximately(r1.w, r2.w))
                {
                    return false;
                }
                var s1 = a.transform.localScale;
                var s2 = b.transform.localScale;
                if (!Mathf.Approximately(s1.x, s2.x) ||
                    !Mathf.Approximately(s1.y, s2.y) ||
                    !Mathf.Approximately(s1.z, s2.z))
                {
                    return false;
                }

                var d1 = a.rectTransform.rect;
                var d2 = b.rectTransform.rect;
                if (!Mathf.Approximately(d1.width,  d2.width) ||
                    !Mathf.Approximately(d1.height, d2.height))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}