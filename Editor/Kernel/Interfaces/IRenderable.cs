using UnityEngine;
using UnityEngine.U2D;

namespace SimpleX
{
    public interface IRenderable
    {
        int depth { get; }
        Material material { get; }
        Texture texture { get; }
        SpriteAtlas spriteAtlas { get; }
        EMaskType maskType { get; }
    }
}