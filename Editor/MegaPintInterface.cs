using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    
    public class MegaPintInterface : ScriptableObject {

        public List<MegaPintCategory> categories;

        public static List<bool> MenuStatus;

        public void DrawCategory(int activeCategory) {
            categories[activeCategory].DrawMenu();
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
            
            public void DrawMenu() {
                var index = 0;
                foreach (var menu in menus) {
                    MenuStatus = menu.DrawMenu(MenuStatus != null && MenuStatus[index]);
                    index++;
                }
            }
        }
        
        [Serializable] public class MegaPintMenu {
            public string menuName;
            public List<MegaPintMenuEntry> menuEntries;
        
            public List<bool> DrawMenu(bool expanded) {
                var arr = new List<bool> {
                    GUILayout.Button(menuName, MegaPint.MegaPintGUI.GetStyle("menubutton"), GUILayout.ExpandWidth(true))
                };

                if (!expanded) return arr;
                foreach (var entry in menuEntries)
                {
                    GUILayout.Button(entry.displayName, MegaPint.MegaPintGUI.GetStyle("menuentrybutton"), GUILayout.ExpandWidth(true));
                }
                EditorGUILayout.Separator();
                return arr;
            }
        }
        
        [Serializable] public class MegaPintMenuEntry {
            public string displayName;
        }
    }
}