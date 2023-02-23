using System;
using System.Collections.Generic;
using System.Reflection;
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
        
        public List<KBatch> batches { get; } = new List<KBatch>();
        public Action OnChanged;

        public void Analysis()
        {
            Dispose();

            var canvases = Transform.FindObjectsOfType<Canvas>(false);
            // 统计mesh数量
            foreach (var canvas in canvases)
            {
                if (IsCanvasEnabled(canvas))
                {
                    totalMeshCount += GetMeshCount(canvas);
                }
            }
            // 注入mesh
            foreach (var canvas in canvases)
            {
                if (IsCanvasEnabled(canvas))
                {
                    InjectMesh(canvas);
                }
            }
        }

        // Canvas是否可用
        private bool IsCanvasEnabled(Canvas canvas)
        {
            return canvas.gameObject.activeInHierarchy && canvas.enabled;
        }

        public void Dispose()
        {
            totalMeshCount = 0;
            injectMeshCount = 0;
            
            batches.Clear();
            KSpriteAtlas.Clear();

            var meshes = Transform.FindObjectsOfType<UIMesh>();
            foreach (var m in meshes)
            {
                GameObject.DestroyImmediate(m);
            }
            
            // OnChanged?.Invoke();
        }

        // 获取canvas下可渲染的mesh数量
        private int GetMeshCount(Canvas canvas)
        {
            var graphics = GetRenderabledGraphics(canvas);
            return graphics.Count;
        }

        // 暂时没有找到获取Mesh的方法，只能采用这种方案：
        // 在Canvas下的Graphic子节点中注入UIMesh组件计算Mesh，因为是异步计算的，所以处理起来稍显麻烦。
        private void InjectMesh(Canvas canvas)
        {
            var renderOrder = 0;

            var graphics = GetRenderabledGraphics(canvas);
            foreach (var graphic in graphics)
            {
                var mesh = graphic.gameObject.GetComponent<UIMesh>();
                if (mesh != null) // 如果不销毁，没有办法进行多次计算
                {
                    GameObject.DestroyImmediate(mesh);
                }
                mesh = graphic.gameObject.AddComponent<UIMesh>();

                renderOrder++;
                var instructions = AllocInstructions(canvas);
                
                var mask = graphic.GetComponent<Mask>();
                if (mask == null)
                {
                    instructions.Add(new KInstruction(graphic, graphic.materialForRendering, mesh.mesh, renderOrder, false, false));
                }
                else
                {
                    instructions.Add(new KInstruction(graphic, graphic.materialForRendering, mesh.mesh, renderOrder, true, false));
                    
                    var unmaskMaterial = GetUnmaskMaterial(mask);
                    instructions.Add(new KInstruction(graphic, unmaskMaterial, mesh.mesh, renderOrder, false, true));
                }

                // mesh计算是在子线程中进行的，所以这里将注入mesh数量和总mesh数量进行对比
                // 仅在最后一个mesh计算完成后才开始对所有canvas进行合批分析，避免线程的同步问题
                mesh.OnMeshChanged = () => {
                    injectMeshCount++;
                    
                    if (totalMeshCount == injectMeshCount)
                    {
                        foreach (var v in wcanvas)
                        {
                            Analysis(v.canvas, v.instructions);
                        }
                        OnChanged?.Invoke();
                    }
                };
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

        private Material GetUnmaskMaterial(Mask mask)
        {
            var mUnmaskMaterial = mask.GetType().GetField("m_UnmaskMaterial", BindingFlags.NonPublic | BindingFlags.Instance);
            return mUnmaskMaterial.GetValue(mask) as Material;
        }

        // 深度优先遍历所有可渲染的子节点
        private List<MaskableGraphic> GetRenderabledGraphics(Canvas canvas)
        {
            var graphics = new List<MaskableGraphic>();

            var transform = canvas.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                if (child.activeInHierarchy)
                {
                    GetRenderabledGraphics(child, graphics);
                }
            }

            return graphics;
        }

        // 获得渲染列表
        private void GetRenderabledGraphics(GameObject root, List<MaskableGraphic> list)
        {
            if (root.GetComponent<Canvas>() == null)
            {
                var graph = root.GetComponent<MaskableGraphic>();
                if (IsRenderabledGraphic(graph))
                {
                    list.Add(graph);
                }

                var transform = root.transform;
                for (int i=0; i<transform.childCount; i++)
                {
                    var child = transform.GetChild(i).gameObject;
                    if (child.activeInHierarchy)
                    {
                        GetRenderabledGraphics(child, list);
                    }
                }
            }
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
            if (alpha < float.Epsilon)
            {
                return false;
            }
            // 面积小于等于0
            var rect = transform.rect;
            if (rect.width * rect.height < float.Epsilon)
            {
                return false;
            }
            // X或Y的缩放值为0
            var scale = transform.lossyScale;
            if (Mathf.Abs(scale.x * scale.y) < float.Epsilon)
            {
                return false;
            }

            // TODO 其他判断条件，比如：UI不在一个平面上等

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
                if (canvas != null && canvas.enabled)
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
                            b.SetBottomWidget(a);
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
                if (a.isUnmask && b.isUnmask)
                {
                    return SortUnmask(a, b);
                }

                return Sort(a, b);
            });
        }

        private int Sort(KInstruction a, KInstruction b)
        {
            // 按depth升序
            if (a.depth < b.depth) return -1;
            if (a.depth > b.depth) return  1;
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

        private int SortUnmask(KInstruction a, KInstruction b)
        {
            // unmask
            if (!a.isUnmask &&  b.isUnmask) return -1;
            if ( a.isUnmask && !b.isUnmask) return  1;
            
            return Sort(b, a);
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
    }
}