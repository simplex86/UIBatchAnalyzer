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

        public static void BeginVerticalGroup()
        {
            GUILayout.Space(-2);
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            GUILayout.Space(2);
        }

        private static Color VER_GROUP_DEFAULT_COLOR = Color.white;
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

        public static void BeginIndent()
        {
            EditorGUI.indentLevel++;
        }

        public static void EndIndent()
        {
            EditorGUI.indentLevel--;
        }
    }
}