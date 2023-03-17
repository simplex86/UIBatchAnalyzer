using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleX
{
    public class KAnalyzer
    {
        private class WCanvas
        {
            public Canvas canvas { get; } = null;
            public List<KInstruction> instructions { get; } = new List<KInstruction>();

            public WCanvas(Canvas canvas)
            {
                this.canvas = canvas;
            }
        }
        
        private int totalMeshCount = 0;
        private int injectMeshCount = 0;
        private List<WCanvas> wcanvas = new List<WCanvas>();
        private List<KBatch> batches = new List<KBatch>();
        private bool completed = false;
        
        public Action OnDirty;

        public Action<List<KBatch>> OnAnalyzed;
        
        public void Analysis()
        {
            Clear();

            var canvases = Transform.FindObjectsOfType<Canvas>(false);
            // 注入mesh
            foreach (var canvas in canvases)
            {
                if (IsCanvasEnabled(canvas))
                {
                    GetRenderabledGraphics(canvas);
                }
            }
        }

        public void Tick()
        {
            if (completed)
            {
                UninjectMeshes();
                
                foreach (var v in wcanvas)
                {
                    Analysis(v.canvas, v.instructions);
                }
                OnAnalyzed?.Invoke(batches);
                
                Clear();
            }
        }

        // Canvas是否可用
        private bool IsCanvasEnabled(Canvas canvas)
        {
            return canvas.gameObject.activeInHierarchy && canvas.enabled;
        }

        public void Clear()
        {
            completed = false;
            totalMeshCount = 0;
            injectMeshCount = 0;
            
            batches.Clear();
            wcanvas.Clear();
            
            UninjectMeshes();
        }

        public void Dispose()
        {
            Clear();
            OnDirty = null;
            OnAnalyzed = null;
        }

        private void InjectMesh(MaskableGraphic graphic, KMesh kmesh)
        {
            totalMeshCount++;
            
            var tmpro = graphic.GetComponent<TMPro.TextMeshProUGUI>();
            if (tmpro != null)
            {
                injectMeshCount++;
                kmesh.Fill(tmpro.mesh);

                completed = (totalMeshCount == injectMeshCount);
            }
            else
            {
                

                var uiMesh = graphic.GetComponent<UIMesh>();
                if (uiMesh != null) // 如果不销毁，没有办法进行多次计算
                {
                    GameObject.DestroyImmediate(uiMesh);
                }

                uiMesh = graphic.gameObject.AddComponent<UIMesh>();
                uiMesh.userData = kmesh;

                // mesh计算是在子线程中进行的，所以这里将注入mesh数量和总mesh数量进行对比
                // 仅在最后一个mesh计算完成后才开始对所有canvas进行合批分析，避免线程的同步问题
                uiMesh.OnMeshChanged = (mesh, userData) => {
                    injectMeshCount++;

                    var kmesh = userData as KMesh;
                    kmesh.Fill(mesh);

                    completed = (totalMeshCount == injectMeshCount);
                };
            }
        }

        private void UninjectMeshes()
        {
            var meshes = Transform.FindObjectsOfType<UIMesh>();
            foreach (var m in meshes)
            {
                GameObject.DestroyImmediate(m);
            }
        }

        private List<KInstruction> AllocInstructions(Canvas canvas)
        {
            foreach (var v in wcanvas)
            {
                if (v.canvas == canvas)
                {
                    return v.instructions;
                }
            }

            var c = new WCanvas(canvas);
            wcanvas.Add(c);

            return c.instructions;
        }

        // 深度优先遍历所有可渲染的子节点
        private void GetRenderabledGraphics(Canvas canvas)
        {
            var renderOrder = 0;
            GetRenderabledGraphics(canvas.gameObject, canvas, renderOrder);
        }

        // 获得渲染列表
        private int GetRenderabledGraphics(GameObject gameObject, Canvas canvas, int renderOrder)
        {
            if (gameObject.activeInHierarchy)
            {
                KMesh kmesh = null;
                var instructions = AllocInstructions(canvas);
                
                var graphic = gameObject.GetComponent<MaskableGraphic>();
                var mask = gameObject.GetComponent<Mask>();
                var rectmask2d = GetRectMask2D(gameObject.transform);
                
                var renderabled = IsRenderabledGraphic(graphic);
                if (renderabled)
                {
                    UnregisterGraphicDirtyHandlers(graphic);
                    RegisterGraphicDirtyHandlers(graphic);
                    
                    kmesh = new KMesh(gameObject.transform);
                    if (mask != null)
                    {
                        instructions.Add(new KInstruction(graphic, kmesh, renderOrder, mask, EMaskType.Mask));
                    }
                    else if (rectmask2d != null)
                    {
                        instructions.Add(new KInstruction(graphic, kmesh, renderOrder, rectmask2d));
                    }
                    else
                    {
                        instructions.Add(new KInstruction(graphic, kmesh, renderOrder));
                    }

                    InjectMesh(graphic, kmesh);
                    renderOrder++;
                }

                var transform = gameObject.transform;
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i).gameObject;
                    if (child.GetComponent<Canvas>() == null) // 忽略嵌套Canvas
                    {
                        renderOrder = GetRenderabledGraphics(child, canvas, renderOrder);
                    }
                }
                
                if (renderabled && mask != null) 
                {
                    // 在最后添加一个unmask instruction，并和mask共享kmesh
                    instructions.Add(new KInstruction(graphic, kmesh, renderOrder, mask, EMaskType.Unmask));
                    renderOrder++;
                }
            }

            return renderOrder;
        }

        // 是否会被渲染
        private bool IsRenderabledGraphic(MaskableGraphic graphic)
        {
            if (graphic == null)
            {
                return false;
            }

            var gameObject = graphic.gameObject;
            var transform = gameObject.GetComponent<RectTransform>();

            // 不可见
            if (!gameObject.activeInHierarchy)
            {
                return false;
            }
            // 未启用
            if (!graphic.enabled)
            {
                return false;
            }
            // 透明
            var alpha = GetLossyAlpha(graphic);
            if (Mathf.Approximately(alpha, 0))
            {
                return false;
            }
            // 面积小于等于0
            var rect = transform.rect;
            if (Mathf.Approximately(rect.width * rect.height, 0))
            {
                return false;
            }
            // X或Y的缩放值为0
            var scale = transform.lossyScale;
            if (Mathf.Approximately(Mathf.Abs(scale.x * scale.y), 0))
            {
                return false;
            }

            return true;
        }

        // 获取alpha值（含CanvasGroup的影响）
        // 在网上很多资料中，Graphic的color.a是否为0是会影响合批的, 但（在2020.3中）测试发现其实并没有影响，仅CanvasGroup的alpha值会影响合批
        private float GetLossyAlpha(MaskableGraphic graphic)
        {
            var alpha = 1.0f; //graphic.color.a对合批没有影响

            var transform = graphic.transform;
            while (true)
            {
                var canvas = transform.GetComponent<Canvas>();
                if (canvas != null && IsCanvasEnabled(canvas))
                {
                    var canvasGroup = transform.GetComponent<CanvasGroup>();
                    if (canvasGroup != null && canvasGroup.enabled)
                    {
                        alpha *= canvasGroup.alpha;
                    }

                    break;
                }
                
                transform = transform.parent;
                if (transform == null) break;
            }

            return alpha;
        }

        private void Analysis(Canvas canvas, List<KInstruction> instructions)
        {
            for (int i=0; i<instructions.Count-1; i++)
            {
                var a = instructions[i];
                for (int j=i+1; j<instructions.Count; j++)
                {
                    var b = instructions[j];
                    if (b.MeshOverlap(a))
                    {
                        var c = b.bottom;
                        if (c == null || c.renderOrder < a.renderOrder)
                        {
                            b.SetBottom(a);
                        }
                    }
                }
            }
            Sort(instructions);
            Batch(canvas, instructions);
        }

        // 排序
        private void Sort(List<KInstruction> instructions)
        {
            instructions.Sort((a, b) => {
                // 按depth升序
                if (a.depth < b.depth) return -1;
                if (a.depth > b.depth) return  1;
                // unmask排序 
                if (a.maskType == EMaskType.Unmask && 
                    b.maskType == EMaskType.Unmask)
                {
                    return Sort(b, a);
                }

                return Sort(a, b);
            });
        }

        private int Sort(KInstruction a, KInstruction b)
        {
            if (EditorApplication.isPlaying)
            {
                return SortInPlayMode(a, b);
            }

            return SortInEditorMode(a, b);
        }
        
        private int SortInEditorMode(KInstruction a, KInstruction b)
        {
            // 按材质ID升序
            var m1 = a.material;
            var m2 = b.material;
            if (m1.GetInstanceID() < m2.GetInstanceID()) return -1;
            if (m1.GetInstanceID() > m2.GetInstanceID()) return  1;
            
            if (a.spriteAtlas == null || b.spriteAtlas == null)
            {
                // 按纹理ID升序
                var t1 = a.texture;
                var t2 = b.texture;
                if (t1 != null && t2 != null)
                {
                    if (t1.GetInstanceID() < t2.GetInstanceID()) return -1;
                    if (t1.GetInstanceID() > t2.GetInstanceID()) return 1;
                }
            }
            else
            {
                // 按图集ID升序
                var s1 = a.spriteAtlas;
                var s2 = b.spriteAtlas;
                if (s1.GetInstanceID() < s2.GetInstanceID()) return -1;
                if (s1.GetInstanceID() > s2.GetInstanceID()) return 1;
            }
            
            // 按renderOrder升序
            return (a.renderOrder < b.renderOrder) ? -1 : 1;
        }
        
        private int SortInPlayMode(KInstruction a, KInstruction b)
        {
            // 按材质ID升序
            var m1 = a.material;
            var m2 = b.material;
            if (m1.GetInstanceID() < m2.GetInstanceID()) return -1;
            if (m1.GetInstanceID() > m2.GetInstanceID()) return  1;
            // 按纹理ID升序
            var t1 = a.texture;
            var t2 = b.texture;
            if (t1 != null && t2 != null)
            {
                if (t1.GetInstanceID() < t2.GetInstanceID()) return -1;
                if (t1.GetInstanceID() > t2.GetInstanceID()) return  1;
            }
            // 按renderOrder升序
            return (a.renderOrder < b.renderOrder) ? -1 : 1;
        }

        private void Batch(Canvas canvas, List<KInstruction> instructions)
        {
            foreach (var ins in instructions)
            {
                var batch = AllocBatch(canvas, ins);
                batch.Add(ins);
            }
        }

        private KBatch AllocBatch(Canvas canvas, KInstruction instruction)
        {
            if (batches.Count == 0)
            {
                return AllocBatch(canvas);
            }
            
            KBatch batch = batches[batches.Count - 1];
            if (!batch.Check(instruction))
            {
                batch = AllocBatch(canvas);
            }

            return batch;
        }

        private KBatch AllocBatch(Canvas canvas)
        {
            var batch = new KBatch(canvas);
            batches.Add(batch);

            return batch;
        }

        private RectMask2D GetRectMask2D(Transform transform)
        {
            var rectmask2d = transform.GetComponent<RectMask2D>();
            while (rectmask2d == null)
            {
                transform = transform.parent;
                if (transform == null || transform.GetComponent<Canvas>() != null)
                {
                    break;
                }
                
                rectmask2d = transform.GetComponent<RectMask2D>();
            }

            return rectmask2d;
        }

        private void RegisterGraphicDirtyHandlers(Graphic graphic)
        {
            graphic.RegisterDirtyLayoutCallback(OnGraphicLayoutDirtyHandler);
            graphic.RegisterDirtyVerticesCallback(OnGraphicVertexesDirtyHandler);
            graphic.RegisterDirtyMaterialCallback(OnGraphicMaterialDirtyHandler);
        }
        
        private void UnregisterGraphicDirtyHandlers(Graphic graphic)
        {
            graphic.UnregisterDirtyLayoutCallback(OnGraphicLayoutDirtyHandler);
            graphic.UnregisterDirtyVerticesCallback(OnGraphicVertexesDirtyHandler);
            graphic.UnregisterDirtyMaterialCallback(OnGraphicMaterialDirtyHandler);
        }
        
        private void OnGraphicLayoutDirtyHandler()
        {
            // Debug.Log("OnGraphicLayoutDirtyHandler");
            OnDirty?.Invoke();
        }

        private void OnGraphicVertexesDirtyHandler()
        {
            // Debug.Log("OnGraphicVertexesDirtyHandler");
            OnDirty?.Invoke();
        }

        private void OnGraphicMaterialDirtyHandler()
        {
            // Debug.Log("OnGraphicMaterialDirtyHandler");
            OnDirty?.Invoke();
        }
    }
}