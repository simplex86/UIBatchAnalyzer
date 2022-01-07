using UnityEngine;
using UnityEditor;

namespace XH
{
    class UIBatchAnalyzerWindow : EditorWindow
    {
        private UIBatchAnalyzerView view = null;

        [MenuItem("Window/UIBatch Analyzer")]
        static void Init()
        {
            var model = new UIBatchAnalyzerModel();
            var ctrl = new UIBatchAnalyzerCtrl(model);
            var view = new UIBatchAnalyzerView(model, ctrl);

            var window = GetWindow<UIBatchAnalyzerWindow>();
            window.view = view;
            window.title = "UI Batch";
            window.Show();
        }

        void OnGUI()
        {
            view.OnGUI();
        }

        void OnDestroy()
        {
            view.OnDestroy();
        }
    }
}