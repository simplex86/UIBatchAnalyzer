using System.Collections.Generic;
using UnityEngine;

namespace SimpleX
{
    public class VBatch
    {
        public KBatch batch { get; private set; } = null;
        public bool expand { get; set; } = false;

        public VBatch(KBatch batch)
        {
            this.batch = batch;
        }
    }
    
    public class VCanvas
    {
        public Canvas canvas { get; private set; } = null;
        public List<VBatch> batches { get; } = new List<VBatch>();
        public int gameObjectCount { get; private set; } = 0;
        public int vertexCount { get; private set; } = 0;
        public int batchCount { get { return batches.Count; }}
        public bool expand { get; set; } = true;

        public VCanvas(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void AddBatch(KBatch batch)
        {
            var vbatch = new VBatch(batch);
            batches.Add(vbatch);

            gameObjectCount += batch.widgetCount;
            vertexCount += batch.vertexCount;
        }
    }
    
    public class UIBatchAnalyzerData
    {
        public List<VCanvas> groups { get; private set; }

        public void OnEnable()
        {
            groups = new List<VCanvas>();
        }

        public void OnDisable()
        {
            groups.Clear();
        }
    }
}