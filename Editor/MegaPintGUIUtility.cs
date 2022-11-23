#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    public static class MegaPintGUIUtility {
        
        public static void GuiLine( int height ) {
             EditorGUILayout.Separator(  );
             var rect = EditorGUILayout.GetControlRect(false, height );
             rect.height = height;
             EditorGUI.DrawRect(rect, new Color ( 0.15f,0.15f,0.15f, 1 ) );
             EditorGUILayout.Separator(  );
        }
        
        public static void GuiLine( int height, float colorValue ) {
            EditorGUILayout.Separator(  );
            var rect = EditorGUILayout.GetControlRect(false, height );
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color ( colorValue,colorValue,colorValue, 1 ) );
            EditorGUILayout.Separator(  );
        }
    }
}

#endif