﻿using UnityEngine;
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

            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        public void OnDisable()
        {
            batchview.onSelectionChanged = null;
            batchview = null;
            
            SceneView.duringSceneGui -= OnSceneGUI;
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

                if (selectedItem != null)
                {
                    SceneView.RepaintAll();
                }
            }
            
            ctrl.Tick();
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

                    foreach (var instruction in batch.instructions)
                    {
                        var instructionItem = new SimpleTreeViewItem(instruction.name);
                        instructionItem.what = instruction;
                        
                        batchItem.AddChild(instructionItem);
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

                GUI.color = Color.green;
                if (GUILayout.Button("Sample"))
                {
                    OnAnalysis();
                }
                GUI.color = Color.white;
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
                else if (selectedItem is KInstruction)
                {
                    OnInstructionGUI(selectedItem as KInstruction);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void OnCanvasGUI(kCanvas canvas)
        {
            EditorGUILayout.ObjectField("Canvas", canvas.canvas, typeof(Canvas));
            EditorGUILayout.Space(4);
            EditorGUILayout.TextField("Batch Count", canvas.batchCount.ToString());
            EditorGUILayout.TextField("Instruction Count", canvas.instructionCount.ToString());
            EditorGUILayout.TextField("Vertex Count", canvas.vertexCount.ToString());
        }
        
        private void OnBatchGUI(KBatch batch)
        {
            EditorGUILayout.ObjectField("Material", batch.material, typeof(GameObject));
            EditorGUILayout.ObjectField("Texture", batch.texture, typeof(Texture));
            EditorGUILayout.Space(4);
            EditorGUILayout.TextField("Instruction Count", batch.instructionCount.ToString());
            EditorGUILayout.TextField("Vertex Count", batch.vertexCount.ToString());
        }
        
        private void OnInstructionGUI(KInstruction instruction)
        {
            EditorGUILayout.ObjectField("Game Object", instruction.gameObject, typeof(GameObject));
            EditorGUILayout.Toggle("Mask", instruction.isMask);
            EditorGUILayout.Toggle("Unmask", instruction.isUnmask);
            EditorGUILayout.ObjectField("Material", instruction.material, typeof(Material));
            EditorGUILayout.ObjectField("Texture", instruction.texture, typeof(Texture));
            EditorGUILayout.Space(4);
            EditorGUILayout.TextField("Depth", instruction.depth.ToString());
            EditorGUILayout.TextField("Render Order", instruction.renderOrder.ToString());
            EditorGUILayout.TextField("Vertex Count", instruction.vertexCount.ToString());
        }
        
        private void OnSceneGUI(SceneView sceneView)
        {
            if (selectedItem != null)
            {
                var defaultColor = Handles.color;

                Handles.color = Color.red;
                {
                    if (selectedItem is kCanvas)
                    {
                        OnDrawCanvasGizmos(selectedItem as kCanvas);
                    }
                    else if (selectedItem is KBatch)
                    {
                        OnDrawBatchGizmos(selectedItem as KBatch);
                    }
                    else if (selectedItem is KInstruction)
                    {
                        OnDrawInstructionGizmos(selectedItem as KInstruction);
                    }
                }
                Handles.color = defaultColor;
            }
        }

        private void OnDrawCanvasGizmos(kCanvas canvas)
        {
            foreach (var batch in canvas.batches)
            {
                OnDrawBatchGizmos(batch);
            }
        }
        
        private void OnDrawBatchGizmos(KBatch batch)
        {
            foreach (var instruction in batch.instructions)
            {
                OnDrawInstructionGizmos(instruction);
            }
        }
        
        private void OnDrawInstructionGizmos(KInstruction instruction)
        {
            OnDrawMeshGizmos(instruction.mesh);
        }

        private void OnDrawMeshGizmos(KMesh mesh)
        {
            foreach (var t in mesh.triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    Handles.DrawLine(t[i + 0], t[i + 1]);
                }
            }
        }

        private void OnSelectionChangedHandler(object seleced)
        {
            selectedItem = seleced;
        }
    }
}