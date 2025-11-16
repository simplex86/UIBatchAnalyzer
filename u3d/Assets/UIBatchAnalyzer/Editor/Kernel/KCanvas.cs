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

        /// <summary>
        /// 添加渲染批次
        /// </summary>
        /// <param name="batch"></param>
        public void AddBatch(KBatch batch)
        {
            batches.Add(batch);
            Sort();

            instructionCount += batch.instructionCount;
            vertexCount += batch.vertexCount;
        }

        /// <summary>
        /// 对渲染批次进行排序
        /// </summary>
        private void Sort()
        {
            batches.Sort((a, b) =>
            {
                if (a.depth < b.depth) return -1;
                if (a.depth > b.depth) return 1;

                return (a.minRenderOrder < b.minRenderOrder) ? -1 : 1;
            });
        }
    }
}