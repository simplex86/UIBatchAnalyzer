using UnityEngine;
using UnityEditor;

namespace SimpleX
{
    public class UIBatchAnalyzerView
    {
        private UIBatchAnalyzerData data;
        private UIBatchAnalyzerCtrl ctrl;

        private SpliteView spliteview = null;
        private SimpleTreeView batchview = null;

        private object selectedItem = null;
        private Color gizmosColor = Color.red;

        private const string _name_ = "UGUI Batch Analyzer";
        private const string _version_ = "v0.7.2";

        public UIBatchAnalyzerView(UIBatchAnalyzerData data, UIBatchAnalyzerCtrl ctrl)
        {
            this.data = data;
            this.ctrl = ctrl;
            
            ctrl.OnChanged = OnChangedHandler;
        }

        public void OnEnable()
        {
            spliteview = new SpliteView(SpliteView.Direction.Horizontal);
            
            batchview = new SimpleTreeView();
            batchview.onSelectionChanged = OnSelectionChangedHandler;

            SceneView.duringSceneGui += OnSceneGUIHandler;
            EditorApplication.playModeStateChanged += OnPlayModeStateChangedHandler;

            selectedItem = null;
        }
        
        public void OnDisable()
        {
            batchview.onSelectionChanged = null;
            batchview = null;
            
            SceneView.duringSceneGui -= OnSceneGUIHandler;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChangedHandler;

            selectedItem = null;
        }

        public void OnGUI()
        {
            OnToolbarGUI();

            if (data.groups.Count == 0)
            {
                if (data.state == EAnalysisState.Idle)
                {
                    EditorGUILayout.HelpBox("UGUI Batch Analyzer show you the batches of UGUI. Click 'Enable' Now!", MessageType.Info);
                }
                else if (data.state == EAnalysisState.Analyzing)
                {
                    EditorGUILayout.HelpBox("Analyzing Now, please wait", MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                {
                    spliteview.BeginSplitView();
                    OnBatchesViewGUI();
                    spliteview.Split();
                    OnDetailsViewGUI();
                    spliteview.EndSplitView();
                }
                EditorGUILayout.EndHorizontal();

                if (selectedItem != null)
                {
                    SceneView.RepaintAll();
                }
            }
        }

        public void OnUpdate()
        {
            ctrl?.Tick();
        }
        
        public void OnHierarchyChange()
        {
            if (!data.enabled) return;
            selectedItem = null;
            // OnHierarchyChange 消息可能会延迟几帧，
            // 为了能在Hierarchy变化时自动重新计算合批，暂时采用这种方式
            if (data.state == EAnalysisState.Analyzed)
            {
                ctrl.Reset();
            }
            else if (data.state == EAnalysisState.Idle)
            {
                OnAnalysis();
            }
        }

        private void OnToolbarGUI()
        {
            var analyzing = (data.state == EAnalysisState.Analyzing);
            
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUI.enabled = !analyzing;
                if (data.enabled)
                {
                    if (GUILayout.Button("Disable", EditorStyles.toolbarButton, GUILayout.Width(80)))
                    {
                        data.enabled = false;
                        OnClear();
                    }
                }
                else
                {
                    if (GUILayout.Button("Enable", EditorStyles.toolbarButton, GUILayout.Width(80)))
                    {
                        data.enabled = true;
                        OnAnalysis();
                    }
                }
                GUI.enabled = true;
                
                EditorGUILayout.LabelField(new GUIContent("Gizmos Color"), GUILayout.Width(80));
                gizmosColor = EditorGUILayout.ColorField(GUIContent.none, gizmosColor, false, true, false, GUILayout.Width(20));
                
                GUILayout.FlexibleSpace();

                GUI.color = Color.gray;
                {
                    EditorGUILayout.LabelField($"{_name_} {_version_}", GUILayout.Width(170));
                }
                GUI.color = Color.white;
                
                GUI.enabled = !analyzing;
                if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    OnClear();
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnAnalysis()
        {
            batchview.Clear();
            selectedItem = null;
            
            ctrl.Analysis();
        }

        private void OnClear()
        {
            batchview.Clear();
            selectedItem = null;
            
            ctrl.Clear();
        }

        private void OnChangedHandler()
        {
            RebuildBatchView();
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
            var rect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            {
                batchview.OnGUI(new Rect(0, 0, rect.width, rect.height));
            }
            EditorGUILayout.EndVertical();
        }

        private void OnDetailsViewGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            {
                if (selectedItem == null)
                {
                    EditorGUILayout.HelpBox("Select a item, then the information will be shown here", MessageType.Info);
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
            EditorGUILayout.TextField("Batch Count", canvas.batchCount.ToString());
            EditorGUILayout.TextField("Instruction Count", canvas.instructionCount.ToString());
            EditorGUILayout.TextField("Vertex Count", canvas.vertexCount.ToString());
        }
        
        private void OnBatchGUI(KBatch batch)
        {
            EditorGUILayout.ObjectField("Material", batch.material, typeof(GameObject));
            EditorGUILayout.ObjectField("Texture", batch.texture, typeof(Texture));
            OnMaterialInfoGUI(batch.isMask, batch.isUnmask, batch.material, batch.texture);
            EditorGUILayout.TextField("Depth", batch.depth.ToString());
            EditorGUILayout.TextField("Instruction Count", batch.instructionCount.ToString());
            EditorGUILayout.TextField("Vertex Count", batch.vertexCount.ToString());
        }
        
        private void OnInstructionGUI(KInstruction instruction)
        {
            EditorGUILayout.ObjectField("Game Object", instruction.gameObject, typeof(GameObject));
            EditorGUILayout.ObjectField("Material", instruction.material, typeof(Material));
            EditorGUILayout.ObjectField("Texture", instruction.texture, typeof(Texture));
            OnMaterialInfoGUI(instruction.isMask, instruction.isUnmask, instruction.material, instruction.texture);
            EditorGUILayout.TextField("Depth", instruction.depth.ToString());
            EditorGUILayout.TextField("Render Order", instruction.renderOrder.ToString());
            EditorGUILayout.TextField("Vertex Count", instruction.vertexCount.ToString());
        }

        private void OnMaterialInfoGUI(bool isMask, bool isUnmask, Material material, Texture texture)
        {
            var text = "";
            if (isMask)
            {
                text = "Mask\n\n";
            }
            else if (isUnmask)
            {
                text = "Unmask\n\n";
            }
            
            var materialName = material.name;
            var materialIID = material.GetInstanceID();
            text += $"Material\n    Name = {materialName}\n    ID = {materialIID}\n";
            var textureName = texture.name;
            var textureIID = texture.GetInstanceID();
            text += $"Texture\n    Name = {textureName}\n    ID = {textureIID}";
            EditorGUILayout.HelpBox(text, MessageType.None);
        }
        
        private void OnSceneGUIHandler(SceneView sceneView)
        {
            if (selectedItem != null)
            {
                var defaultColor = Handles.color;

                Handles.color = gizmosColor;
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
        
        private void OnPlayModeStateChangedHandler(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode ||
                state == PlayModeStateChange.ExitingPlayMode)
            {
                data.enabled = false;
                OnClear();
            }
        }
    }
}