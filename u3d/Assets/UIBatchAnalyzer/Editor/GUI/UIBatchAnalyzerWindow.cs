using UnityEditor;

namespace SimpleX
{
    class UIBatchAnalyzerWindow : EditorWindow
    {
        private UIBatchAnalyzerView view;

        [MenuItem("SimpleX/UGUI Batch Analyzer")]
        private static void OnMenu()
        {
            var window = GetWindow<UIBatchAnalyzerWindow>("UGUI Batch Analyzer");
            window.Show();
        }

        private void OnEnable()
        {
            var data = new UIBatchAnalyzerData();
            data.OnEnable();
            
            var ctrl = new UIBatchAnalyzerCtrl(data);
            ctrl.OnEnable();
            
            view = new UIBatchAnalyzerView(data, ctrl, "v1.0.4");
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
        }

        private void OnDestroy()
        {
            view = null;
        }
    }
}