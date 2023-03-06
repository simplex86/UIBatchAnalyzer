using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

namespace SimpleX
{
    class UIBatchProfilerWindow : EditorWindow
    {
        private UIBatchAnalyzerView view;

        [MenuItem("SimpleX/UIBatch Profiler")]
        private static void OnMenu()
        {
            var window = GetWindow<UIBatchProfilerWindow>("UGUI Batch");
            window.Show();
        }

        private void OnEnable()
        {
            var data = new UIBatchAnalyzerData();
            data.OnEnable();
            
            var ctrl = new UIBatchAnalyzerCtrl(data);
            ctrl.OnEnable();
            
            view = new UIBatchAnalyzerView(data, ctrl);
            view.OnEnable();
        }

        private void OnGUI()
        {
            view?.OnGUI();
            this.Repaint();
        }

        private void Update()
        {
            view?.OnUpdate();
        }

        private void OnDisable()
        {
            view?.OnDisable();
        }

        private void OnDestroy()
        {
            view = null;
        }

        private void OnHierarchyChange()
        {
            view?.OnHierarchyChange();
        }
    }
}