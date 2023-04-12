#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaPint.Editor.Utility;
using MegaPint.Editor.Utility.MaterialSets;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace MegaPint.Editor {
    
    public class MegaPintInterface : ScriptableObject {

        public List<MegaPintCategory> categories;

        private static MegaPintMenu ActiveMenu;
        private static MegaPintMenuEntry ActiveMenuEntry;

        private Vector2 _materialSetsScrollPos;
        private Vector2 _materialSetsScrollPos1;
        private List<GameObject> _materialSetsSelection;

        public List<MegaPintBulkRenaming.MegaPintRenameCommand> commandChain;
        public List<MegaPintBulkRenaming.MegaPintRenameCommand> commandChainRemove;
        public MegaPintBulkRenaming.MegaPintRenameCommand moveUp;
        public MegaPintBulkRenaming.MegaPintRenameCommand moveDown;
        
        public List<GameObject> bulkRenamingSelectedGameObjects;
        public List<string> bulkRenamingSelectedFiles; // PATHS
        public List<string> bulkRenamingSelectedFilesBackUp; // PATHS
        public List<string> bulkRenamingSelectedFolders; // PATHS
        public List<string> bulkRenamingSelectedFoldersBackUp; // PATHS

        private Vector2 _bulkRenamingScrollPos;
        private Vector2 _bulkRenamingScrollPos1;

        public void DrawCategory(int activeCategory) => categories[activeCategory].Draw(activeCategory);
        
        public string[] GetCategoryNames() {
            var arr = new string[categories.Count];
            for (var i = 0; i < categories.Count; i++) {
                arr[i] = categories[i].categoryName;
            }
            return arr;
        }

        #region Generic

        [Serializable] public class MegaPintCategory {
            public string categoryName;
            public List<MegaPintMenu> menus;
            
            public void Draw(int currentCategoryIndex) {
                foreach (var menu in menus.Where(menu => GetMenuVisibility(currentCategoryIndex, menu.menuName))) {
                    menu.Draw();
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
                MegaPintGUIUtility.Space(1);
            }
        }
        
        [Serializable] public class MegaPintMenuEntry {
            public string entryName;
            public bool hasContent;
            public MegaPintFunctions.MegaPintFunction function;
            
            public void Draw() {
                if (!GUILayout.Button(entryName, MegaPint.MegaPintGUI.GetStyle("menuentrybutton"),
                        GUILayout.ExpandWidth(true))) return;
                if (function != MegaPintFunctions.MegaPintFunction.None) {
                    MegaPintFunctions.InvokeFunction(function);
                    if (!hasContent) return;
                }
                    
                ActiveMenuEntry = this;
            }
        }

        #endregion

        #region Custom

        public void DrawContent(int activeCategory) {

            if (ActiveMenu == null) return;

            switch (activeCategory) {
                case 0: // Applications
                    switch (ActiveMenu.menuName) {

                    }
                    break;
                case 1: // Utility
                    switch (ActiveMenu.menuName) {
                        case "AutoSave - Scene":
                            MegaPintGUIUtility.Space(1);
                            
                            EditorGUILayout.LabelField("AutoSave", MegaPint.MegaPintGUI.GetStyle("header1"));
                            MegaPintGUIUtility.GuiLine(3);
                            MegaPintGUIUtility.Space(1);
                            
                            MegaPint.Settings.autoSaveIntervalTime = EditorGUILayout.IntField("Interval Time", MegaPint.Settings.autoSaveIntervalTime);
                            MegaPint.Settings.autoSaveMode = (MegaPintSettings.MegaPintAutoSaveMode)EditorGUILayout.EnumPopup("Save Mode", MegaPint.Settings.autoSaveMode);
                            
                            if (MegaPint.Settings.autoSaveMode == MegaPintSettings.MegaPintAutoSaveMode.SaveAsDuplicate) {
                                EditorGUILayout.LabelField("Duplicate Folder", ".../" + MegaPint.Settings.autoSaveDuplicatePath);
                                
                                if (MegaPint.Settings.autoSaveDuplicatePath.Equals("")) EditorGUILayout.HelpBox("No Folder Selected", MessageType.Error);
                               
                                EditorGUILayout.BeginHorizontal();
                                    MegaPintGUIUtility.Space(2);
                                    
                                    if (GUILayout.Button("Change", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                        var path = EditorUtility.OpenFolderPanel("Select Folder for Duplicates", "Assets/", "");
                                        if (!path.Equals("")) {
                                            path = path.Replace(MegaPint.GetApplicationPath(), "");
                                            MegaPint.Settings.autoSaveDuplicatePath = path;
                                        }
                                    }
                                    MegaPintGUIUtility.Space(4);
                                    
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
                                    MegaPintGUIUtility.Space(1);
                                    
                                    EditorGUILayout.LabelField("Render Camera", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    MegaPintGUIUtility.Space(1);
                                    
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
                                                switch (MegaPintScreenshot.FileName) {
                                                    case null:
                                                    case "": EditorApplication.Beep(); break;
                                                    default: {
                                                        if (MegaPint.Settings.screenshotSavePath.Equals("")) EditorApplication.Beep();
                                                        else MegaPintScreenshot.RenderImage();
                                                        break;
                                                    }
                                                }
                                            }else EditorApplication.Beep();
                                        }
                                    EditorGUILayout.EndHorizontal();
                                    switch (MegaPintScreenshot.FileName) {
                                        case null:
                                        case "": EditorGUILayout.HelpBox("File Name Required", MessageType.Error); break;
                                    }
                                    MegaPintGUIUtility.Space(4);
                                    
                                    MegaPintScreenshot.RenderCamera = (Camera) EditorGUILayout.ObjectField("Rendering Camera", MegaPintScreenshot.RenderCamera, typeof(Camera), true);
                                    if (MegaPintScreenshot.RenderCamera == null) EditorGUILayout.HelpBox( "No Rendering Camera selected", MessageType.Error );
                                    MegaPintScreenshot.CurrentResolution = (MegaPintScreenshot.MegaPintScreenshotResolutions)EditorGUILayout.EnumPopup("Resolution", MegaPintScreenshot.CurrentResolution);
                                    if (MegaPintScreenshot.CurrentResolution == MegaPintScreenshot.MegaPintScreenshotResolutions.Custom) {
                                        EditorGUILayout.BeginHorizontal();
                                        MegaPintGUIUtility.Space(4);
                                        
                                        MegaPintScreenshot.CustomXRes = EditorGUILayout.IntField("Width", MegaPintScreenshot.CustomXRes);
                                        if (MegaPintScreenshot.CustomXRes <= 0) MegaPintScreenshot.CustomXRes = 1;
                                        MegaPintGUIUtility.Space(1);
                                        
                                        MegaPintScreenshot.CustomYRes = EditorGUILayout.IntField("Height", MegaPintScreenshot.CustomYRes);
                                        if (MegaPintScreenshot.CustomYRes <= 0) MegaPintScreenshot.CustomYRes = 1;
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    MegaPint.Settings.screenshotStrengthNormal = EditorGUILayout.Slider("Image Strength", MegaPint.Settings.screenshotStrengthNormal, .01f, 1);
                                    MegaPint.Settings.screenshotStrengthGlow = EditorGUILayout.Slider("Postprocessing Strength", MegaPint.Settings.screenshotStrengthGlow, 0, 1);
                                    EditorGUILayout.LabelField("Output Folder", ".../" + MegaPint.Settings.screenshotSavePath);
                                    if (MegaPint.Settings.screenshotSavePath.Equals("")) EditorGUILayout.HelpBox("No Folder Selected", MessageType.Error);
                                    EditorGUILayout.BeginHorizontal();
                                        MegaPintGUIUtility.Space(2);
                                    
                                        if (GUILayout.Button("Change", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                            var path = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets/", "");
                                            if (!path.Equals("")) {
                                                path = path.Replace(MegaPint.GetApplicationPath(), "");
                                                MegaPint.Settings.screenshotSavePath = path;
                                            }
                                        }   
                                        MegaPintGUIUtility.Space(4);
                                    EditorGUILayout.EndHorizontal();
                                    break;
                                case "Render Window": 
                                    MegaPintGUIUtility.Space(1);
                                    EditorGUILayout.LabelField("Render Window", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    MegaPintGUIUtility.Space(1);
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
                                    MegaPintGUIUtility.Space(4);
                                    MegaPintScreenshot.CurrentWindow = (MegaPintScreenshot.MegaPintTargetWindows)EditorGUILayout.EnumPopup("Target", MegaPintScreenshot.CurrentWindow);
                                    if (MegaPintScreenshot.CurrentWindow == MegaPintScreenshot.MegaPintTargetWindows.WindowByName) {
                                        MegaPintScreenshot.WindowName = EditorGUILayout.TextField("Window Name", MegaPintScreenshot.WindowName);
                                        if (MegaPintScreenshot.WindowName == null) EditorGUILayout.HelpBox("Window Name Required", MessageType.Error);
                                        else if (MegaPintScreenshot.WindowName.Equals("")) EditorGUILayout.HelpBox("Window Name Required", MessageType.Error);
                                    }
                                    EditorGUILayout.LabelField("Output Folder", ".../" + MegaPint.Settings.screenshotSavePath);
                                    if (MegaPint.Settings.screenshotSavePath.Equals("")) EditorGUILayout.HelpBox("No Folder Selected", MessageType.Error);
                                    EditorGUILayout.BeginHorizontal();
                                        MegaPintGUIUtility.Space(2);
                                        if (GUILayout.Button("Change", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                            var path = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets/", "");
                                            if (!path.Equals("")) {
                                                path = path.Replace(MegaPint.GetApplicationPath(), "");
                                                MegaPint.Settings.screenshotSavePath = path;
                                            }
                                        }   
                                        MegaPintGUIUtility.Space(4);
                                    EditorGUILayout.EndHorizontal();
                                    break;
                            }
                            break;
                        case "Material Sets [BETA]":
                            if (ActiveMenuEntry == null) {
                                MegaPintGUIUtility.Space(1);
                                EditorGUILayout.LabelField("Material Sets", MegaPint.MegaPintGUI.GetStyle("header1"));
                                MegaPintGUIUtility.GuiLine(3);
                                MegaPintGUIUtility.Space(1);
                                EditorGUILayout.LabelField("With this tool you create material sets", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("add them to objects and call them to change", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("from a central overlay.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                MegaPintGUIUtility.Space(4);
                                EditorGUILayout.LabelField("The edit tab let's you create new", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("and edit existing material sets.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                MegaPintGUIUtility.Space(4);
                                EditorGUILayout.LabelField("In the apply tab you add the sets", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("to selected gameObjects in your scene.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                MegaPintGUIUtility.Space(4);
                                EditorGUILayout.LabelField("By calling apply on a set will result in", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("all objects changing, that are selected", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                EditorGUILayout.LabelField("and contain the set.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                
                                MegaPintGUIUtility.Space(4);
                                
                                EditorGUILayout.LabelField("MaterialSet Folder", ".../" + MegaPint.Settings.materialSetsSavePath);
                                if (MegaPint.Settings.materialSetsSavePath.Equals("")) EditorGUILayout.HelpBox("No Folder Selected", MessageType.Error);
                                EditorGUILayout.BeginHorizontal();
                                MegaPintGUIUtility.Space(2);
                                if (GUILayout.Button("Change", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                    var path = EditorUtility.OpenFolderPanel("Select MaterialSet Folder", "Assets/", "");
                                    if (!path.Equals("")) {
                                        path = path.Replace(MegaPint.GetApplicationPath(), "");
                                        MegaPint.Settings.materialSetsSavePath = path;
                                    }
                                }   
                                MegaPintGUIUtility.Space(4);
                                EditorGUILayout.EndHorizontal();
                                return;
                            }

                            switch (ActiveMenuEntry.entryName) {
                                case "Edit":
                                    if (MegaPint.Settings.materialSetsSavePath.Equals("")) {
                                        EditorGUILayout.HelpBox("No MaterialSet folder selected", MessageType.Error);
                                    }
                                    else {
                                        MegaPintGUIUtility.Space(1);
                                        EditorGUILayout.LabelField("Material Sets", MegaPint.MegaPintGUI.GetStyle("header1"));
                                        MegaPintGUIUtility.GuiLine(3);
                                        MegaPintGUIUtility.Space(1);
                                    
                                        _materialSetsScrollPos = EditorGUILayout.BeginScrollView(_materialSetsScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                                        if (MegaPint.Settings.materialSets.Count > 0) {
                                            for (var i = 0; i < MegaPint.Settings.materialSets.Count; i++) {
                                                EditorGUILayout.BeginVertical(MegaPint.MegaPintGUI.GetStyle(i % 2 == 0 ? "bg3" : "bg2"), GUILayout.ExpandWidth(true));
                                            
                                                MegaPintGUIUtility.Space(1);
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
                                                        MegaPintGUIUtility.Space(2);
                                                        EditorGUILayout.EndVertical();
                                                        continue;
                                                    }
                                                }

                                                var indent = EditorGUI.indentLevel;
                                                EditorGUI.indentLevel++;
                                            
                                                MegaPintGUIUtility.Space(1);

                                                if (i >= MegaPint.Settings.materialSets.Count) {
                                                    MegaPintGUIUtility.Space(2);
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
                                                MegaPintGUIUtility.Space(2);
                                                if (GUILayout.Button("Add Material Slot", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.Height(20), GUILayout.Width(125))) {
                                                    set.materials.Add(null);
                                                }
                                                EditorGUILayout.EndHorizontal();
                                            
                                                MegaPintGUIUtility.Space(2);
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
                                        MegaPintGUIUtility.Space(1);
                                        EditorGUILayout.LabelField("Material Sets",
                                            MegaPint.MegaPintGUI.GetStyle("header1"));
                                        MegaPintGUIUtility.GuiLine(3);
                                        MegaPintGUIUtility.Space(1);

                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.LabelField("Selection:",
                                            "Objects need any rendering component.");
                                        EditorGUILayout.LabelField("[Add set]", "Adds the set to the object.");
                                        EditorGUILayout.LabelField("[Apply]", "Apply set as the current materials.");
                                        EditorGUI.indentLevel--;

                                        MegaPintGUIUtility.Space(1);
                                        MegaPintGUIUtility.GuiLine(1);
                                        MegaPintGUIUtility.Space(1);

                                        EditorGUILayout.BeginHorizontal();
                                        if (GUILayout.Button("Update", MegaPint.MegaPintGUI.GetStyle("button1"),
                                                GUILayout.Height(15), GUILayout.Width(75))) {
                                            _materialSetsSelection = new List<GameObject>();
                                            if (Selection.gameObjects.Length > 0) {
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
                                        }

                                        EditorGUILayout.LabelField("Selected Renderers:",
                                            _materialSetsSelection.Count + "");
                                        EditorGUILayout.EndHorizontal();

                                        MegaPintGUIUtility.Space(1);
                                        if (_materialSetsSelection.Count <= 0)
                                            EditorGUILayout.HelpBox("Nothing selected!", MessageType.Warning);
                                        else {
                                            _materialSetsScrollPos1 =
                                                EditorGUILayout.BeginScrollView(_materialSetsScrollPos1, GUIStyle.none,
                                                    GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true),
                                                    GUILayout.ExpandHeight(true));

                                            EditorGUILayout.BeginHorizontal();
                                            var count = 0;
                                            for (var i = 0; i < MegaPint.Settings.materialSets.Count; i++) {
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
                                                    MegaPintGUIUtility.Space(1);
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
                        case "Bulk Renaming [BETA]":
                            if (ActiveMenuEntry == null) {
                                MegaPintGUIUtility.Space(1);
                                EditorGUILayout.LabelField("Bulk Renaming", MegaPint.MegaPintGUI.GetStyle("header1"));
                                MegaPintGUIUtility.GuiLine(3);
                                MegaPintGUIUtility.Space(1);
                                // TODO description
                                return;
                            }

                            switch (ActiveMenuEntry.entryName) {
                                case "Command Chain":
                                    EditorGUILayout.BeginHorizontal();
                                    
                                    EditorGUILayout.BeginVertical(GUILayout.MaxWidth(10), GUILayout.ExpandHeight(true));
                                    MegaPintGUIUtility.Space(1);
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUILayout.MaxWidth(250), GUILayout.ExpandHeight(true));
                                    
                                    MegaPintGUIUtility.Space(1);
                                        
                                    EditorGUILayout.BeginHorizontal(MegaPint.MegaPintGUI.GetStyle("bg2"), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                                    EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                                    
                                    EditorGUILayout.LabelField("Selection", MegaPint.MegaPintGUI.GetStyle("centertext"));
                                    MegaPintGUIUtility.GuiLine(3, .25f);
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("Add Files")) {
                                        var folderPath = EditorUtility.OpenFolderPanel("Select Folder with files", "Assets", "");
                                        var list = Directory.GetFiles(folderPath + "/", "*", SearchOption.TopDirectoryOnly);
                                        var listCleaned = list.Where(s => !s.Contains(".meta")).ToList();
                                        var listFinal = new List<string>();

                                        foreach (var item in listCleaned) {
                                            var str = item;
                                            var args = str.Split("/");
                                            var index = 0;

                                            for (var j = 0; j < args.Length; j++) {
                                                var s = args[j];
                                                if (s.Contains("Assets")) index = j;
                                            }

                                            str = "";
                                            for (var j = index; j < args.Length; j++) {
                                                str += $"{args[j]}/";
                                            }

                                            str = str.Remove(str.Length - 1, 1);
                                            listFinal.Add(str);
                                        }

                                        bulkRenamingSelectedFiles ??= new List<string>();
                                        bulkRenamingSelectedFiles.AddRange(listFinal);
                                        
                                        bulkRenamingSelectedFiles = MegaPintBulkRenaming.RemoveDuplicates(bulkRenamingSelectedFiles);
                                        bulkRenamingSelectedFilesBackUp = bulkRenamingSelectedFiles;
                                        var log = "";
                                        foreach (var file in bulkRenamingSelectedFilesBackUp) {
                                            log += $"\n{file}";
                                        }

                                        Debug.Log(log);
                                    }

                                    if (GUILayout.Button("Add Folder")) {
                                        var folderPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
                                        var args = folderPath.Split("/");
                                        var index = 0;
                                        
                                        for (var i = 0; i < args.Length; i++) {
                                            var s = args[i];
                                            if (s.Contains("Assets")) index = i;
                                        }

                                        folderPath = "";
                                        for (var i = index; i < args.Length; i++) {
                                            folderPath += $"{args[i]}/";
                                        }

                                        folderPath = folderPath.Remove(folderPath.Length - 1, 1);

                                        bulkRenamingSelectedFolders ??= new List<string>();
                                        bulkRenamingSelectedFolders.Add(folderPath);
                                        
                                        bulkRenamingSelectedFolders = MegaPintBulkRenaming.RemoveDuplicates(bulkRenamingSelectedFolders);
                                        bulkRenamingSelectedFoldersBackUp = bulkRenamingSelectedFolders;
                                    }

                                    if (GUILayout.Button("Clear")) {
                                        bulkRenamingSelectedGameObjects?.Clear();
                                        bulkRenamingSelectedFiles?.Clear();
                                        bulkRenamingSelectedFolders?.Clear();
                                        
                                        bulkRenamingSelectedFilesBackUp?.Clear();
                                        bulkRenamingSelectedFoldersBackUp?.Clear();
                                    }
                                    
                                    EditorGUILayout.EndHorizontal();
                                    
                                    if (GUILayout.Button("Add selected GameObjects")) {
                                        bulkRenamingSelectedGameObjects ??= new List<GameObject>();
                                        bulkRenamingSelectedGameObjects.AddRange(Selection.gameObjects);
                                        bulkRenamingSelectedGameObjects = MegaPintBulkRenaming.RemoveDuplicates(bulkRenamingSelectedGameObjects);
                                    }
                                    
                                    MegaPintGUIUtility.GuiLine(3, .25f);

                                    _bulkRenamingScrollPos = GUILayout.BeginScrollView(_bulkRenamingScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar);
                                    
                                    EditorGUILayout.LabelField("GameObjects", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                    if (bulkRenamingSelectedGameObjects is { Count: > 0 }) {
                                        var removes = bulkRenamingSelectedGameObjects.Where(MegaPintBulkRenaming.DrawGameObjectEntry).ToList();
                                        if (removes.Count > 0) {
                                            foreach (var remove in removes) {
                                                bulkRenamingSelectedGameObjects.Remove(remove);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        EditorGUILayout.LabelField("None");
                                    }
                                    MegaPintGUIUtility.GuiLine(2, .25f);
                                    MegaPintGUIUtility.Space(2);
                                    
                                    EditorGUILayout.LabelField("Files", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                    if (bulkRenamingSelectedFiles is { Count: > 0 }) {
                                        var removes = bulkRenamingSelectedFiles.Where(MegaPintBulkRenaming.DrawFileEntry).ToList();
                                        if (removes.Count > 0) {
                                            foreach (var remove in removes) {
                                                bulkRenamingSelectedFiles.Remove(remove);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        EditorGUILayout.LabelField("None");
                                    }
                                    MegaPintGUIUtility.GuiLine(2, .25f);
                                    MegaPintGUIUtility.Space(2);
                                    
                                    EditorGUILayout.LabelField("Folders", MegaPint.MegaPintGUI.GetStyle("centertext1"));
                                    if (bulkRenamingSelectedFolders is { Count: > 0 }) {
                                        var removes = bulkRenamingSelectedFolders.Where(MegaPintBulkRenaming.DrawFolderEntry).ToList();
                                        if (removes.Count > 0) {
                                            foreach (var remove in removes) {
                                                bulkRenamingSelectedFolders.Remove(remove);
                                            }
                                        }
                                    }
                                    else EditorGUILayout.LabelField("None");
                                    MegaPintGUIUtility.GuiLine(2, .25f);
                                    MegaPintGUIUtility.Space(2);
                                    
                                    EditorGUILayout.EndScrollView();
                                    
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                        
                                    MegaPintGUIUtility.Space(1);
                                    
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUILayout.MaxWidth(10), GUILayout.ExpandHeight(true));
                                    MegaPintGUIUtility.Space(1);
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                                    MegaPintGUIUtility.Space(1);
                                    EditorGUILayout.LabelField("Command Chain", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3, .15f);
                                    
                                    _bulkRenamingScrollPos1 = GUILayout.BeginScrollView(_bulkRenamingScrollPos1, GUIStyle.none, GUI.skin.verticalScrollbar);

                                    foreach (var command in commandChain) {
                                        command.DrawContent();
                                        MegaPintGUIUtility.Space(1);
                                    }
                                    
                                    if (commandChainRemove.Count > 0 && commandChain.Count > 1) {
                                        foreach (var remove in commandChainRemove) {
                                            commandChain.Remove(remove);
                                        }
                                    }
                                    
                                    commandChainRemove?.Clear();

                                    if (moveUp != null) {
                                        var originalIndex = commandChain.IndexOf(moveUp);
                                        if (originalIndex > 0) {
                                            var targetIndex = originalIndex--;
                                            (commandChain[targetIndex], commandChain[originalIndex]) = (commandChain[originalIndex], commandChain[targetIndex]);
                                        }
                                        
                                        moveUp = null;
                                    }
                                    
                                    if (moveDown != null) {
                                        var originalIndex = commandChain.IndexOf(moveDown);
                                        if (originalIndex < commandChain.Count - 1) {
                                            var targetIndex = originalIndex++;
                                            (commandChain[targetIndex], commandChain[originalIndex]) = (commandChain[originalIndex], commandChain[targetIndex]);
                                        }
                                        
                                        moveDown = null;
                                    }

                                    EditorGUILayout.BeginHorizontal();
                                    
                                    if (GUILayout.Button("Clear", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.MaxWidth(100))) {
                                        commandChain = new List<MegaPintBulkRenaming.MegaPintRenameCommand>{new ()};
                                    }
                                    
                                    if (GUILayout.Button("Add Command", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                        commandChain.Add(new MegaPintBulkRenaming.MegaPintRenameCommand());
                                    }
                                    
                                    EditorGUILayout.EndHorizontal();
                                    
                                    if (GUILayout.Button("Execute Command Chain", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                                        foreach (var command in commandChain) {
                                            command.Execute(bulkRenamingSelectedGameObjects,
                                                bulkRenamingSelectedFiles, bulkRenamingSelectedFolders,
                                                commandChain[^1] == command);
                                        }
                                        bulkRenamingSelectedFilesBackUp = bulkRenamingSelectedFiles;
                                        bulkRenamingSelectedFoldersBackUp = bulkRenamingSelectedFolders;
                                    }

                                    EditorGUILayout.EndScrollView();
                                    
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                    break;
                            }
                            
                            break;
                    }
                    break;
                case 2: // Settings
                    switch (ActiveMenu.menuName) {
                        case "General":
                            if (ActiveMenuEntry == null) {
                                MegaPintGUIUtility.Space(1);
                                EditorGUILayout.LabelField("General", MegaPint.MegaPintGUI.GetStyle("header1"));
                                MegaPintGUIUtility.GuiLine(3);
                                MegaPint.Settings.warnOnPreviewToolUse = EditorGUILayout.Toggle("Preview Tool Warning", MegaPint.Settings.warnOnPreviewToolUse, MegaPint.MegaPintGUI.toggle);
                                return;
                            }

                            switch (ActiveMenuEntry.entryName) {
                                case "Applications":
                                    MegaPintGUIUtility.Space(1);
                                    EditorGUILayout.LabelField("Applications - Settings", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    MegaPintGUIUtility.Space(1);
                                    EditorGUILayout.LabelField("Chosen applications are displayed in the applications-menu.", MegaPint.MegaPintGUI.GetStyle("centertext"));
                                    MegaPintGUIUtility.Space(3);
                                    // MegaPint.Settings.visibleGeoNode = EditorGUILayout.Toggle("GeoNode",MegaPint.Settings.visibleGeoNode, MegaPint.MegaPintGUI.toggle);

                                    break;
                                case "Utility":
                                    MegaPintGUIUtility.Space(1);
                                    EditorGUILayout.LabelField("Utility - Settings", MegaPint.MegaPintGUI.GetStyle("header1"));
                                    MegaPintGUIUtility.GuiLine(3);
                                    MegaPintGUIUtility.Space(1);
                                    EditorGUILayout.LabelField("Chosen utilities are displayed in the utility-menu.", MegaPint.MegaPintGUI.GetStyle("centertext"));
                                    MegaPintGUIUtility.Space(3);
                                    MegaPint.Settings.visibleAutoSave = EditorGUILayout.Toggle("AutoSave - Scene", MegaPint.Settings.visibleAutoSave, MegaPint.MegaPintGUI.toggle);
                                    MegaPint.Settings.visibleScreenshot = EditorGUILayout.Toggle("Screenshot", MegaPint.Settings.visibleScreenshot, MegaPint.MegaPintGUI.toggle);
                                    MegaPint.Settings.visibleMaterialSets = EditorGUILayout.Toggle("Material Sets [BETA]", MegaPint.Settings.visibleMaterialSets, MegaPint.MegaPintGUI.toggle);
                                    MegaPint.Settings.visibleBulkRenaming = EditorGUILayout.Toggle("Bulk Renaming [BETA]", MegaPint.Settings.visibleBulkRenaming, MegaPint.MegaPintGUI.toggle);
                                    break;
                            }
                            break;
                        case "Contact":
                            MegaPintGUIUtility.Space(1);
                            EditorGUILayout.LabelField("Contact", MegaPint.MegaPintGUI.GetStyle("header1"));
                            MegaPintGUIUtility.GuiLine(3);
                            MegaPintGUIUtility.Space(1);
                            EditorGUILayout.LabelField("This is a work-in-progress project and will be updated regulary.", MegaPint.MegaPintGUI.GetStyle("centertext"));
                            EditorGUILayout.LabelField("Currently developed by Niklas Räder (Tiogiras).", MegaPint.MegaPintGUI.GetStyle("centertext"));
                            MegaPintGUIUtility.Space(1);
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
                        case "Material Sets [BETA]": return MegaPint.Settings.visibleMaterialSets;
                        case "Bulk Renaming [BETA]": return MegaPint.Settings.visibleBulkRenaming;
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