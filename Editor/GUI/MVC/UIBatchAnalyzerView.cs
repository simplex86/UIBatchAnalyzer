using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;

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
        private bool isDirty = false;

        private static GUIStyle _style_ = null;
        private const string _name_ = "UGUI Batch Analyzer";
        private const string _version_ = "v0.9.2";

        public UIBatchAnalyzerView(UIBatchAnalyzerData data, UIBatchAnalyzerCtrl ctrl)
        {
            this.data = data;
            this.ctrl = ctrl;
        }

        public void OnEnable()
        {
            selectedItem = null;
            spliteview = new SpliteView(SpliteView.Direction.Horizontal);
            
            batchview = new SimpleTreeView();
            batchview.onSelectionChanged = OnSelectionChangedHandler;

            SceneView.duringSceneGui += OnSceneGUIHandler;
            EditorApplication.playModeStateChanged += OnPlayModeStateChangedHandler;
            EditorApplication.update += OnUpdate;
            EditorApplication.hierarchyChanged += OnHierarchyChange;
            
            data.OnEnable();
            ctrl.OnEnable();

            ctrl.OnAnalyzed = OnAnalyzedHandler;
        }
        
        public void OnDisable()
        {
            selectedItem = null;
            
            batchview.onSelectionChanged = null;
            batchview = null;
            
            SceneView.duringSceneGui -= OnSceneGUIHandler;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChangedHandler;
            EditorApplication.update -= OnUpdate;
            EditorApplication.hierarchyChanged -= OnHierarchyChange;
            
            data.OnDisable();
            ctrl.OnDisable();
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
                GUILayout.FlexibleSpace();
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
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

            OnStatusGUI();
        }

        public void OnUpdate()
        {
            if (data.enabled)
            {
                if (data.state == EAnalysisState.Analyzing)
                {
                    ctrl?.Tick();
                }
                
                if (data.dirty && 
                    data.state == EAnalysisState.Idle)
                {
                    OnAnalysis();
                }
            }
        }
        
        public void OnHierarchyChange()
        {
            if (data.enabled)
            {
                // OnHierarchyChange 消息可能会延迟几帧，
                // 为了能在Hierarchy变化时自动重新计算合批，暂时采用这种方式
                if (data.state == EAnalysisState.Analyzed)
                {
                    ctrl.ToIdle();
                }
                else if (data.state == EAnalysisState.Idle)
                {
                    data.dirty = true;
                }
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
                    if (_style_ == null)
                    {
                        _style_ = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
                    }
                    EditorGUILayout.LabelField($"{_name_} {_version_}", _style_);
                }
                GUI.color = Color.white;
                
                // GUI.enabled = !analyzing;
                // if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50)))
                // {
                //     OnClear();
                // }
                // GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnStatusGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUI.color = Color.gray;
                if (data.enabled)
                {
                    var canvasCount = data.groups.Count;
                    var batchCount = 0;
                    foreach (var group in data.groups)
                    {
                        batchCount += group.batchCount;
                    }
                    EditorGUILayout.LabelField($"Canvas Count: {canvasCount}, Batch Count: {batchCount}");
                }
                GUI.color = Color.white;
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

        private void OnAnalyzedHandler()
        {
            RebuildBatchView();
        }

        private void RebuildBatchView()
        {
            foreach (var group in data.groups)
            {
                var canvasItem = new SimpleTreeViewItem(group.canvas.name);
                canvasItem.userData = group;
                
                batchview.AddChild(canvasItem);
                batchview.SetExpanded(canvasItem.id, true);

                for (int i=0; i<group.batchCount; i++)
                {
                    var batch = group.batches[i];
                    var batchItem = new SimpleTreeViewItem($"Batch ({i+1} / {group.batchCount})");
                    batchItem.userData = batch;

                    foreach (var instruction in batch.instructions)
                    {
                        var instructionItem = new SimpleTreeViewItem(instruction.name);
                        instructionItem.userData = instruction;
                        
                        batchItem.AddChild(instructionItem);
                    }
                    canvasItem.AddChild(batchItem);

                    batchview.SetExpanded(batchItem.id, false);
                }
            }
            
            if (data.groups.Count > 0)
            {
                batchview.Reload();
                // batchview.ExpandAll();
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
            OnMaskInfoGUI(batch.maskType);
            OnMaterialInfoGUI(batch.material);
            OnSpriteAtlasInfoGUI(batch.spriteAtlas);
            OnTextureInfoGUI(batch.texture);
            EditorGUILayout.TextField("Depth", batch.depth.ToString());
            EditorGUILayout.TextField("Instruction Count", batch.instructionCount.ToString());
            EditorGUILayout.TextField("Vertex Count", batch.vertexCount.ToString());
        }
        
        private void OnInstructionGUI(KInstruction instruction)
        {
            EditorGUILayout.ObjectField("Game Object", instruction.gameObject, typeof(GameObject));
            OnMaterialInfoGUI(instruction.material);
            OnSpriteAtlasInfoGUI(instruction.spriteAtlas);
            OnTextureInfoGUI(instruction.texture);
            EditorGUILayout.TextField("Depth", instruction.depth.ToString());
            EditorGUILayout.TextField("Render Order", instruction.renderOrder.ToString());
            EditorGUILayout.TextField("Vertex Count", instruction.vertexCount.ToString());
        }

        private void OnMaskInfoGUI(EMaskType maskType)
        {
            if (maskType == EMaskType.Mask)
            {
                EditorGUILayout.HelpBox("Batch for Mask", MessageType.Info);
            }
            else if (maskType == EMaskType.Unmask)
            {
                EditorGUILayout.HelpBox("Batch for Unmask", MessageType.Info);
            }
        }

        private void OnMaterialInfoGUI(Material material)
        {
            if (material != null)
            {
                EditorGUILayout.ObjectField("Material", material, typeof(Material));

                var materialIID = material.GetInstanceID();
                EditorGUILayout.HelpBox($"ID: {materialIID}", MessageType.Info);
            }
        }
        
        private void OnSpriteAtlasInfoGUI(SpriteAtlas spriteAtlas)
        {
            if (spriteAtlas != null)
            {
                EditorGUILayout.ObjectField("Sprite Atlas", spriteAtlas, typeof(SpriteAtlas));

                var textureIID = spriteAtlas.GetInstanceID();
                EditorGUILayout.HelpBox($"ID: {textureIID}", MessageType.Info);
            }
        }
        
        private void OnTextureInfoGUI(Texture texture)
        {
            if (texture != null)
            {
                EditorGUILayout.ObjectField("Texture", texture, typeof(Texture));

                var textureIID = texture.GetInstanceID();
                EditorGUILayout.HelpBox($"ID: {textureIID}", MessageType.Info);
            }
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