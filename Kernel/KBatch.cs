using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace SimpleX
{
    public class KBatch
    {
        // Canvas
        public Canvas canvas { get; private set; } = null;
        // 子节点
        public List<KInstruction> instructions { get; } = new List<KInstruction>();
        // 材质
        public Material material 
        {
            get { return (instructions.Count > 0) ? instructions[0].material : null; }
        }
        // 图集
        public SpriteAtlas spriteAtlas 
        {
            get { return (instructions.Count > 0) ? instructions[0].spriteAtlas : null; }
        }
        // 纹理
        public Texture texture 
        {
            get { return (instructions.Count > 0) ? instructions[0].texture : null; }
        }
        // 子控件数量
        public int instructionCount { get { return instructions.Count; }}
        // 顶点数量
        public int vertexCount { get; private set; } = 0;

        public KBatch(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void Add(KInstruction instruction)
        {
            instructions.Add(instruction);
            vertexCount += instruction.vertexCount;
        }

        public bool Check(KInstruction instruction)
        {
            if (instructions.Count == 0) return true;
            return instructions[0].CheckBatch(instruction);
        }
    }
}