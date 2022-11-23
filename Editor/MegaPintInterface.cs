#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using MegaPint.Editor.Utility;
using UnityEditor;
using UnityEngine;

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
            public bool hasContent;
            public MegaPintFunctions.MegaPintFunction function;
            public List<MegaPintMenuEntry> menuEntries;
        
            public void Draw() {
                if (GUILayout.Button(menuName, MegaPint.MegaPintGUI.GetStyle("menubutton"), GUILayout.ExpandWidth(true))) {
                    if (function != MegaPintFunctions.MegaPintFunction.None) {
                        MegaPintFunctions.InvokeFunction(function);
                        return;
                    }

                    if (_activeMenu == this) {
                        if (hasContent) {
                            if (_activeMenuEntry == null) _activeMenu = null;
                            else _activeMenuEntry = null;
                        }
                        else {
                            _activeMenu = null;
                            _activeMenuEntry = null;
                        }
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
                        case "GeoNode":
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("GeoNode", MegaPint.MegaPintGUI.GetStyle("header1"));
                            MegaPintGUIUtility.GuiLine(3);
                            EditorGUILayout.Separator();
                            break;
                    }
                    break;
                case 1: // Utility
                    switch (_activeMenu.menuName) {
                        case "AutoSave - Scene":
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("AutoSave", MegaPint.MegaPintGUI.GetStyle("header1"));
                            MegaPintGUIUtility.GuiLine(3);
                            EditorGUILayout.Separator();
                            MegaPint.Settings.autoSaveIntervalTime = EditorGUILayout.IntField("Interval Time", MegaPint.Settings.autoSaveIntervalTime);
                            MegaPint.Settings.autoSaveMode = (MegaPintSettings.MegaPintAutoSaveMode)EditorGUILayout.EnumPopup("Save Mode", MegaPint.Settings.autoSaveMode);
                            if (MegaPint.Settings.autoSaveMode == MegaPintSettings.MegaPintAutoSaveMode.SaveAsDuplicate) {
                                EditorGUILayout.LabelField("Duplicate Folder", ".../" + MegaPint.Settings.autoSaveDuplicatePath);
                                if (MegaPint.Settings.autoSaveDuplicatePath.Equals("")) EditorGUILayout.HelpBox("No Folder Selected", MessageType.Error);
                                EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    if (GUILayout.Button("Change", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                        var path = EditorUtility.OpenFolderPanel("Select Folder for Duplicates", "Assets/", "");
                                        if (!path.Equals("")) {
                                            path = path.Replace(MegaPint.GetApplicationPath(), "");
                                            MegaPint.Settings.autoSaveDuplicatePath = path;
                                        }
                                    }
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.EndHorizontal();
                            }
                            MegaPint.Settings.autoSaveAudioWarning = EditorGUILayout.Toggle("Warning on exit", MegaPint.Settings.autoSaveAudioWarning);
                            MegaPint.Settings.autoSaveConsoleLog = EditorGUILayout.Toggle("Console Logging", MegaPint.Settings.autoSaveConsoleLog);
                            break;
                        case "Screenshot":
                            if (_activeMenuEntry == null) {
                                return;
                            }
                            
                            switch (_activeMenuEntry.entryName) {
                                case "Render Camera":
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.LabelField("Render Camera", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    EditorGUILayout.Separator();
                                    GUILayout.Box(MegaPintScreenshot.PreviewTexture, GUILayout.MaxHeight(300), GUILayout.MinHeight(300), GUILayout.ExpandWidth(true));
                                    EditorGUILayout.BeginHorizontal();
                                        MegaPintScreenshot.FileName = EditorGUILayout.TextField("File Name", MegaPintScreenshot.FileName);
                                        EditorGUILayout.LabelField(".png", GUILayout.MaxWidth(100));
                                        if (GUILayout.Button("Preview", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.MaxWidth(100))) {
                                            if (MegaPintScreenshot.RenderCamera == null) EditorApplication.Beep();
                                            else MegaPintScreenshot.RenderPreview();
                                        }
                                        if (GUILayout.Button("Export", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.MaxWidth(100))) {
                                            if (MegaPintScreenshot.RenderCamera != null) {
                                                if (MegaPintScreenshot.FileName == null) EditorApplication.Beep();
                                                else if (MegaPintScreenshot.FileName.Equals("")) EditorApplication.Beep();
                                                else if (MegaPint.Settings.screenshotSavePath.Equals("")) EditorApplication.Beep();
                                                else MegaPintScreenshot.RenderImage();
                                            }else EditorApplication.Beep();
                                        }
                                    EditorGUILayout.EndHorizontal();
                                    if (MegaPintScreenshot.FileName == null) EditorGUILayout.HelpBox("File Name Required", MessageType.Error);
                                    else if (MegaPintScreenshot.FileName.Equals("")) EditorGUILayout.HelpBox("File Name Required", MessageType.Error);
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    MegaPintScreenshot.RenderCamera = (Camera) EditorGUILayout.ObjectField("Rendering Camera", MegaPintScreenshot.RenderCamera, typeof(Camera), true);
                                    if (MegaPintScreenshot.RenderCamera == null) EditorGUILayout.HelpBox( "No Rendering Camera selected", MessageType.Error );
                                    MegaPintScreenshot.CurrentResolution = (MegaPintScreenshot.MegaPintScreenshotResolutions)EditorGUILayout.EnumPopup("Resolution", MegaPintScreenshot.CurrentResolution);
                                    if (MegaPintScreenshot.CurrentResolution == MegaPintScreenshot.MegaPintScreenshotResolutions.Custom) {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                        EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                        MegaPintScreenshot.CustomXRes = EditorGUILayout.IntField("Width", MegaPintScreenshot.CustomXRes);
                                        if (MegaPintScreenshot.CustomXRes <= 0) MegaPintScreenshot.CustomXRes = 1;
                                        EditorGUILayout.Separator();
                                        MegaPintScreenshot.CustomYRes = EditorGUILayout.IntField("Height", MegaPintScreenshot.CustomYRes);
                                        if (MegaPintScreenshot.CustomYRes <= 0) MegaPintScreenshot.CustomYRes = 1;
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    MegaPint.Settings.screenshotStrengthNormal = EditorGUILayout.Slider("Image Strength", MegaPint.Settings.screenshotStrengthNormal, .01f, 1);
                                    MegaPint.Settings.screenshotStrengthGlow = EditorGUILayout.Slider("Postprocessing Strength", MegaPint.Settings.screenshotStrengthGlow, 0, 1);
                                    EditorGUILayout.LabelField("Output Folder", ".../" + MegaPint.Settings.screenshotSavePath);
                                    if (MegaPint.Settings.screenshotSavePath.Equals("")) EditorGUILayout.HelpBox("No Folder Selected", MessageType.Error);
                                    EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                        if (GUILayout.Button("Change", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                            var path = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets/", "");
                                            if (!path.Equals("")) {
                                                path = path.Replace(MegaPint.GetApplicationPath(), "");
                                                MegaPint.Settings.screenshotSavePath = path;
                                            }
                                        }   
                                        EditorGUILayout.Separator(); EditorGUILayout.Separator(); 
                                        EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    EditorGUILayout.EndHorizontal();
                                    break;
                                case "Render Window": 
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.LabelField("Render Window", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    EditorGUILayout.Separator();
                                    GUILayout.Box(MegaPintScreenshot.WindowPreviewTexture, GUILayout.MaxHeight(300), GUILayout.MinHeight(300), GUILayout.ExpandWidth(true));
                                    EditorGUILayout.BeginHorizontal();
                                        MegaPintScreenshot.FileName = EditorGUILayout.TextField("File Name", MegaPintScreenshot.FileName);
                                        EditorGUILayout.LabelField(".png", GUILayout.MaxWidth(100));
                                        if (GUILayout.Button("Preview", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.MaxWidth(100))) MegaPintScreenshot.RenderWindowPreview();
                                        if (GUILayout.Button("Export", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.MaxWidth(100))) {
                                            if (MegaPintScreenshot.FileName == null) EditorApplication.Beep();
                                            else if (MegaPintScreenshot.FileName.Equals("")) EditorApplication.Beep();
                                            else if (MegaPint.Settings.screenshotSavePath.Equals("")) EditorApplication.Beep();
                                            else MegaPintScreenshot.RenderWindowImage();
                                        }
                                    EditorGUILayout.EndHorizontal();
                                    if (MegaPintScreenshot.FileName == null) EditorGUILayout.HelpBox("File Name Required", MessageType.Error);
                                    else if (MegaPintScreenshot.FileName.Equals("")) EditorGUILayout.HelpBox("File Name Required", MessageType.Error);
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    MegaPintScreenshot.CurrentWindow = (MegaPintScreenshot.MegaPintTargetWindows)EditorGUILayout.EnumPopup("Target", MegaPintScreenshot.CurrentWindow);
                                    if (MegaPintScreenshot.CurrentWindow == MegaPintScreenshot.MegaPintTargetWindows.WindowByName) {
                                        MegaPintScreenshot.WindowName = EditorGUILayout.TextField("Window Name", MegaPintScreenshot.WindowName);
                                        if (MegaPintScreenshot.WindowName == null) EditorGUILayout.HelpBox("Window Name Required", MessageType.Error);
                                        else if (MegaPintScreenshot.WindowName.Equals("")) EditorGUILayout.HelpBox("Window Name Required", MessageType.Error);
                                    }
                                    EditorGUILayout.LabelField("Output Folder", ".../" + MegaPint.Settings.screenshotSavePath);
                                    if (MegaPint.Settings.screenshotSavePath.Equals("")) EditorGUILayout.HelpBox("No Folder Selected", MessageType.Error);
                                    EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                        if (GUILayout.Button("Change", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                            var path = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets/", "");
                                            if (!path.Equals("")) {
                                                path = path.Replace(MegaPint.GetApplicationPath(), "");
                                                MegaPint.Settings.screenshotSavePath = path;
                                            }
                                        }   
                                        EditorGUILayout.Separator(); EditorGUILayout.Separator(); 
                                        EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    EditorGUILayout.EndHorizontal();
                                    break;
                            }
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
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    MegaPint.Settings.visibleGeoNode = EditorGUILayout.Toggle("GeoNode",MegaPint.Settings.visibleGeoNode, MegaPint.MegaPintGUI.toggle);

                                    break;
                                case "Utility":
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.LabelField("Utility - Settings", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.LabelField("Choosen utilities are displayed in the utility-menu.", MegaPint.MegaPintGUI.GetStyle("centertext"));
                                    EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                    MegaPint.Settings.visibleAutoSave = EditorGUILayout.Toggle("AutoSave - Scene",MegaPint.Settings.visibleAutoSave, MegaPint.MegaPintGUI.toggle);
                                    MegaPint.Settings.visibleScreenshot = EditorGUILayout.Toggle("Screenshot",MegaPint.Settings.visibleScreenshot, MegaPint.MegaPintGUI.toggle);
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
                        case "GeoNode": return MegaPint.Settings.visibleGeoNode;
                    }
                    break;
                case 1: // Utility
                    switch (menuName) {
                        case "AutoSave - Scene": return MegaPint.Settings.visibleAutoSave;
                        case "Screenshot": return MegaPint.Settings.visibleScreenshot;
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

#endif