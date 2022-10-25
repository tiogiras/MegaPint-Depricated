using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor {
    public class MegaPintCategory {
        protected static void DrawMenuEntry(string entryLabel)
        {

        }

        protected static bool DrawMenuElementEntry(string entryLabel) {
            var value = false;
            EditorGUILayout.BeginVertical(CustomGUIUtility.Background(.25f), GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Separator();
                value = GUILayout.Button(entryLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();
            return value;
        }
    }
}
