using System;
using System.Collections.Generic;
using UnityEngine;

namespace XH
{
    public class KBatch
    {
        public int depth { get; private set; } = 0;
        public List<KWidget> widgets { get; } = new List<KWidget>();

        public KBatch(int depth)
        {
            this.depth = depth;
        }

        public void Add(KWidget widget)
        {
            widgets.Add(widget);
        }

        public bool Check(KWidget widget)
        {
            if (widgets.Count == 0) return true;
            return widgets[0].CheckBatch(widget);
        }
    }
}