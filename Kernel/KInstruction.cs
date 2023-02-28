using System.Reflection;
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
        public bool isMask { get; } = false;
        public bool isUnmask { get; } = false;
        public Texture texture => (graphic == null) ? null : graphic.mainTexture;
        public int vertexCount => (mesh == null) ? 0 : mesh.vertexCount;
        
        private MaskableGraphic graphic = null;

        public KInstruction(MaskableGraphic graphic, KMesh mesh, int renderOrder, Mask mask = null, bool isUnmask = false)
        {
            this.graphic = graphic;
            this.material = graphic.materialForRendering;
            this.mesh = mesh;
            this.renderOrder = renderOrder;
            this.isMask = false;
            this.isUnmask = false;

            if (mask != null)
            {
                this.isMask = true;
                this.isUnmask = isUnmask;
                
                if (isUnmask)
                {
                    this.isMask = false;
                    material = GetUnmaskMaterial(mask);
                }
            }
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
    }
}