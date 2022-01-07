using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace XH
{
    class UIBatchAnalyzerCtrl
    {
        private UIBatchAnalyzerModel model = null;
        private KAnalyzer analyzer = new KAnalyzer();

        public UIBatchAnalyzerCtrl(UIBatchAnalyzerModel model)
        {
            this.model = model;
            analyzer.OnCompleted = (batches) => {
                foreach (var b in batches)
                {
                    var text = $"batch {b.depth}: ";
                    foreach (var w in b.widgets)
                    {
                        text += $"{w.name}, ";
                    }
                    Debug.Log(text);
                }

                model.batches.Clear();
                foreach (var b in batches)
                {
                    model.batches.Add(b);
                }
            };
        }

        public void Run()
        {
            analyzer.Analysis();
        }

        public void Cleanup()
        {
            analyzer.Dispose();
        }
    }
}