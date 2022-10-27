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

        public void DrawCategory(int activeCategory) => categories[activeCategory].Draw(activeCategory);
        
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
            
            public void Draw(int currentCategoryIndex) {
                foreach (var menu in menus) {
                    if (GetMenuVisibility(currentCategoryIndex, menu.menuName)) menu.Draw();
                }
            }
        }
        
        [Serializable] public class MegaPintMenu {
            public string menuName;
            public MegaPintFunctions.MegaPintFunction function;
            public List<MegaPintMenuEntry> menuEntries;
        
            public void Draw() {
                if (GUILayout.Button(menuName, MegaPint.MegaPintGUI.GetStyle("menubutton"), GUILayout.ExpandWidth(true))) {
                    if (function != MegaPintFunctions.MegaPintFunction.None) {
                        MegaPintFunctions.InvokeFunction(function);
                        return;
                    }
                    
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
            public MegaPintFunctions.MegaPintFunction function;
            
            public void Draw() {
                if (GUILayout.Button(entryName, MegaPint.MegaPintGUI.GetStyle("menuentrybutton"), GUILayout.ExpandWidth(true))) {
                    if (function != MegaPintFunctions.MegaPintFunction.None) {
                        MegaPintFunctions.InvokeFunction(function);
                        return;
                    }
                    
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
                        case "AutoSave":
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("AutoSave", MegaPint.MegaPintGUI.GetStyle("header1"));
                            MegaPintGUIUtility.GuiLine(3);
                            EditorGUILayout.Separator();
                            MegaPint.Settings.autoSaveIntervalTime = EditorGUILayout.IntField("Interval Time", MegaPint.Settings.autoSaveIntervalTime);
                            MegaPint.Settings.autoSaveMode = (MegaPintSettings.MegaPintAutoSaveMode)EditorGUILayout.EnumPopup("Save Mode", MegaPint.Settings.autoSaveMode);
                            if (MegaPint.Settings.autoSaveMode == MegaPintSettings.MegaPintAutoSaveMode.SaveAsDuplicate) {
                                EditorGUILayout.LabelField("Duplicate Folder", MegaPint.Settings.autoSaveDuplicatePath);
                                EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    if (GUILayout.Button("Change")) {
                                        MegaPint.Settings.autoSaveDuplicatePath = EditorUtility.OpenFolderPanel("Select Folder for Duplicates", "Assets/", "");
                                    }
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator(); 
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.EndHorizontal();
                            }
                            MegaPint.Settings.autoSaveAudioWarning = EditorGUILayout.Toggle("Warning on exit", MegaPint.Settings.autoSaveAudioWarning);
                            break;
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
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.LabelField("Choosen applications are displayed in the applications-menu.", MegaPint.MegaPintGUI.GetStyle("centertext"));
                                    break;
                                case "Utility":
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.LabelField("Utility - Settings", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.LabelField("Choosen utilities are displayed in the utility-menu.", MegaPint.MegaPintGUI.GetStyle("centertext"));
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    MegaPint.Settings.visibleAutoSave = EditorGUILayout.Toggle("Scene AutoSave",MegaPint.Settings.visibleAutoSave, MegaPint.MegaPintGUI.toggle);
                                    break;
                            }
                            break;
                        case "Contact":
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("Contact", MegaPint.MegaPintGUI.GetStyle("header1"));
                            MegaPintGUIUtility.GuiLine(3);
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("This is a work-in-progress project and will be updated regulary.", MegaPint.MegaPintGUI.GetStyle("centertext"));
                            EditorGUILayout.LabelField("Currently developed by Niklas Räder (Tiogiras).", MegaPint.MegaPintGUI.GetStyle("centertext"));
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("You can contact me on any platform listed below.", MegaPint.MegaPintGUI.GetStyle("centertext"));
                            EditorGUILayout.LabelField("Discord: Tiogiras#0666", MegaPint.MegaPintGUI.GetStyle("centertext"));
                            EditorGUILayout.LabelField("Mail: tiogiras@gmail.com", MegaPint.MegaPintGUI.GetStyle("centertext"));
                            break;
                    }
                    break;
            }
        }

        #endregion

        private static bool GetMenuVisibility(int currentCategory, string menuName) {
            switch (currentCategory) {
                case 0: // Applications
                    switch (menuName) {
                        
                    }
                    break;
                case 1: // Utility
                    switch (menuName) {
                        case "AutoSave": return MegaPint.Settings.visibleAutoSave;
                    }
                    break;
                case 2: // Settings
                    switch (menuName) {
                        case "General": return true;
                        case "Contact": return true;
                    }
                    break;
            }

            return false;
        }
    }
}