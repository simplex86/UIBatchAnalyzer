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
        public KMesh mesh { get; }= null;
        public Material material { get; } = null;
        public bool isMask { get; } = false;
        public bool isUnmask { get; } = false;
        public Texture texture => (graphic == null) ? null : graphic.mainTexture;
        public SpriteAtlas spriteAtlas { get; }
        public int vertexCount => (mesh == null) ? 0 : mesh.vertexCount;
        
        private MaskableGraphic graphic = null;

        public KInstruction(MaskableGraphic graphic, Material material, KMesh mesh, int renderOrder, bool isMask = false, bool isUnmask = false)
        {
            this.graphic = graphic;
            this.material = material;
            this.mesh = mesh;
            this.renderOrder = renderOrder;
            this.isMask = isMask;
            this.isUnmask = isUnmask;
            
            var image = graphic.gameObject.GetComponent<Image>();
            if (image != null && image.sprite != null)
            {
                spriteAtlas = KSpriteAtlas.GetSpriteAtlas(image.sprite);
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
            // 材质
            if (material.GetInstanceID() != instruction.material.GetInstanceID())
            {
                return false;
            }
            // 图集
            if ((spriteAtlas != null && instruction.spriteAtlas == null) || (spriteAtlas == null && instruction.spriteAtlas != null))
            {
                return false;
            }
            if (spriteAtlas != null && instruction.spriteAtlas != null)
            {
                return (spriteAtlas.GetInstanceID() == instruction.spriteAtlas.GetInstanceID());
            }
            // 纹理
            return (texture.GetInstanceID() == instruction.texture.GetInstanceID());
        }
    }
}