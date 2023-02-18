using System.Collections.Generic;
using UnityEngine;

namespace SimpleX
{
    public class kCanvas
    {
        public Canvas canvas { get; private set; } = null;
        public List<KBatch> batches { get; } = new List<KBatch>();
        public int widgetCount { get; private set; } = 0;
        public int vertexCount { get; private set; } = 0;
        public int batchCount { get { return batches.Count; }}

        public kCanvas(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void AddBatch(KBatch batch)
        {
            batches.Add(batch);

            widgetCount += batch.widgetCount;
            vertexCount += batch.vertexCount;
        }
    }
}