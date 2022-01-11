using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    public class KAnalyzer
    {
        public Action OnChanged;
        public List<KBatch> batches = new List<KBatch>();

        public void Analysis()
        {
            batches.Clear();

            var canvases = Transform.FindObjectsOfType<Canvas>();
            foreach (var canvas in canvases)
            {
                InjectCanvas(canvas);
            }
        }

        public void Dispose()
        {
            batches.Clear();

            var meshes = Transform.FindObjectsOfType<UIMesh>();
            foreach (var m in meshes)
            {
                GameObject.DestroyImmediate(m);
            }

            if (OnChanged != null)
            {
                OnChanged();
            }
        }

        // 暂时没有找到获取Mesh的方法，只能采用这种方案：
        // 在Canvas下的Graphic子节点中注入UIMesh组件计算Mesh，因为是异步计算的，所以处理起来稍显麻烦。
        private void InjectCanvas(Canvas canvas)
        {
            var widgets = new List<KWidget>();
            var meshCounter = 0;
            var hierarchyIndex = 0;

            var graphics = GetRenderabledGraphics(canvas);
            foreach (var graphic in graphics)
            {
                var mesh = graphic.gameObject.GetComponent<UIMesh>();
                if (mesh != null) // 如果不销毁，没有办法进行多次计算
                {
                    GameObject.DestroyImmediate(mesh);
                }
                mesh = graphic.gameObject.AddComponent<UIMesh>();

                hierarchyIndex++;
                widgets.Add(new KWidget(graphic, mesh.mesh, hierarchyIndex));

                mesh.OnMeshChanged = () => {
                    meshCounter++;
                    if (widgets.Count == meshCounter)
                    {
                        Analysis(canvas, widgets);
                    }
                };
            }
        }

        private Mesh GetMesh(MaskableGraphic graphic)
        {
            try
            {
                Type type = GetTypeByName<MaskableGraphic>("UnityEngine.UI.MaskableGraphic");
                if (type != null)
                {
                    var field = type.GetField("m_CachedMesh", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        var mesh = field.GetValue(graphic) as Mesh;
                        if (mesh != null) Debug.Log("ddddddddddd");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            return null;
        }

        private Type GetTypeByName<T>(string typename)
        {
            Type type = typeof(T);
            Assembly assembly = Assembly.GetAssembly(type);
            
            Type[] types = assembly.GetTypes();
            foreach (var t in types)
            {
                if (t.ToString() == typename) return t;
            }
            
            return null;
        }

        // 深度优先遍历所有可渲染的子节点
        private List<MaskableGraphic> GetRenderabledGraphics(Canvas canvas)
        {
            var graphics = new List<MaskableGraphic>();
            
            if (canvas.gameObject.activeInHierarchy)
            {
                var transform = canvas.transform;
                for (int i=0; i<transform.childCount; i++)
                {
                    var child = transform.GetChild(i).gameObject;
                    if (child.activeInHierarchy)
                    {
                        GetRenderabledGraphics(child, graphics);
                    }
                }
            }

            return graphics;
        }

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
                        graph = child.GetComponent<MaskableGraphic>();
                        if (IsRenderabledGraphic(graph))
                        {
                            list.Add(graph);
                        }
                        GetRenderabledGraphics(child, list);
                    }
                }
            }
        }

        private bool IsRenderabledGraphic(MaskableGraphic graphic)
        {
            if (graphic == null)
            {
                return false;
            }
            // 不可见
            if (!graphic.gameObject.activeInHierarchy)
            {
                return false;
            }
            // 透明
            if (graphic.color.a < float.Epsilon)
            {
                return false;
            }

            // TODO 其他判断条件，比如：缩放值是否为0，mask的影响等等

            return true;
        }

        private void Analysis(Canvas canvas, List<KWidget> widgets)
        {
            for (int i=0; i<widgets.Count-1; i++)
            {
                var a = widgets[i];
                for (int j=i+1; j<widgets.Count; j++)
                {
                    var b = widgets[j];
                    if (b.MeshOverlap(a))
                    {
                        var c = b.bottom;
                        if (c == null || c.hierarchyIndex < a.hierarchyIndex)
                        {
                            b.SetBottomWidget(a);
                        }
                    }
                }
            }
            Sort(widgets);
            Batch(canvas, widgets);

            if (OnChanged != null)
            {
                OnChanged();
            }
        }

        private void Sort(List<KWidget> widgets)
        {
            widgets.Sort((a, b) => {
                // 按depth升序
                if (a.depth < b.depth) return -1;
                if (a.depth > b.depth) return 1;
                // 按材质ID升序
                var m1 = a.material;
                var m2 = b.material;
                if (m1.GetInstanceID() < m2.GetInstanceID()) return -1;
                if (m1.GetInstanceID() > m2.GetInstanceID()) return 1;
                // 按纹理ID降序
                var t1 = a.texture;
                var t2 = b.texture;
                if (t1 != null && t2 != null)
                {
                    if (t1.GetInstanceID() < t2.GetInstanceID()) return 1;
                    if (t1.GetInstanceID() > t2.GetInstanceID()) return -1;
                }
                // 按hierarchyIndex降序
                return (a.hierarchyIndex < b.hierarchyIndex) ? 1 : -1;
            });
        }

        private void Batch(Canvas canvas, List<KWidget> widgets)
        {
            foreach (var w in widgets)
            {
                var batch = AllocBatch(canvas, w);
                batch.Add(w);
            }
        }

        private KBatch AllocBatch(Canvas canvas, KWidget widget)
        {
            KBatch batch = (batches.Count == 0) ? AllocBatch(canvas, widget.depth) : batches[batches.Count - 1];
            if (batch.depth != widget.depth || !batch.Check(widget))
            {
                batch = AllocBatch(canvas, widget.depth);
            }

            return batch;
        }

        private KBatch AllocBatch(Canvas canvas, int depth)
        {
            var batch = new KBatch(canvas, depth);
            batches.Add(batch);

            return batch;
        }
    }
}