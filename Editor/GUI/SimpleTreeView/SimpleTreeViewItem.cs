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
        
        public object what { get; set; } = null;
    }
}