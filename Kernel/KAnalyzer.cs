using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    public class KAnalyzer
    {
        public Action<List<KBatch>> OnCompleted;

        private List<KBatch> batches = new List<KBatch>();

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
            var meshes = Transform.FindObjectsOfType<UIMesh>();
            foreach (var m in meshes)
            {
                GameObject.DestroyImmediate(m);
            }
        }

        private void InjectCanvas(Canvas canvas)
        {
            var widgets = new List<KWidget>();
            var meshCounter = 0;
            var hierarchyIndex = 0;

            var graphics = GetActivedGraphic(canvas.gameObject);
            foreach (var graphic in graphics)
            {
                var mesh = graphic.gameObject.GetComponent<UIMesh>();
                if (mesh != null)
                {
                    GameObject.DestroyImmediate(mesh);
                }
                mesh = graphic.gameObject.AddComponent<UIMesh>();

                hierarchyIndex++;
                widgets.Add(new KWidget(graphic, mesh.mesh, hierarchyIndex));

                mesh.OnMeshChanged = (ui) => {
                    meshCounter++;
                    if (widgets.Count == meshCounter)
                    {
                        Analysis(widgets);
                    }
                };

                // var mm = GetMesh(graphic);
                // if (mm != null)
                // {
                //     KMesh mesh = new KMesh(graphic.transform);
                //     mesh.Fill(mm);

                //     hierarchyIndex++;
                //     widgets.Add(new KWidget(graphic, mesh, hierarchyIndex));
                // }
            }
        }

        private Mesh GetMesh(Graphic graphic)
        {
            try
            {
                Type type = GetTypeByName<Graphic>("UnityEngine.UI.Graphic");
                if (type != null)
                {
                    var field = type.GetField("m_CachedMesh", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        return field.GetValue(graphic) as Mesh;
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

        // 深度优先遍历所有可见的子节点
        private List<Graphic> GetActivedGraphic(GameObject root)
        {
            var graphics = new List<Graphic>();

            var children = root.GetComponentsInChildren<Graphic>(true);// TODO 不确定该函数是不是深度优先的，可能后面需要改
            foreach (var c in children)
            {
                if (c.gameObject.activeInHierarchy)
                {
                    graphics.Add(c);
                }
            }

            return graphics;
        }

        private void Analysis(List<KWidget> widgets)
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
            Batch(widgets);

            OnCompleted(batches);
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

            foreach (var w in widgets)
            {
                Debug.Log($"{w.name}:{w.depth}");
            }
        }

        private void Batch(List<KWidget> widgets)
        {
            foreach (var w in widgets)
            {
                if (batches.Count == 0)
                {
                    var b = new KBatch(w.depth);
                    batches.Add(b);
                }
                
                var batch = batches[batches.Count - 1];
                if (batch.depth != w.depth || !batch.Check(w))
                {
                    batch = new KBatch(w.depth);
                    batches.Add(batch);
                }
                batch.Add(w);
            }
        }
    }
}