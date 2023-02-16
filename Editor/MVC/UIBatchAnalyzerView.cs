using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

namespace SimpleX
{
    public class UIBatchAnalyzerView
    {
        private UIBatchAnalyzerData data;
        private UIBatchAnalyzerCtrl ctrl;
        
        private Vector2 scrollpos = Vector2.zero;

        public UIBatchAnalyzerView(UIBatchAnalyzerData data, UIBatchAnalyzerCtrl ctrl)
        {
            this.data = data;
            this.ctrl = ctrl;
        }

        public void OnEnable()
        {
            
        }
        
        public void OnDisable()
        {
            
        }

        public void OnGUI()
        {
            OnToolbarGUI();
                
            if (data.groups.Count == 0)
            {
                EditorGUILayout.HelpBox("Empty", MessageType.Info);
            }
            else
            {
                scrollpos = EditorGUILayout.BeginScrollView(scrollpos);
                {
                    foreach (var group in data.groups)
                    {
                        OnCanvasGUI(group);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void OnToolbarGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("Analysis", EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    ctrl.Analysis();
                }
                
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnCanvasGUI(VCanvas group)
        {
            var canvas = group.canvas;

            group.expand = UIBatchProfilerGUI.ToggleGroup($"Canvas - {canvas.name}   |   Batch Count : {group.batchCount}", group.expand);
            if (group.expand)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.ObjectField(canvas, typeof(Canvas));
                    foreach (var batch in group.batches)
                    {
                        OnBatchGUI(batch);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void OnBatchGUI(VBatch batch)
        {
            var kbatch = batch.batch;
            
            batch.expand = EditorGUILayout.Foldout(batch.expand, $"Batch", true);
            if (batch.expand)
            {
                UIBatchProfilerGUI.BeginIndent();
                {
                    EditorGUILayout.ObjectField("Material", kbatch.material, typeof(Material));
                    if (kbatch.texture != null)
                    {
                        EditorGUILayout.ObjectField("Texture", kbatch.texture, typeof(Texture));
                    }
                    else if (kbatch.spriteAtlas != null)
                    {
                        EditorGUILayout.ObjectField("Sprite Atlas", kbatch.spriteAtlas, typeof(SpriteAtlas));
                    }

                    EditorGUILayout.LabelField("Widget List", EditorStyles.boldLabel);
                    UIBatchProfilerGUI.BeginIndent();
                    {
                        foreach (var widget in kbatch.widgets)
                        {
                            OnWidgetGUI(widget);
                        }
                    }
                    UIBatchProfilerGUI.EndIndent();
                }
                UIBatchProfilerGUI.EndIndent();
            }
        }

        private void OnWidgetGUI(KWidget widget)
        {
            EditorGUILayout.ObjectField(widget.gameObject.name, widget.gameObject, typeof(GameObject));
        }
    }
}