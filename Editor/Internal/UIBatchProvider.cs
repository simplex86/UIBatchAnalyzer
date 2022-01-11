using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XH
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
            if (OnChanged != null)
            {
                OnChanged(analyzer.batches);
            }
        }

        private void OnPlayModeStateChangedHandler(PlayModeStateChange state)
        {
            Dispose();

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Analysis(); // TODO 窗口上看不到数据，暂时还没调试
            }
        }
    }
}