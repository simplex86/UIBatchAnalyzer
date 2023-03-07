using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SimpleX
{
    public class UIBatchAnalyzerCtrl
    {
        public Action OnChanged = null;
        
        private UIBatchAnalyzerData data;
        private KAnalyzer analyzer = null;

        public UIBatchAnalyzerCtrl(UIBatchAnalyzerData data)
        {
            this.data = data;
        }
        
        public void OnEnable()
        {
            analyzer = new KAnalyzer();
            analyzer.OnAnalyzed = OnBatchAnalyzedHandler;

            data.enabled = false;
            data.state = EAnalysisState.Idle;
        }

        public void OnDisable()
        {
            data.enabled = false;
            analyzer.OnAnalyzed = null;
            analyzer.Dispose();
        }
        
        public void Analysis()
        {
            if (data.enabled)
            {
                data.groups.Clear();
                data.state = EAnalysisState.Analyzing;
                analyzer.Analysis();
            }
        }
        
        public void Tick()
        {
            if (data.enabled && 
                data.state == EAnalysisState.Analyzing)
            {
                analyzer?.Tick();
            }
        }

        public void Reset()
        {
            data.state = EAnalysisState.Idle;
        }

        public void Clear()
        {
            Reset();
            data.groups.Clear();
            analyzer.Dispose();
        }
        
        private void OnBatchAnalyzedHandler(List<KBatch> batches)
        {
            if (data.enabled)
            {
                data.groups.Clear();

                foreach (var batch in batches)
                {
                    var group = AllocGroup(batch.canvas);
                    group.AddBatch(batch);
                }

                data.state = EAnalysisState.Analyzed;
                OnChanged?.Invoke();
            }
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