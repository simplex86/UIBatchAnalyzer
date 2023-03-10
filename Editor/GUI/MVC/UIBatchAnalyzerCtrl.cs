using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SimpleX
{
    public class UIBatchAnalyzerCtrl
    {
        public Action OnAnalyzed = null;
        
        private UIBatchAnalyzerData data;
        private KAnalyzer analyzer = null;

        public UIBatchAnalyzerCtrl(UIBatchAnalyzerData data)
        {
            this.data = data;
        }
        
        public void OnEnable()
        {
            analyzer = new KAnalyzer();
            analyzer.OnDirty = OnDirtyHandler;
            analyzer.OnAnalyzed = OnBatchAnalyzedHandler;

            data.enabled = false;
            data.state = EAnalysisState.Idle;
            
            KSpriteAtlas.Load();
        }

        public void OnDisable()
        {
            analyzer.Dispose();
            
            data.enabled = false;
            data.state = EAnalysisState.Idle;
            data.groups.Clear();
            
            KSpriteAtlas.Clear();
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
            analyzer?.Tick();
        }

        public void ToIdle()
        {
            data.state = EAnalysisState.Idle;
        }

        public void Clear()
        {
            ToIdle();
            data.groups.Clear();
            analyzer.Clear();
        }
        
        private void OnBatchAnalyzedHandler(List<KBatch> batches)
        {
            data.dirty = false;
            
            if (data.enabled)
            {
                data.groups.Clear();

                foreach (var batch in batches)
                {
                    var group = AllocGroup(batch.canvas);
                    group.AddBatch(batch);
                }

                data.state = EAnalysisState.Analyzed;
                OnAnalyzed?.Invoke();
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

        private void OnDirtyHandler()
        {
            data.dirty = true;
        }
    }
}