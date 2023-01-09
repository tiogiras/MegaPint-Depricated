#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using MegaPint.Editor.Utility;
using MegaPint.Editor.Utility.MaterialSets;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    
    public class MegaPintInterface : ScriptableObject {

        public List<MegaPintCategory> categories;

        private static MegaPintMenu ActiveMenu;
        private static MegaPintMenuEntry ActiveMenuEntry;

        private Vector2 _materialSetsScrollPos;
        private Vector2 _materialSetsScrollPos1;
        private List<GameObject> _materialSetsSelection;

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

                    if (ActiveMenu == this) {
                        if (hasContent) {
                            if (ActiveMenuEntry == null) ActiveMenu = null;
                            else ActiveMenuEntry = null;
                        }
                        else {
                            ActiveMenu = null;
                            ActiveMenuEntry = null;
                        }
                    }
                    else {
                        ActiveMenu = this;
                        ActiveMenuEntry = null;
                    }
                }

                if (ActiveMenu != this) return;
                
                foreach (var entry in menuEntries) {
                    entry.Draw();
                }
                EditorGUILayout.Separator();
            }
        }
        
        [Serializable] public class MegaPintMenuEntry {
            public string entryName;
            public bool hasContent;
            public MegaPintFunctions.MegaPintFunction function;
            
            public void Draw() {
                if (GUILayout.Button(entryName, MegaPint.MegaPintGUI.GetStyle("menuentrybutton"), GUILayout.ExpandWidth(true))) {
                    if (function != MegaPintFunctions.MegaPintFunction.None) {
                        MegaPintFunctions.InvokeFunction(function);
                        if (!hasContent) return;
                    }
                    
                    ActiveMenuEntry = this;
                }
            }
        }

        #endregion

        #region Custom

        public void DrawContent(int activeCategory) {

            if (ActiveMenu == null) return;

            switch (activeCategory) {
                case 0: // Applications
                    switch (ActiveMenu.menuName) {
                        case "GeoNode":
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("GeoNode", MegaPint.MegaPintGUI.GetStyle("header1"));
                            MegaPintGUIUtility.GuiLine(3);
                            EditorGUILayout.Separator();
                            break;
                    }
                    break;
                case 1: // Utility
                    switch (ActiveMenu.menuName) {
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
                            if (ActiveMenuEntry == null) {
                                return;
                            }
                            
                            switch (ActiveMenuEntry.entryName) {
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
                        case "Material Sets":
                            if (ActiveMenuEntry == null) {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("Material Sets", MegaPint.MegaPintGUI.GetStyle("header1"));
                                MegaPintGUIUtility.GuiLine(3);
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("With this tool you create material sets", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("add them to objects and call them to change", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("from a central overlay.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("The edit tab let's you create new", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("and edit existing material sets.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("In the apply tab you add the sets", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("to selected gameObjects in your scene.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("By calling apply on a set will result in", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("all objects changing, that are selected", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("and contain the set.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                
                                EditorGUILayout.LabelField("MaterialSet Folder", ".../" + MegaPint.Settings.materialSetsSavePath);
                                if (MegaPint.Settings.materialSetsSavePath.Equals("")) EditorGUILayout.HelpBox("No Folder Selected", MessageType.Error);
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                if (GUILayout.Button("Change", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                    var path = EditorUtility.OpenFolderPanel("Select MaterialSet Folder", "Assets/", "");
                                    if (!path.Equals("")) {
                                        path = path.Replace(MegaPint.GetApplicationPath(), "");
                                        MegaPint.Settings.materialSetsSavePath = path;
                                    }
                                }   
                                EditorGUILayout.Separator(); EditorGUILayout.Separator(); 
                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                EditorGUILayout.EndHorizontal();
                                return;
                            }

                            switch (ActiveMenuEntry.entryName) {
                                case "Edit":
                                    if (MegaPint.Settings.materialSetsSavePath.Equals("")) {
                                        EditorGUILayout.HelpBox("No MaterialSet folder selected", MessageType.Error);
                                    }
                                    else {
                                        EditorGUILayout.Separator();
                                        EditorGUILayout.LabelField("Material Sets", MegaPint.MegaPintGUI.GetStyle("header1"));
                                        MegaPintGUIUtility.GuiLine(3);
                                        EditorGUILayout.Separator();
                                    
                                        _materialSetsScrollPos = EditorGUILayout.BeginScrollView(_materialSetsScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                                        if (MegaPint.Settings.materialSets.Count > 0) {
                                            for (var i = 0; i < MegaPint.Settings.materialSets.Count; i++) {
                                                EditorGUILayout.BeginVertical(MegaPint.MegaPintGUI.GetStyle(i % 2 == 0 ? "bg3" : "bg2"), GUILayout.ExpandWidth(true));
                                            
                                                EditorGUILayout.Separator();
                                                EditorGUILayout.BeginHorizontal();
                                                MegaPint.Settings.materialSetsFoldouts[i] =
                                                    EditorGUILayout.Foldout(MegaPint.Settings.materialSetsFoldouts[i], MegaPint.Settings.materialSets[i].materialSetName);
                                                    if (GUILayout.Button("Remove", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.Width(75), GUILayout.Height(15))) {
                                                        AssetDatabase.DeleteAsset(MegaPint.Settings.materialSets[i].assetPath);
                                                        AssetDatabase.Refresh();
                                                    
                                                        MegaPintMaterialSets.UpdateMaterialSetsTemp();
                                                    }
                                                EditorGUILayout.EndHorizontal();

                                                if (i < MegaPint.Settings.materialSetsFoldouts.Count) {
                                                    if (!MegaPint.Settings.materialSetsFoldouts[i]) {
                                                        EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                                        EditorGUILayout.EndVertical();
                                                        continue;
                                                    }
                                                }

                                                var indent = EditorGUI.indentLevel;
                                                EditorGUI.indentLevel++;
                                            
                                                EditorGUILayout.Separator();

                                                if (i >= MegaPint.Settings.materialSets.Count) {
                                                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                                    EditorGUILayout.EndVertical();
                                                    continue;
                                                }
                                                var set = MegaPint.Settings.materialSets[i];
                                            
                                                MegaPintGUIUtility.GuiLine(1, i % 2 == 0 ? 0.15f : 0.3f);
                                                for (var j = 0; j < set.materials.Count; j++) {
                                                    EditorGUILayout.BeginHorizontal();
                                                    set.materials[j] = (Material)EditorGUILayout.ObjectField("Slot " + (j + 1), set.materials[j], typeof(Material), false);
                                                    if (GUILayout.Button("Remove", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.Width(75), GUILayout.Height(15))) {
                                                        set.materials.RemoveAt(j);
                                                        if (set.materials.Count == 0) {
                                                            AssetDatabase.DeleteAsset(set.assetPath);
                                                            AssetDatabase.Refresh();
                                                        
                                                            MegaPintMaterialSets.UpdateMaterialSetsTemp();
                                                        }
                                                    }
                                                    EditorGUILayout.EndHorizontal();
                                                }

                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                                if (GUILayout.Button("Add Material Slot", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.Height(20), GUILayout.Width(125))) {
                                                    set.materials.Add(null);
                                                }
                                                EditorGUILayout.EndHorizontal();
                                            
                                                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                                                EditorGUILayout.EndVertical();
                                                EditorGUI.indentLevel = indent;
                                            }
                                        }
                                        EditorGUILayout.EndScrollView();

                                        if (GUILayout.Button("Create new Material Set", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                            MegaPintCreateMaterialSet.Init();
                                        } 
                                    }
                                    
                                    
                                    break;
                                case "Apply":
                                    if (MegaPint.Settings.materialSetsSavePath.Equals("")) {
                                        EditorGUILayout.HelpBox("No MaterialSet folder selected", MessageType.Error);
                                    }
                                    else {
                                        EditorGUILayout.Separator();
                                        EditorGUILayout.LabelField("Material Sets",
                                            MegaPint.MegaPintGUI.GetStyle("header1"));
                                        MegaPintGUIUtility.GuiLine(3);
                                        EditorGUILayout.Separator();

                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.LabelField("Selection:",
                                            "Objects need any rendering component.");
                                        EditorGUILayout.LabelField("[Add set]", "Adds the set to the object.");
                                        EditorGUILayout.LabelField("[Apply]", "Apply set as the current materials.");
                                        EditorGUI.indentLevel--;

                                        EditorGUILayout.Separator();
                                        MegaPintGUIUtility.GuiLine(1);
                                        EditorGUILayout.Separator();

                                        EditorGUILayout.BeginHorizontal();
                                        if (GUILayout.Button("Update", MegaPint.MegaPintGUI.GetStyle("button1"),
                                                GUILayout.Height(15), GUILayout.Width(75))) {
                                            _materialSetsSelection = new List<GameObject>();
                                            foreach (var o in Selection.gameObjects) {
                                                if (o.GetComponent<MeshRenderer>() != null) {
                                                    _materialSetsSelection.Add(o);
                                                    continue;
                                                }

                                                if (o.GetComponent<SkinnedMeshRenderer>() != null) {
                                                    _materialSetsSelection.Add(o);
                                                }
                                            }
                                        }

                                        EditorGUILayout.LabelField("Selected Renderers:",
                                            _materialSetsSelection.Count + "");
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.Separator();
                                        if (_materialSetsSelection.Count <= 0)
                                            EditorGUILayout.HelpBox("Nothing selected!", MessageType.Warning);
                                        else {
                                            _materialSetsScrollPos1 =
                                                EditorGUILayout.BeginScrollView(_materialSetsScrollPos1, GUIStyle.none,
                                                    GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true),
                                                    GUILayout.ExpandHeight(true));

                                            EditorGUILayout.BeginHorizontal();
                                            var count = 0;
                                            for (int i = 0; i < MegaPint.Settings.materialSets.Count; i++) {
                                                EditorGUILayout.BeginVertical(
                                                    MegaPint.MegaPintGUI.GetStyle(i % 2 == 0 ? "bg2" : "bg3"));
                                                EditorGUILayout.LabelField(MegaPint.Settings.materialSets[i]
                                                    .materialSetName);
                                                EditorGUILayout.BeginHorizontal();
                                                if (GUILayout.Button("Add set",
                                                        MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.Height(20),
                                                        GUILayout.Width(65))) {
                                                    var newComps = 0;
                                                    var existComps = 0;
                                                    foreach (var o in _materialSetsSelection) {
                                                        MegaPintMaterialSlots comp;
                                                        if (o.GetComponent<MegaPintMaterialSlots>() == null) {
                                                            comp = o.AddComponent<MegaPintMaterialSlots>();
                                                            newComps++;
                                                        }
                                                        else {
                                                            comp = o.GetComponent<MegaPintMaterialSlots>();
                                                            existComps++;
                                                        }

                                                        if (comp == null) continue;
                                                        if (!comp.materialSets.Contains(
                                                                MegaPint.Settings.materialSets[i]))
                                                            comp.materialSets.Add(MegaPint.Settings.materialSets[i]);
                                                    }

                                                    Debug.Log("Added set to " + (newComps + existComps) +
                                                              " components... \n [" + existComps + " existing] [" +
                                                              newComps + " new]");
                                                }

                                                if (GUILayout.Button("Apply", MegaPint.MegaPintGUI.GetStyle("button1"),
                                                        GUILayout.Height(20), GUILayout.Width(65))) {
                                                    var applyCount = 0;
                                                    foreach (var o in _materialSetsSelection) {
                                                        if (o.GetComponent<MegaPintMaterialSlots>() == null) continue;
                                                        var comp = o.GetComponent<MegaPintMaterialSlots>();
                                                        if (comp.materialSets.Contains(
                                                                MegaPint.Settings.materialSets[i])) {
                                                            comp.currentMaterialSet = MegaPint.Settings.materialSets[i];
                                                            if (o.GetComponent<MeshRenderer>() != null) {
                                                                o.GetComponent<MeshRenderer>().sharedMaterials =
                                                                    MegaPint.Settings.materialSets[i].materials
                                                                        .ToArray();
                                                            }

                                                            if (o.GetComponent<SkinnedMeshRenderer>() != null) {
                                                                o.GetComponent<SkinnedMeshRenderer>().sharedMaterials =
                                                                    MegaPint.Settings.materialSets[i].materials
                                                                        .ToArray();
                                                            }

                                                            applyCount++;
                                                        }
                                                    }

                                                    Debug.Log("Applied [" +
                                                              MegaPint.Settings.materialSets[i].materialSetName +
                                                              "] to " + applyCount + " components.");
                                                }

                                                EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.EndVertical();

                                                if (count >= 2) {
                                                    EditorGUILayout.EndHorizontal();
                                                    EditorGUILayout.Separator();
                                                    EditorGUILayout.BeginHorizontal();
                                                    count = 0;
                                                }
                                                else count++;
                                            }

                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.EndScrollView();
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case 2: // Settings
                    switch (ActiveMenu.menuName) {
                        case "General":
                            if (ActiveMenuEntry == null) {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("General", MegaPint.MegaPintGUI.GetStyle("header1"));
                                MegaPintGUIUtility.GuiLine(3);
                                return;
                            }

                            switch (ActiveMenuEntry.entryName) {
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
                                    MegaPint.Settings.visibleMaterialSets = EditorGUILayout.Toggle("Material Sets",MegaPint.Settings.visibleMaterialSets, MegaPint.MegaPintGUI.toggle);
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
                        case "Material Sets": return MegaPint.Settings.visibleMaterialSets;
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