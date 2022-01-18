using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace XH
{
    public class KBatch
    {
        // Canvas
        public Canvas canvas { get; private set; } = null;
        // 子节点
        public List<KWidget> widgets { get; } = new List<KWidget>();
        // 材质
        public Material material 
        {
            get { return (widgets.Count > 0) ? widgets[0].material : null; }
        }
        // 图集
        public SpriteAtlas spriteAtlas 
        {
            get { return (widgets.Count > 0) ? widgets[0].spriteAtlas : null; }
        }
        // 纹理
        public Texture texture 
        {
            get { return (widgets.Count > 0) ? widgets[0].texture : null; }
        }
        // 子控件数量
        public int widgetCount { get { return widgets.Count; }}
        // 顶点数量
        public int vertexCount { get; private set; } = 0;

        public KBatch(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void Add(KWidget widget)
        {
            widgets.Add(widget);
            vertexCount += widget.vertexCount;
        }

        public bool Check(KWidget widget)
        {
            if (widgets.Count == 0) return true;
            return widgets[0].CheckBatch(widget);
        }
    }
}