using UnityEngine;
using UnityEngine.U2D;

namespace SimpleX
{
    /// <summary>
    /// 可渲染对象的接口
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// 深度
        /// </summary>
        int depth { get; }
        /// <summary>
        /// 材质
        /// </summary>
        Material material { get; }
        /// <summary>
        /// 纹理
        /// </summary>
        Texture texture { get; }
        /// <summary>
        /// 图集
        /// </summary>
        SpriteAtlas spriteAtlas { get; }
        /// <summary>
        /// 蒙版类型
        /// </summary>
        EMaskType maskType { get; }
        /// <summary>
        /// 顶点数
        /// </summary>
        int vertexCount { get; }
    }
}