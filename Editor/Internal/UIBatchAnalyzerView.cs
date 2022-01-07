using UnityEngine;
using UnityEditor;

namespace XH
{
    class UIBatchAnalyzerView
    {
        private UIBatchAnalyzerModel model = null;
        private UIBatchAnalyzerCtrl ctrl = null;

        public UIBatchAnalyzerView(UIBatchAnalyzerModel model, UIBatchAnalyzerCtrl ctrl)
        {
            this.model = model;
            this.ctrl = ctrl;
        }

        public void OnGUI()
        {
            for (int i=0; i<model.batches.Count; i++)
            {
                var batch = model.batches[i];

                BeginToggleGroup($"Batch {i}", true);
                EditorGUILayout.LabelField("Depth", batch.depth.ToString());
                foreach (var w in batch.widgets)
                {
                    EditorGUILayout.ObjectField(w.gameObject, typeof(GameObject));
                }
                EndToggleGroup();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Run", GUILayout.Height(32)))
            {
                ctrl.Run();
            }
        }

        public void OnDestroy()
        {
            ctrl.Cleanup();
        }

        private bool BeginToggleGroup(string title, bool expand)
        {
            expand = GUILayout.Toggle(expand, "", EditorStyles.miniButton, GUILayout.Height(18));
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.Label(new Rect(rect.x + 2, rect.y + 0, rect.width, rect.height), title);

            GUILayout.Space(-2);
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            GUILayout.Space(2);

            return expand;
        }

        public static void EndToggleGroup()
        {
            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }
    }
}