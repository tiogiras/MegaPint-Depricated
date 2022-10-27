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
        private static MegaPintMenuEntry _activeMenuEntry;

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

        #region Generic

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
                if (GUILayout.Button(menuName, MegaPint.MegaPintGUI.GetStyle("menubutton"), GUILayout.ExpandWidth(true))) {
                    if (_activeMenu == this) {
                        if (_activeMenuEntry == null) _activeMenu = null;
                        else _activeMenuEntry = null;
                    }
                    else {
                        _activeMenu = this;
                        _activeMenuEntry = null;
                    }
                }

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
                if (GUILayout.Button(entryName, MegaPint.MegaPintGUI.GetStyle("menuentrybutton"), GUILayout.ExpandWidth(true))) {
                    _activeMenuEntry = this;
                }
            }
        }

        #endregion

        #region Custom

        public void DrawContent(int activeCategory) {

            if (_activeMenu == null) return;

            switch (activeCategory) {
                case 0: // Applications
                    switch (_activeMenu.menuName) {
                        
                    }
                    break;
                case 1: // Utility
                    switch (_activeMenu.menuName) {

                    }
                    break;
                case 2: // Settings
                    switch (_activeMenu.menuName) {
                        case "General":
                            if (_activeMenuEntry == null) {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("General", MegaPint.MegaPintGUI.GetStyle("header1"));
                                MegaPintGUIUtility.GuiLine(3);
                                return;
                            }

                            switch (_activeMenuEntry.entryName) {
                                case "Applications":
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.LabelField("Applications - Settings", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    break;
                                case "Utility":
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.LabelField("Utility - Settings", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    break;
                            }
                            break;
                        case "Contact":
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("Contact", MegaPint.MegaPintGUI.GetStyle("header1"));
                            MegaPintGUIUtility.GuiLine(3);
                            break;
                    }
                    break;
            }
        }

        #endregion
    }
}