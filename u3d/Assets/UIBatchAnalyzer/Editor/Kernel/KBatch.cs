using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace SimpleX
{
    /// <summary>
    /// 渲染批次
    /// </summary>
    public class KBatch : IRenderable
    {
        /// <summary>
        /// Canvas
        /// </summary>
        public Canvas canvas { get; } = null;
        /// <summary>
        /// 子节点
        /// </summary>
        public List<KInstruction> instructions { get; } = new List<KInstruction>();
        /// <summary>
        /// 材质
        /// </summary>
        public Material material => (instructions.Count > 0) ? instructions[0].material : null;
        /// <summary>
        /// 图集
        /// </summary>
        public SpriteAtlas spriteAtlas => (instructions.Count > 0) ? instructions[0].spriteAtlas : null;
        /// <summary>
        /// 纹理
        /// </summary>
        public Texture texture => (instructions.Count > 0) ? instructions[0].texture : null;
        /// <summary>
        /// 深度
        /// </summary>
        public int depth => (instructions.Count > 0) ? instructions[0].depth : -1;
        /// <summary>
        /// Mask类型
        /// </summary>
        public EMaskType maskType => (instructions.Count > 0) ? instructions[0].maskType : EMaskType.None;
        /// <summary>
        /// 子控件数量
        /// </summary>
        public int instructionCount => instructions.Count;
        /// <summary>
        /// 顶点数量
        /// </summary>
        public int vertexCount { get; private set; } = 0;
        /// <summary>
        /// 最小渲染序号
        /// </summary>
        public int minRenderOrder { get; private set; } = int.MaxValue;

        public KBatch(Canvas canvas)
        {
            this.canvas = canvas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instruction"></param>
        public void Add(KInstruction instruction)
        {
            instructions.Add(instruction);
            minRenderOrder = Mathf.Min(instruction.renderOrder, minRenderOrder);
            vertexCount += instruction.vertexCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        public bool Check(KInstruction instruction)
        {
            if (instructions.Count == 0) return true;
            return instructions[0].CheckBatch(instruction);
        }
    }
}