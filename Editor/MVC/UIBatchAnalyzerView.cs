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
        private SimpleTreeView batchview = null;

        private object selectedItem = null;

        public UIBatchAnalyzerView(UIBatchAnalyzerData data, UIBatchAnalyzerCtrl ctrl)
        {
            this.data = data;
            this.ctrl = ctrl;
        }

        public void OnEnable()
        {
            scrollpos = Vector2.zero;
            
            batchview = new SimpleTreeView();
            batchview.onSelectionChanged = OnSelectionChangedHandler;
        }
        
        public void OnDisable()
        {
            
        }

        public void OnGUI()
        {
            if (data.groups.Count == 0)
            {
                EditorGUILayout.HelpBox("Click 'Sample' button to XXXX", MessageType.Info);
                
                GUI.color = Color.green;
                if (GUILayout.Button("Sample"))
                {
                    OnAnalysis();
                }
                GUI.color = Color.white;
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                {
                    OnBatchesViewGUI();
                    OnDetailsViewGUI();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void OnAnalysis()
        {
            batchview.Clear();
            selectedItem = null;
            
            ctrl.Analysis(() =>
            {
                RebuildBatchView();
            });
        }

        private void RebuildBatchView()
        {
            foreach (var group in data.groups)
            {
                var canvasItem = new SimpleTreeViewItem(group.canvas.name);
                canvasItem.what = group;

                foreach (var batch in group.batches)
                {
                    var batchItem = new SimpleTreeViewItem("Batch");
                    batchItem.what = batch;

                    foreach (var widget in batch.widgets)
                    {
                        var widgetItem = new SimpleTreeViewItem(widget.name);
                        widgetItem.what = widget;
                        
                        batchItem.AddChild(widgetItem);
                    }
                    
                    canvasItem.AddChild(batchItem);
                }
                
                batchview.AddChild(canvasItem);
            }
            
            if (data.groups.Count > 0)
            {
                batchview.Reload();
                batchview.ExpandAll();
            }
        }

        private void OnBatchesViewGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            {
                var rect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                {
                    scrollpos = EditorGUILayout.BeginScrollView(scrollpos);
                    {
                        batchview.OnGUI(new Rect(rect));
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("Sample"))
                {
                    OnAnalysis();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void OnDetailsViewGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.Width(400));
            {
                if (selectedItem == null)
                {
                    EditorGUILayout.HelpBox("Select a item please, and the information will be shown here", MessageType.Info);
                }
                else if (selectedItem is kCanvas)
                {
                    OnCanvasGUI(selectedItem as kCanvas);
                }
                else if (selectedItem is KBatch)
                {
                    OnBatchGUI(selectedItem as KBatch);
                }
                else if (selectedItem is KWidget)
                {
                    OnWidgetGUI(selectedItem as KWidget);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void OnCanvasGUI(kCanvas canvas)
        {
            EditorGUILayout.ObjectField("Canvas", canvas.canvas, typeof(Canvas));
            EditorGUILayout.Space(4);
            EditorGUILayout.TextField("Batch Count", canvas.batchCount.ToString());
            EditorGUILayout.TextField("Widget Count", canvas.widgetCount.ToString());
            EditorGUILayout.TextField("Vertex Count", canvas.vertexCount.ToString());
        }
        
        private void OnBatchGUI(KBatch batch)
        {
            EditorGUILayout.ObjectField("Material", batch.material, typeof(GameObject));
            EditorGUILayout.ObjectField("Texture", batch.texture, typeof(Texture));
            EditorGUILayout.Space(4);
            EditorGUILayout.TextField("Widget Count", batch.widgetCount.ToString());
            EditorGUILayout.TextField("Vertex Count", batch.vertexCount.ToString());
        }
        
        private void OnWidgetGUI(KWidget widget)
        {
            EditorGUILayout.ObjectField("Widget", widget.gameObject, typeof(GameObject));
            EditorGUILayout.ObjectField("Material", widget.material, typeof(Material));
            EditorGUILayout.ObjectField("Texture", widget.texture, typeof(Texture));
            EditorGUILayout.Space(4);
            EditorGUILayout.TextField("Depth", widget.depth.ToString());
            EditorGUILayout.TextField("Render Order", widget.renderOrder.ToString());
            EditorGUILayout.TextField("Vertex Count", widget.vertexCount.ToString());
        }

        private void OnSelectionChangedHandler(object seleced)
        {
            selectedItem = seleced;
        }
    }
}