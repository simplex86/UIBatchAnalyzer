using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace SimpleX
{
    public class SimpleTreeView : TreeView
    {
        private SimpleTreeViewItem root = null;
        
        public SimpleTreeView()
            : base(new TreeViewState())
        {
            root = new SimpleTreeViewItem("Root")
            {
                depth = -1
            };
        }

        public System.Action<object> onSelectionChanged;

        public void AddChild(SimpleTreeViewItem child)
        {
            root.AddChild(child);
        }

        public void Clear()
        {
            if (root.children != null)
            {
                for (int i = 0; i < root.children.Count; i++)
                {
                    root[i].Clear();
                }
                root.children.Clear();
                
                Reload();
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count == 0)
            {
                Debug.Log($"select nothing");
            }
            else
            {
                var item = GetItem(selectedIds[0]);
                onSelectionChanged?.Invoke(item.what);
            }
        }

        private SimpleTreeViewItem GetItem(int id)
        {
            if (root.children == null)
            {
                return null;
            }
            
            foreach (var child in root.children)
            {
                var item = GetItem(child, id);
                if (item != null)
                {
                    return item as SimpleTreeViewItem;
                }
            }

            return null;
        }
        
        private TreeViewItem GetItem(TreeViewItem item, int id)
        {
            if (item.id == id)
            {
                return item;
            }

            if (item.children == null)
            {
                return null;
            }

            foreach (var child in item.children)
            {
                var find = GetItem(child, id);
                if (find != null) return find;
            }

            return null;
        }
    }
}