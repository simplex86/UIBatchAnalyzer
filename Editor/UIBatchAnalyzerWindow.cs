using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

namespace XH
{
    class UIBatchAnalyzerWindow : EditorWindow
    {
        class VBatch
        {
            public KBatch batch { get; private set; } = null;
            public bool expand { get; set; } = false;

            public VBatch(KBatch batch)
            {
                this.batch = batch;
            }
        }

        class VCanvas
        {
            public Canvas canvas { get; private set; } = null;
            public List<VBatch> batches { get; } = new List<VBatch>();
            public int gameObjectCount { get; private set; } = 0;
            public int vertexCount { get; private set; } = 0;
            public int batchCount { get { return batches.Count; }}
            public bool expand { get; set; } = true;

            public VCanvas(Canvas canvas)
            {
                this.canvas = canvas;
            }

            public void AddBatch(KBatch batch)
            {
                var vbatch = new VBatch(batch);
                batches.Add(vbatch);

                gameObjectCount += batch.widgetCount;
                vertexCount += batch.vertexCount;
            }
        }

        private List<VCanvas> groups = new List<VCanvas>();
        private Vector2 scrollpos = Vector2.zero;

        [MenuItem("Window/UIBatch Analyzer")]
        private static void OnMenu()
        {
            var window = GetWindow<UIBatchAnalyzerWindow>("UGUI Batch");
            window.Show();
        }

        private static Color STATISTICS_TITLE_COLOR = new Color(1.0f, 1.0f, 0.0f);

        private void OnGUI()
        {
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
                UIBatchAnalyzerGUI.BeginVerticalGroup("♨ Statistics", STATISTICS_TITLE_COLOR);
                {
                    EditorGUILayout.LabelField("Canvas Count", canvasCount.ToString());
                    EditorGUILayout.LabelField("GameObject Count", gameObjectCount.ToString());
                    EditorGUILayout.LabelField("Vertex Count", vertexCount.ToString());
                    EditorGUILayout.LabelField("Batch Count", batchCount.ToString());
                }
                UIBatchAnalyzerGUI.EndVerticalGroup();
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
                Analysis();
            }
            GUI.color = Color.white;
        }

        private void OnDestroy()
        {
            UIBatchProvider.Instance.Dispose();
        }

        private void Analysis()
        {
            UIBatchProvider.Instance.OnChanged = OnBatchChangedHandler;
            UIBatchProvider.Instance.Analysis();
        }

        private void OnBatchChangedHandler(List<KBatch> batches)
        {
            groups.Clear();
            foreach (var batch in batches)
            {
                var group = AllocGroup(batch.canvas);
                group.AddBatch(batch);
            }
            Repaint();
        }

        private VCanvas AllocGroup(Canvas canvas)
        {
            foreach (var g in groups)
            {
                if (g.canvas == canvas) return g;
            }

            var group = new VCanvas(canvas);
            groups.Add(group);

            return group;
        }

        private static Color GROUP_DEFAULT_COLOR = Color.magenta;
        private void DrawGroup(string title, VCanvas group)
        {
            var batches = group.batches;

            group.expand = UIBatchAnalyzerGUI.ToggleGroup(title, GROUP_DEFAULT_COLOR, group.expand);
            if (group.expand)
            {
                UIBatchAnalyzerGUI.BeginVerticalGroup();
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
                UIBatchAnalyzerGUI.EndVerticalGroup();
            }
        }

        private static Color BATCH_DEFAULT_COLOR = Color.cyan;
        private void DrawBatch(string title, VBatch batch)
        {
            var value = batch.batch;

            batch.expand = UIBatchAnalyzerGUI.ToggleGroup(title, BATCH_DEFAULT_COLOR, batch.expand);
            if (batch.expand)
            {
                // 参数列表
                UIBatchAnalyzerGUI.BeginVerticalGroup("≣ Parameters");
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
                UIBatchAnalyzerGUI.EndVerticalGroup();
                // 节点列表
                UIBatchAnalyzerGUI.BeginVerticalGroup("≣ Widgets");
                foreach (var w in value.widgets)
                {
                    DrawWidget(w);
                }
                UIBatchAnalyzerGUI.EndVerticalGroup();
            }
        }

        private void DrawWidget(KWidget widget)
        {
            EditorGUILayout.ObjectField(widget.gameObject, typeof(GameObject));
        }
    }
}