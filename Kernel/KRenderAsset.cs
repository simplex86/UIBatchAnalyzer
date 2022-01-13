using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class KRenderAsset
{
    public Material material { get; private set; } = null;
    public Texture texture { get; private set; } = null;
    public SpriteAtlas spriteAtlas { get; private set; } = null;

    public KRenderAsset(MaskableGraphic graphic)
    {
        material = graphic.material;
        texture = graphic.mainTexture;

        Image image = graphic.gameObject.GetComponent<Image>();
        if (image != null && image.sprite != null)
        {
            spriteAtlas = KSpriteAtlas.GetSpriteAtlas(image.sprite);
        }
    }

    public bool Equals(KRenderAsset other)
    {
        // 材质
        if (material.GetInstanceID() != other.material.GetInstanceID())
        {
            return false;
        }
        // 图集
        if ((spriteAtlas != null && other.spriteAtlas == null) || (spriteAtlas == null && other.spriteAtlas != null))
        {
            return false;
        }
        if (spriteAtlas != null && other.spriteAtlas != null)
        {
            return (spriteAtlas.GetInstanceID() == other.spriteAtlas.GetInstanceID());
        }
        // 纹理
        return (texture.GetInstanceID() == other.texture.GetInstanceID());
    }
}