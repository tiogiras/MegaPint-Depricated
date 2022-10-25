using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    public static class CustomGUIUtility {
        
        public static GUIStyle Headline(int size, Color color) {
            var style = Headline(size);
            style.normal.textColor = color;
            return style;
        }

        public static GUIStyle Background(float colorValue)
        {
            var style = new GUIStyle
            {
                normal = {background = MakeTex(600, 1, new Color(colorValue, colorValue, colorValue, 1.0f))}
            };
            return style;
        }
        
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width*height];
 
            for(int i = 0; i < pix.Length; i++)
                pix[i] = col;
 
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
 
            return result;
        }
        
        public static GUIStyle Headline(int size) {
            var style = new GUIStyle {
                alignment = TextAnchor.MiddleCenter,
                fontSize = size,
                fontStyle = FontStyle.Bold,
                normal = {textColor = Color.white}
            };
            return style;
        }

        public static GUIStyle CenteredText(int size, Color color) {
            var style = CenteredText(size);
            style.normal.textColor = color;
            return style;
        }
        
        public static GUIStyle CenteredText( int size ) {
            var style = new GUIStyle {
                alignment = TextAnchor.MiddleCenter,
                fontSize = size,
                normal = {textColor = new Color( .75f, .75f, .75f, 1 ) }
            };
            return style;
        }
        
        public static void GuiLine( int height ) {
             EditorGUILayout.Separator(  );
             var rect = EditorGUILayout.GetControlRect(false, height );
             rect.height = height;
             EditorGUI.DrawRect(rect, new Color ( 0.15f,0.15f,0.15f, 1 ) );
             EditorGUILayout.Separator(  );
         }
    }
}