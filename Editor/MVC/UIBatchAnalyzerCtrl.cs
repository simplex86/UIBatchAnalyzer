using System;
using UnityEngine;

namespace SimpleX
{
    public class UIBatchAnalyzerCtrl
    {
        private UIBatchAnalyzerData data;
        
        private KAnalyzer analyzer = null;
        private Action callback = null;

        public UIBatchAnalyzerCtrl(UIBatchAnalyzerData data)
        {
            this.data = data;
        }
        
        public void OnEnable()
        {
            analyzer = new KAnalyzer();
            analyzer.OnChanged = OnBatchChangedHandler;
        }

        public void OnDisable()
        {
            analyzer.OnChanged = null;
            analyzer.Dispose();
        }
        
        public void Analysis(Action callback)
        {
            this.callback = callback;
            analyzer.Analysis();
        }
        
        private void OnBatchChangedHandler()
        {
            data.groups.Clear();
            
            foreach (var batch in analyzer.batches)
            {
                var group = AllocGroup(batch.canvas);
                group.AddBatch(batch);
            }
            
            callback?.Invoke();
        }
        
        private kCanvas AllocGroup(Canvas canvas)
        {
            var groups = data.groups;
            
            foreach (var g in groups)
            {
                if (g.canvas == canvas) return g;
            }

            var group = new kCanvas(canvas);
            groups.Add(group);

            return group;
        }
    }
}