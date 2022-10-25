using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    public class MegaPintCategory {
        protected static bool DrawMenuEntry(string entryLabel)
        {
            return GUILayout.Button(entryLabel, GUILayout.ExpandWidth(true));
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
