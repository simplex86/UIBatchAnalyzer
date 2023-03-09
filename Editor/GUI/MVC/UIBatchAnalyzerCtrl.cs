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
            analyzer.OnDirty = OnMaterialDiryHandler;
            analyzer.OnAnalyzed = OnBatchAnalyzedHandler;

            data.enabled = false;
            data.state = EAnalysisState.Idle;
            
            KSpriteAtlas.Load();
        }

        public void OnDisable()
        {
            data.enabled = false;
            analyzer.OnDirty = null;
            analyzer.OnAnalyzed = null;
            analyzer.Dispose();
            
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

        private void OnMaterialDiryHandler()
        {
            data.dirty = true;
        }
    }
}