using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace MegaPint.Editor {
    
    public class MegaPintInterface : ScriptableObject {

        public List<MegaPintCategory> categories;

        private static MegaPintMenu _activeMenu;

        public void DrawCategory(int activeCategory) {
            categories[activeCategory].Draw();
        }
        
        public string[] GetCategoryNames() {
            var arr = new string[categories.Count];
            for (var i = 0; i < categories.Count; i++) {
                arr[i] = categories[i].categoryName;
            }
            return arr;
        }
        
        [Serializable]
        public class MegaPintCategory {
            public string categoryName;
            public List<MegaPintMenu> menus;
            
            public void Draw() {
                foreach (var menu in menus) {
                    menu.Draw();
                }
            }
        }
        
        [Serializable] public class MegaPintMenu {
            public string menuName;
            public List<MegaPintMenuEntry> menuEntries;
        
            public void Draw() {
                if (GUILayout.Button(menuName, MegaPint.MegaPintGUI.GetStyle("menubutton"), GUILayout.ExpandWidth(true))) _activeMenu = this;

                if (_activeMenu != this) return;
                
                foreach (var entry in menuEntries) {
                    entry.Draw();
                }
                EditorGUILayout.Separator();
            }
        }
        
        [Serializable] public class MegaPintMenuEntry {
            public string entryName;

            public void Draw() {
                GUILayout.Button(entryName, MegaPint.MegaPintGUI.GetStyle("menuentrybutton"), GUILayout.ExpandWidth(true));
            }
        }
    }
}