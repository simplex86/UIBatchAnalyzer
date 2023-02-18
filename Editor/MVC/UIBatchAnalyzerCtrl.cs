using System.Collections.Generic;
using UnityEngine;

namespace SimpleX
{
    public class UIBatchAnalyzerCtrl
    {
        private UIBatchAnalyzerData data;

        public System.Action callback = null;

        public UIBatchAnalyzerCtrl(UIBatchAnalyzerData data)
        {
            this.data = data;
        }
        
        public void OnEnable()
        {
            UIBatchProvider.Instance.OnChanged = OnBatchChangedHandler;
        }

        public void OnDisable()
        {
            UIBatchProvider.Instance.OnChanged = null;
        }
        
        public void Analysis(System.Action callback)
        {
            this.callback = callback;
            UIBatchProvider.Instance.Analysis();
        }
        
        private void OnBatchChangedHandler(List<KBatch> batches)
        {
            data.groups.Clear();
            foreach (var batch in batches)
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