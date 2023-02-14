using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SimpleX
{
    class UIBatchProvider
    {
        public Action<List<KBatch>> OnChanged;
        private KAnalyzer analyzer = new KAnalyzer();

        public static UIBatchProvider Instance { get; } = new UIBatchProvider();

        protected UIBatchProvider()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChangedHandler;
        }

        public void Analysis()
        {
            analyzer.OnChanged = OnChangedHandler;
            analyzer.Analysis();
        }

        public void Dispose()
        {
            analyzer.Dispose();
            OnChanged = null;
        }

        private void OnChangedHandler()
        {
            OnChanged?.Invoke(analyzer.batches);
        }

        private void OnPlayModeStateChangedHandler(PlayModeStateChange state)
        {
            analyzer.Dispose();

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Analysis();
            }
        }
    }
}