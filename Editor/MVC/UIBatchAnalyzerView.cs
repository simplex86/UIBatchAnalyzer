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
        private static readonly Color STATISTICS_TITLE_COLOR = new Color(1.0f, 1.0f, 0.0f);

        public UIBatchAnalyzerView(UIBatchAnalyzerData data, UIBatchAnalyzerCtrl ctrl)
        {
            this.data = data;
            this.ctrl = ctrl;
        }

        public void OnEnable()
        {
            
        }

        public void OnGUI()
        {
            var groups = data.groups;
            
            if (groups.Count == 0)
            {
                EditorGUILayout.HelpBox("Empty", MessageType.Info);
            }
            else
            {
                scrollpos = EditorGUILayout.BeginScrollView(scrollpos);
                // 统计数据，例如顶点总数量，GameObject总数量等
                int canvasCount = groups.Count;
                int gameObjectCount = 0;
                int vertexCount = 0;
                int batchCount = 0;
                foreach (var group in groups)
                {
                    gameObjectCount += group.gameObjectCount;
                    vertexCount += group.vertexCount;
                    batchCount += group.batchCount;
                }
                UIBatchProfilerGUI.BeginVerticalGroup("♨ Statistics", STATISTICS_TITLE_COLOR);
                {
                    EditorGUILayout.LabelField("Canvas Count", canvasCount.ToString());
                    EditorGUILayout.LabelField("GameObject Count", gameObjectCount.ToString());
                    EditorGUILayout.LabelField("Vertex Count", vertexCount.ToString());
                    EditorGUILayout.LabelField("Batch Count", batchCount.ToString());
                }
                UIBatchProfilerGUI.EndVerticalGroup();
                // Canvas列表
                foreach (var group in groups)
                {
                    var title = $"{group.canvas.gameObject.name} - {group.batches.Count}";
                    DrawGroup(title, group);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.FlexibleSpace();

            GUI.color = Color.green;
            if (GUILayout.Button("Analysis", GUILayout.Height(32)))
            {
                ctrl.Analysis();
            }
            GUI.color = Color.white;
        }

        public void OnDisable()
        {
            
        }
        
        private static Color GROUP_DEFAULT_COLOR = Color.magenta;
        private void DrawGroup(string title, VCanvas group)
        {
            var batches = group.batches;

            group.expand = UIBatchProfilerGUI.ToggleGroup(title, GROUP_DEFAULT_COLOR, group.expand);
            if (group.expand)
            {
                UIBatchProfilerGUI.BeginVerticalGroup();
                {
                    EditorGUILayout.ObjectField("Canvas", group.canvas, typeof(Canvas));
                    // 顶点总数量，GameObject总数量等
                    EditorGUILayout.LabelField("GameObject Count", group.gameObjectCount.ToString());
                    EditorGUILayout.LabelField("Vertex Count", group.vertexCount.ToString());
                    EditorGUILayout.LabelField("Batch Count", batches.Count.ToString());
                    for (int i=0; i<batches.Count; i++)
                    {
                        DrawBatch($"Batch {i+1}", batches[i]);
                    }
                }
                UIBatchProfilerGUI.EndVerticalGroup();
            }
        }

        private static Color BATCH_DEFAULT_COLOR = Color.cyan;
        private void DrawBatch(string title, VBatch batch)
        {
            var value = batch.batch;

            batch.expand = UIBatchProfilerGUI.ToggleGroup(title, BATCH_DEFAULT_COLOR, batch.expand);
            if (batch.expand)
            {
                // 参数列表
                UIBatchProfilerGUI.BeginVerticalGroup("≣ Parameters");
                {
                    EditorGUILayout.LabelField("Vertex Count", value.vertexCount.ToString());
                    EditorGUILayout.ObjectField("Material", value.material, typeof(Material));
                    if (value.spriteAtlas == null)
                    {
                        EditorGUILayout.ObjectField("Texture", value.texture, typeof(Texture));
                    }
                    else
                    {
                        EditorGUILayout.ObjectField("Sprite Atlas", value.spriteAtlas, typeof(SpriteAtlas));
                    }
                }
                UIBatchProfilerGUI.EndVerticalGroup();
                // 节点列表
                UIBatchProfilerGUI.BeginVerticalGroup("≣ Widgets");
                foreach (var w in value.widgets)
                {
                    DrawWidget(w);
                }
                UIBatchProfilerGUI.EndVerticalGroup();
            }
        }

        private void DrawWidget(KWidget widget)
        {
            EditorGUILayout.ObjectField(widget.gameObject, typeof(GameObject));
        }
    }
}