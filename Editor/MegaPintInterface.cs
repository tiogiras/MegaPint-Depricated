using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace MegaPint.Editor {
    
    public class MegaPintInterface : ScriptableObject {

        public List<MegaPintCategory> categories;

        [Serializable]
        public class MegaPintCategory {
            public string categoryName;
            public List<MegaPintMenu> menus;
            
            public List<bool> DrawMenu(int expandedMenu) {
                var index = 0;
                var arr = new List<bool>();
                foreach (var menu in menus) {
                    arr = menu.DrawMenu(index == expandedMenu);
                    index++;
                }
                return arr;
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
                arr.AddRange(menuEntries.Select(entry => GUILayout.Button(entry.displayName, 
                    MegaPint.MegaPintGUI.GetStyle("menuentrybutton"), GUILayout.ExpandWidth(true))));
                EditorGUILayout.Separator();
                return arr;
            }
        }
        
        [Serializable] public class MegaPintMenuEntry {
            public string displayName;
        }
    }
}