using UnityEngine;
using UnityEditor;

namespace SimpleX
{
    class UIBatchProfilerGUI
    {
        public static bool ToggleGroup(string title, bool expand)
        {
            return ToggleGroup(title, Color.white, expand);
        }

        public static bool ToggleGroup(string title, Color color, bool expand)
        {
            GUI.color = color;
            expand = GUILayout.Toggle(expand, "", EditorStyles.miniButton, GUILayout.Height(18));
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.Label(new Rect(rect.x + 5, rect.y + 0, rect.width, rect.height), title);
            GUI.color = Color.white;

            return expand;
        }

        public static void BeginIndent()
        {
            EditorGUI.indentLevel++;
        }

        public static void EndIndent()
        {
            EditorGUI.indentLevel--;
        }

        private const int s_panelHeaderHeight = -10;
        public static Rect BeginPanel(string title)
        {
            var rect = EditorGUILayout.BeginVertical(EditorStyles.textArea);

            EditorGUILayout.LabelField(title);
            EditorGUILayout.Space(-6);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.ExpandHeight(true), GUILayout.Height(3));
            EditorGUILayout.Space(9);

            return new Rect(rect.x, rect.y + s_panelHeaderHeight, rect.width, rect.height - s_panelHeaderHeight);
        }

        public static void EndPanel()
        {
            EditorGUILayout.EndVertical();
        }
    }
}