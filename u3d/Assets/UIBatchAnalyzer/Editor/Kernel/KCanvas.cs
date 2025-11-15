using System.Collections.Generic;
using UnityEngine;

namespace SimpleX
{
    public class KCanvas
    {
        public Canvas canvas { get; } = null;
        public List<KBatch> batches { get; } = new List<KBatch>();
        public int instructionCount { get; private set; } = 0;
        public int vertexCount { get; private set; } = 0;
        public int batchCount => batches.Count;

        public KCanvas(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void AddBatch(KBatch batch)
        {
            batches.Add(batch);

            instructionCount += batch.instructionCount;
            vertexCount += batch.vertexCount;
        }
    }
}