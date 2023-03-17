using UnityEditor.IMGUI.Controls;

namespace SimpleX
{
    public class SimpleTreeViewItem : TreeViewItem
    {
        private static int s_id = 0;

        public SimpleTreeViewItem(string displayName)
            : base(s_id, 0, displayName)
        {
            s_id++;
        }

        public SimpleTreeViewItem this[int index] => children[index] as SimpleTreeViewItem;
        
        public object userData { get; set; } = null;

        public void Clear()
        {
            if (children == null) return;
            
            for (int i = 0; i < children.Count; i++)
            {
                this[i].Clear();
            }
            children.Clear();
        }
    }
}