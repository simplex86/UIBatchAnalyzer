using UnityEngine;
using UnityEditor;

namespace XH
{
    class UIBatchAnalyzerGUI
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
            GUI.Label(new Rect(rect.x + 2, rect.y + 0, rect.width, rect.height), title);
            GUI.color = Color.white;

            return expand;
        }

        public static void BeginVerticalGroup()
        {
            GUILayout.Space(-2);
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            GUILayout.Space(2);
        }

        private static Color VER_GROUP_DEFAULT_COLOR = new Color(0.8f, 0.4f, 0.2f);
        public static void BeginVerticalGroup(string title)
        {
            BeginVerticalGroup(title, VER_GROUP_DEFAULT_COLOR);
        }

        public static void BeginVerticalGroup(string title, Color color)
        {
            GUI.color = color;
            GUILayout.Label(title, EditorStyles.helpBox);
            GUI.color = Color.white;

            GUILayout.Space(-2);
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            GUILayout.Space(2);
        }

        public static void EndVerticalGroup()
        {
            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }
    }
}