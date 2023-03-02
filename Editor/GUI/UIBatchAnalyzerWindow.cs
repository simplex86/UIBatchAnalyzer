using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

namespace SimpleX
{
    class UIBatchProfilerWindow : EditorWindow
    {
        private UIBatchAnalyzerData data;
        private UIBatchAnalyzerCtrl ctrl;
        private UIBatchAnalyzerView view;

        [MenuItem("SimpleX/UIBatch Profiler")]
        private static void OnMenu()
        {
            var window = GetWindow<UIBatchProfilerWindow>("UGUI Batch");
            window.Show();
        }

        private void OnEnable()
        {
            data = new UIBatchAnalyzerData();
            data.OnEnable();
            
            ctrl = new UIBatchAnalyzerCtrl(data);
            ctrl.OnEnable();
            
            view = new UIBatchAnalyzerView(data, ctrl);
            view.OnEnable();
        }

        private void OnGUI()
        {
            view?.OnGUI();
            this.Repaint();
        }

        private void OnDisable()
        {
            view?.OnDisable();
            ctrl?.OnDisable();
            data?.OnDisable();
        }

        private void OnDestroy()
        {
            
        }
    }
}