using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MegaPint.Editor.Utility {
    public class MegaPintBulkRenaming : MonoBehaviour {

        public static bool DrawGameObjectEntry(GameObject o) {
            if (o == null) return true;
            
            var value = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) value = true;
            EditorGUILayout.LabelField(o.name);
            EditorGUILayout.EndHorizontal();
            return value;
        }
        
        public static bool DrawFileEntry(MegaPintAsset asset) {
            if (asset == null) return true;
            
            var value = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) value = true;
            EditorGUILayout.LabelField(asset.ShortName);
            EditorGUILayout.EndHorizontal();
            return value;
        }
        
        public static bool DrawFolderEntry(MegaPintAsset asset) {
            if (asset == null) return true;
            
            var value = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) value = true;
            EditorGUILayout.LabelField(asset.ShortName);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        public class MegaPintAsset {
            public string AssetPath;
            public string ShortName;
            public string Ending;
        }

        public static MegaPintAsset LoadAssetAtPath(string path) {
            if (path.Equals("")) throw new ArgumentException("Path cannot be null");
            var args = path.Split("/").ToList();
            
            var index = -1;
            foreach (var arg in args.Where(arg => arg.Contains("Assets"))) {
                index = args.IndexOf(arg);
                break;
            }
            if (index < 0) throw new ArgumentException($"File outside the project: {path}");
            
            var assetPath = "";
            for (var i = index; i < args.Count; i++) {
                assetPath += $"/{args[i]}";
            }
            assetPath = assetPath.Remove(0, 1);

            var loadedObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (loadedObj == null) throw new NullReferenceException($"No asset found at: {assetPath}");

            Debug.Log(args[^1]);
            var shortName = args[^1].Contains(".") ? args[^1].Split(".")[0] : args[^1];
            var ending = args[^1].Contains(".") ? $".{args[^1].Split(".")[1]}" : "";

            Debug.Log($"Added Asset: \n MetaPath: {path} \n AssetPath: {assetPath} \n Ending: {ending} \n ShortName: {shortName}");
            
            return new MegaPintAsset {
                AssetPath = assetPath,
                Ending = ending,
                ShortName = shortName
            };
        }

        public static List<GameObject> RemoveDuplicates(IEnumerable<GameObject> objs) {
            var newList = new List<GameObject>();
            foreach (var o in objs.Where(o => !newList.Contains(o))) {
                newList.Add(o);
            }

            return newList;
        }
        
        public static List<MegaPintAsset> RemoveDuplicates(IEnumerable<MegaPintAsset> objs) {
            var newList = new List<MegaPintAsset>();
            foreach (var asset in objs) {
                if (newList.Contains(asset)) continue;
                newList.Add(asset);
            }
            return newList;
        }

        public static void InitializeChainWhenNull() {
            if (MegaPint.Interface.commandChain == null || MegaPint.Interface.commandChain.Count == 0) 
                MegaPint.Interface.commandChain = new List<MegaPintRenameCommand> { new() };
            MegaPint.Interface.commandChainRemove = new List<MegaPintRenameCommand>();
            MegaPint.Interface.moveUp = null;
            MegaPint.Interface.moveDown = null;
        }

        public enum MegaPintRenamingCommandFunction { SetTo, Remove, RemoveAt, Replace, Insert, InsertAtPattern, Index }

        [Serializable]
        public class MegaPintRenameCommand
        {
            public MegaPintRenamingCommandFunction function;

            private string _setToNewName;
            private string _removeRemoveStr;
            private int _removeAtIndex;
            private int _removeAtLength = 1;
            private string _replaceCurrent;
            private string _replaceNew;
            private int _insertIndex;
            private string _insertString;
            
            private enum MegaPintInsertPattern { LowerUpper, UpperLower }
            private MegaPintInsertPattern _insertPattern;

            private enum MegaPintIndexPosition {
                End,
                Start
            }

            private MegaPintIndexPosition _indexPosition;

            private enum MegaPintIndexMode {
                NumbersUp,
                NumbersDown,
                AlphabeticalUp,
                AlphabeticalDown
            }

            private MegaPintIndexMode _indexMode;
            private string _indexPrefix = "%%";
            private string _indexFormat = "(%%)";

            public void DrawContent() {
                var height = function switch {
                    MegaPintRenamingCommandFunction.RemoveAt or MegaPintRenamingCommandFunction.Replace or MegaPintRenamingCommandFunction.Insert => 120,
                    MegaPintRenamingCommandFunction.Index => 260,
                    _ => 100
                };

                EditorGUILayout.BeginHorizontal(MegaPint.MegaPintGUI.GetStyle("bg2"), GUILayout.MaxHeight(height));

                MegaPintGUIUtility.SpaceVert(1);

                EditorGUILayout.BeginVertical();

                MegaPintGUIUtility.Space(1);
                function = (MegaPintRenamingCommandFunction)EditorGUILayout.EnumPopup("Function", function);

                switch (function) {
                    case MegaPintRenamingCommandFunction.SetTo:
                        EditorGUILayout.HelpBox("Fully change the string to the new string", MessageType.Info);
                        MegaPintGUIUtility.Space(1);

                        _setToNewName = EditorGUILayout.TextField("New string", _setToNewName);
                        break;
                    case MegaPintRenamingCommandFunction.Remove:
                        EditorGUILayout.HelpBox("Remove a certain string", MessageType.Info);
                        MegaPintGUIUtility.Space(1);

                        _removeRemoveStr = EditorGUILayout.TextField("Remove string", _removeRemoveStr);
                        break;
                    case MegaPintRenamingCommandFunction.RemoveAt:
                        EditorGUILayout.HelpBox("Remove the char at a certain index", MessageType.Info);
                        MegaPintGUIUtility.Space(1);

                        _removeAtIndex = EditorGUILayout.IntField("Index", _removeAtIndex);
                        _removeAtLength = EditorGUILayout.IntField("Length", _removeAtLength);

                        if (_removeAtIndex < 0) _removeAtIndex = 0;
                        if (_removeAtLength <= 0) _removeAtLength = 1;
                        break;
                    case MegaPintRenamingCommandFunction.Replace:
                        EditorGUILayout.HelpBox("Replace all current sequences with the new strings", MessageType.Info);
                        MegaPintGUIUtility.Space(1);

                        _replaceCurrent = EditorGUILayout.TextField("Current", _replaceCurrent);
                        _replaceNew = EditorGUILayout.TextField("New", _replaceNew);
                        break;
                    case MegaPintRenamingCommandFunction.Insert:
                        EditorGUILayout.HelpBox("Insert a string at a certain index", MessageType.Info);
                        MegaPintGUIUtility.Space(1);

                        _insertIndex = EditorGUILayout.IntField("Index", _insertIndex);
                        _insertString = EditorGUILayout.TextField("Inserted string", _insertString);

                        if (_insertIndex < 0) _insertIndex = 0;
                        break;
                    case MegaPintRenamingCommandFunction.InsertAtPattern:
                        EditorGUILayout.HelpBox("Insert a string at a certain pattern", MessageType.Info);
                        MegaPintGUIUtility.Space(1);

                        _insertPattern = (MegaPintInsertPattern)EditorGUILayout.EnumPopup("Pattern", _insertPattern);
                        _insertString = EditorGUILayout.TextField("Inserted string", _insertString);
                        break;
                    case MegaPintRenamingCommandFunction.Index:
                        EditorGUILayout.HelpBox("Index all selection based on the specified format", MessageType.Info);
                        MegaPintGUIUtility.Space(1);

                        _indexPosition =
                            (MegaPintIndexPosition)EditorGUILayout.EnumPopup("Positioning", _indexPosition);
                        _indexMode = (MegaPintIndexMode)EditorGUILayout.EnumPopup("Mode", _indexMode);

                        MegaPintGUIUtility.Space(1);

                        if (_indexPrefix.Equals("")) EditorGUILayout.HelpBox("No Prefix specified", MessageType.Error);
                        else EditorGUILayout.HelpBox($"Prefix = {_indexPrefix}", MessageType.Info);

                        if (!_indexFormat.Contains(_indexPrefix) || _indexPrefix.Equals(""))
                            EditorGUILayout.HelpBox("Prefix not found in format", MessageType.Error);
                        else EditorGUILayout.HelpBox("Prefix detected", MessageType.Info);

                        MegaPintGUIUtility.Space(1);

                        _indexPrefix = EditorGUILayout.TextField("Prefix", _indexPrefix);
                        _indexFormat = EditorGUILayout.TextField("Format", _indexFormat);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorGUILayout.EndVertical();

                MegaPintGUIUtility.SpaceVert(1);

                EditorGUILayout.BeginVertical(MegaPint.MegaPintGUI.GetStyle("bg3"), GUILayout.MaxWidth(1),
                    GUILayout.ExpandHeight(true));
                MegaPintGUIUtility.Space(1);
                EditorGUILayout.EndVertical();

                MegaPintGUIUtility.SpaceVert(1);

                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(25));
                MegaPintGUIUtility.Space(1);
                if (GUILayout.Button("<-", GUILayout.MaxWidth(25)))
                {
                    MegaPint.Interface.moveUp = this;
                }

                if (GUILayout.Button("-", GUILayout.MaxWidth(25)))
                {
                    MegaPint.Interface.commandChainRemove.Add(this);
                }

                if (GUILayout.Button("->", GUILayout.MaxWidth(25)))
                {
                    MegaPint.Interface.moveDown = this;
                }

                EditorGUILayout.EndVertical();

                MegaPintGUIUtility.SpaceVert(1);

                EditorGUILayout.EndHorizontal();
            }

            public void Execute(bool finalStep) {
                switch (function) {
                    case MegaPintRenamingCommandFunction.SetTo:
                        foreach (var o in MegaPint.Interface.bulkRenamingSelectedGameObjects) {
                            SetGameObjectName(o, _setToNewName);
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFiles != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFiles.Count; i++) {
                                var file = MegaPint.Interface.bulkRenamingSelectedFiles[i];
                                MegaPint.Interface.bulkRenamingSelectedFiles[i] = SetFileName(file, _setToNewName, finalStep, false);
                            }   
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFolders != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFolders.Count; i++) {
                                var folder = MegaPint.Interface.bulkRenamingSelectedFolders[i];
                                MegaPint.Interface.bulkRenamingSelectedFolders[i] = SetFolderName(folder, _setToNewName, finalStep);
                            }   
                        }
                        break;
                    case MegaPintRenamingCommandFunction.Remove:
                        foreach (var o in MegaPint.Interface.bulkRenamingSelectedGameObjects) {
                            var newName = o.name.Replace(_removeRemoveStr, "");
                            SetGameObjectName(o, newName);
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFiles != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFiles.Count; i++) {
                                var file = MegaPint.Interface.bulkRenamingSelectedFiles[i];
                                var newName = file.ShortName.Replace(_removeRemoveStr, "");
                                MegaPint.Interface.bulkRenamingSelectedFiles[i] = SetFileName(file, newName, finalStep, false);
                            }   
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFolders != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFolders.Count; i++) {
                                var folder = MegaPint.Interface.bulkRenamingSelectedFolders[i];
                                var newName = folder.ShortName.Replace(_removeRemoveStr, "");
                                MegaPint.Interface.bulkRenamingSelectedFolders[i] = SetFolderName(folder, newName, finalStep);
                            }   
                        }
                        break;
                    case MegaPintRenamingCommandFunction.RemoveAt:
                        foreach (var o in MegaPint.Interface.bulkRenamingSelectedGameObjects) {
                            var newName = o.name.Remove(_removeAtIndex, _removeAtLength);
                            SetGameObjectName(o, newName);
                        }
                        
                        if (MegaPint.Interface.bulkRenamingSelectedFiles != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFiles.Count; i++) {
                                var file = MegaPint.Interface.bulkRenamingSelectedFiles[i];
                                var newName = file.ShortName.Remove(_removeAtIndex, _removeAtLength);
                                MegaPint.Interface.bulkRenamingSelectedFiles[i] = SetFileName(file, newName, finalStep, false);
                            }   
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFolders != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFolders.Count; i++) {
                                var folder = MegaPint.Interface.bulkRenamingSelectedFolders[i];
                                var newName = folder.ShortName.Remove(_removeAtIndex, _removeAtLength);
                                MegaPint.Interface.bulkRenamingSelectedFolders[i] = SetFolderName(folder, newName, finalStep);
                            }   
                        }
                        break;
                    case MegaPintRenamingCommandFunction.Replace:
                        foreach (var o in MegaPint.Interface.bulkRenamingSelectedGameObjects) {
                            var newName = o.name.Replace(_replaceCurrent, _replaceNew);
                            SetGameObjectName(o, newName);
                        }
                        
                        if (MegaPint.Interface.bulkRenamingSelectedFiles != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFiles.Count; i++) {
                                var file = MegaPint.Interface.bulkRenamingSelectedFiles[i];
                                var newName = file.ShortName.Replace(_replaceCurrent, _replaceNew);
                                MegaPint.Interface.bulkRenamingSelectedFiles[i] = SetFileName(file, newName, finalStep, false);
                            }   
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFolders != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFolders.Count; i++) {
                                var folder = MegaPint.Interface.bulkRenamingSelectedFolders[i];
                                var newName = folder.ShortName.Replace(_replaceCurrent, _replaceNew);
                                MegaPint.Interface.bulkRenamingSelectedFolders[i] = SetFolderName(folder, newName, finalStep);
                            }   
                        }
                        break;
                    case MegaPintRenamingCommandFunction.Insert:
                        foreach (var o in MegaPint.Interface.bulkRenamingSelectedGameObjects) {
                            if (_insertIndex > o.name.Length - 1) continue;
                            var newName = o.name.Insert(_insertIndex, _insertString);
                            SetGameObjectName(o, newName);
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFiles != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFiles.Count; i++) {
                                var file = MegaPint.Interface.bulkRenamingSelectedFiles[i];
                                var fileName = file.ShortName;
                                if (_insertIndex > fileName.Length - 1) continue;
                                var newName = fileName.Insert(_insertIndex, _insertString);
                                MegaPint.Interface.bulkRenamingSelectedFiles[i] = SetFileName(file, newName, finalStep, false);
                            }   
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFolders != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFolders.Count; i++) {
                                var folder = MegaPint.Interface.bulkRenamingSelectedFolders[i];
                                var folderName = folder.ShortName;
                                if (_insertIndex > folderName.Length - 1) continue;
                                var newName = folderName.Insert(_insertIndex, _insertString);
                                MegaPint.Interface.bulkRenamingSelectedFolders[i] = SetFolderName(folder, newName, finalStep);
                            }   
                        }
                        break;
                    case MegaPintRenamingCommandFunction.InsertAtPattern:
                        foreach (var o in MegaPint.Interface.bulkRenamingSelectedGameObjects) {
                            if (_insertIndex > o.name.Length - 1) continue;

                            var newName = o.name;
                            switch (_insertPattern) {
                                case MegaPintInsertPattern.LowerUpper:
                                    var detectedPatternIndices = new List<int>();
                                    var lastChar = newName[0];
                                    for (var i = 0; i < newName.Length; i++) {
                                        var c = newName[i];
                                        if (char.IsUpper(c) && !char.IsUpper(lastChar)) {
                                            detectedPatternIndices.Add(i);
                                        }
                                        
                                        lastChar = c;
                                    }
                                    
                                    var insertedChars = 0;
                                    if (detectedPatternIndices.Count > 0) {
                                        foreach (var index in detectedPatternIndices) {
                                            newName = newName.Insert(index + insertedChars, _insertString);
                                            insertedChars += _insertString.Length;
                                        }
                                    }
                                    break;
                                case MegaPintInsertPattern.UpperLower:
                                    detectedPatternIndices = new List<int>();
                                    lastChar = newName[0];
                                    for (var i = 0; i < newName.Length; i++) {
                                        var c = newName[i];
                                        if (!char.IsUpper(c) && char.IsUpper(lastChar)) {
                                            detectedPatternIndices.Add(i);
                                        }
                                        
                                        lastChar = c;
                                    }
                                    
                                    insertedChars = 0;
                                    if (detectedPatternIndices.Count > 0) {
                                        foreach (var index in detectedPatternIndices) {
                                            newName = newName.Insert(index + insertedChars, _insertString);
                                            insertedChars += _insertString.Length;
                                        }
                                    }
                                    break;
                                default: throw new ArgumentOutOfRangeException();
                            }

                            SetGameObjectName(o, newName);
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFiles != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFiles.Count; i++) {
                                var file = MegaPint.Interface.bulkRenamingSelectedFiles[i];
                                var fileName = file.ShortName;
                                if (_insertIndex > fileName.Length - 1) continue;
                                
                                var newName = fileName;
                                switch (_insertPattern) {
                                    case MegaPintInsertPattern.LowerUpper:
                                        var detectedPatternIndices = new List<int>();
                                        var lastChar = newName[0];
                                        for (var j = 0; j < newName.Length; j++) {
                                            var c = newName[j];
                                            if (char.IsUpper(c) && !char.IsUpper(lastChar)) {
                                                detectedPatternIndices.Add(j);
                                            }
                                            
                                            lastChar = c;
                                        }
                                        
                                        var insertedChars = 0;
                                        if (detectedPatternIndices.Count > 0) {
                                            foreach (var index in detectedPatternIndices) {
                                                newName = newName.Insert(index + insertedChars, _insertString);
                                                insertedChars += _insertString.Length;
                                            }
                                        }
                                        break;
                                    case MegaPintInsertPattern.UpperLower:
                                        detectedPatternIndices = new List<int>();
                                        lastChar = newName[0];
                                        for (var j = 0; j < newName.Length; j++) {
                                            var c = newName[j];
                                            if (!char.IsUpper(c) && char.IsUpper(lastChar)) {
                                                detectedPatternIndices.Add(j);
                                            }
                                            
                                            lastChar = c;
                                        }
                                        
                                        insertedChars = 0;
                                        if (detectedPatternIndices.Count > 0) {
                                            foreach (var index in detectedPatternIndices) {
                                                newName = newName.Insert(index + insertedChars, _insertString);
                                                insertedChars += _insertString.Length;
                                            }
                                        }
                                        break;
                                    default: throw new ArgumentOutOfRangeException();
                                }
                                
                                MegaPint.Interface.bulkRenamingSelectedFiles[i] = SetFileName(file, newName, finalStep, false);
                            }   
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFolders != null) {
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFolders.Count; i++) {
                                var folder = MegaPint.Interface.bulkRenamingSelectedFolders[i];
                                var folderName = folder.ShortName;
                                if (_insertIndex > folderName.Length - 1) continue;
                                                                
                                var newName = folderName;
                                switch (_insertPattern) {
                                    case MegaPintInsertPattern.LowerUpper:
                                        var detectedPatternIndices = new List<int>();
                                        var lastChar = newName[0];
                                        for (var j = 0; j < newName.Length; j++) {
                                            var c = newName[j];
                                            if (char.IsUpper(c) && !char.IsUpper(lastChar)) {
                                                detectedPatternIndices.Add(j);
                                            }
                                            
                                            lastChar = c;
                                        }
                                        
                                        var insertedChars = 0;
                                        if (detectedPatternIndices.Count > 0) {
                                            foreach (var index in detectedPatternIndices) {
                                                newName = newName.Insert(index + insertedChars, _insertString);
                                                insertedChars += _insertString.Length;
                                            }
                                        }
                                        break;
                                    case MegaPintInsertPattern.UpperLower:
                                        detectedPatternIndices = new List<int>();
                                        lastChar = newName[0];
                                        for (var j = 0; j < newName.Length; j++) {
                                            var c = newName[j];
                                            if (!char.IsUpper(c) && char.IsUpper(lastChar)) {
                                                detectedPatternIndices.Add(j);
                                            }
                                            
                                            lastChar = c;
                                        }
                                        
                                        insertedChars = 0;
                                        if (detectedPatternIndices.Count > 0) {
                                            foreach (var index in detectedPatternIndices) {
                                                newName = newName.Insert(index + insertedChars, _insertString);
                                                insertedChars += _insertString.Length;
                                            }
                                        }
                                        break;
                                    default: throw new ArgumentOutOfRangeException();
                                }

                                MegaPint.Interface.bulkRenamingSelectedFolders[i] = SetFolderName(folder, newName, finalStep);
                            }   
                        }
                        break;
                    case MegaPintRenamingCommandFunction.Index:

                        var individualNames = new List<string>();
                        var individualNamesCount = new List<int>();

                        foreach (var o in MegaPint.Interface.bulkRenamingSelectedGameObjects) {
                            if (individualNames.Contains(o.name)) {
                                var index = individualNames.IndexOf(o.name);
                                individualNamesCount[index]++;
                            }
                            else {
                                individualNames.Add(o.name);
                                individualNamesCount.Add(1);
                            }
                        }
                        
                        for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedGameObjects.Count; i++) {
                            var o = MegaPint.Interface.bulkRenamingSelectedGameObjects[i];
                            var index = _indexMode switch {
                                MegaPintIndexMode.NumbersUp => $"{i + 1}",
                                MegaPintIndexMode.NumbersDown => $"{individualNamesCount[individualNames.IndexOf(o.name)] - i}",
                                MegaPintIndexMode.AlphabeticalUp => $"{NumberToAlphabetical(i + 1)}",
                                MegaPintIndexMode.AlphabeticalDown => $"{NumberToAlphabetical(individualNamesCount[individualNames.IndexOf(o.name)] - i)}",
                                _ => ""
                            };

                            var formattedIndex = _indexFormat.Replace(_indexPrefix, index);
                            var newName = (_indexPosition == MegaPintIndexPosition.Start ? formattedIndex : "") +
                                          o.name + (_indexPosition == MegaPintIndexPosition.End ? formattedIndex : "");
                            SetGameObjectName(o, newName);
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFiles != null) {
                            individualNames = new List<string>();
                            individualNamesCount = new List<int>();
                        
                            foreach (var file in MegaPint.Interface.bulkRenamingSelectedFiles) {
                                if (individualNames.Contains(file.ShortName)) {
                                    var index = individualNames.IndexOf(file.ShortName);
                                    individualNamesCount[index]++;
                                }
                                else {
                                    individualNames.Add(file.ShortName);
                                    individualNamesCount.Add(1);
                                }
                            }
                        
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFiles.Count; i++) {
                                var file = MegaPint.Interface.bulkRenamingSelectedFiles[i];

                                var index = _indexMode switch {
                                    MegaPintIndexMode.NumbersUp => $"{i + 1}",
                                    MegaPintIndexMode.NumbersDown => $"{individualNamesCount[individualNames.IndexOf(file.ShortName)] - i}",
                                    MegaPintIndexMode.AlphabeticalUp => $"{NumberToAlphabetical(i + 1)}",
                                    MegaPintIndexMode.AlphabeticalDown => $"{NumberToAlphabetical(individualNamesCount[individualNames.IndexOf(file.ShortName)] - i)}",
                                    _ => ""
                                };

                                var formattedIndex = _indexFormat.Replace(_indexPrefix, index);
                                var newName = (_indexPosition == MegaPintIndexPosition.Start ? formattedIndex : "") +
                                              file.ShortName + (_indexPosition == MegaPintIndexPosition.End ? formattedIndex : "");
                                MegaPint.Interface.bulkRenamingSelectedFiles[i] = SetFileName(file, newName, finalStep, false);
                            }
                        }

                        if (MegaPint.Interface.bulkRenamingSelectedFolders != null) {
                            individualNames = new List<string>();
                            individualNamesCount = new List<int>();
                        
                            foreach (var folder in MegaPint.Interface.bulkRenamingSelectedFolders) {
                                if (individualNames.Contains(folder.ShortName)) {
                                    var index = individualNames.IndexOf(folder.ShortName);
                                    individualNamesCount[index]++;
                                }
                                else {
                                    individualNames.Add(folder.ShortName);
                                    individualNamesCount.Add(1);
                                }
                            }
                        
                            for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFolders.Count; i++) {
                                var folder = MegaPint.Interface.bulkRenamingSelectedFolders[i];

                                var index = _indexMode switch {
                                    MegaPintIndexMode.NumbersUp => $"{i + 1}",
                                    MegaPintIndexMode.NumbersDown => $"{individualNamesCount[individualNames.IndexOf(folder.ShortName)] - i}",
                                    MegaPintIndexMode.AlphabeticalUp => $"{NumberToAlphabetical(i + 1)}",
                                    MegaPintIndexMode.AlphabeticalDown => $"{NumberToAlphabetical(individualNamesCount[individualNames.IndexOf(folder.ShortName)] - i)}",
                                    _ => ""
                                };

                                var formattedIndex = _indexFormat.Replace(_indexPrefix, index);
                                var newName = (_indexPosition == MegaPintIndexPosition.Start ? formattedIndex : "") +
                                          folder.ShortName + (_indexPosition == MegaPintIndexPosition.End ? formattedIndex : "");
                            
                                MegaPint.Interface.bulkRenamingSelectedFolders[i] = SetFolderName(folder, newName, finalStep);
                            }   
                        }
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            private const int _ColumnBase = 26;
            private const int _DigitMax = 7;
            private const string _Digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            
            private string NumberToAlphabetical(int index) {
                switch (index) {
                    case <= 0: break;
                    case <= _ColumnBase: return _Digits[index - 1].ToString();
                }

                var sb = new StringBuilder().Append(' ', _DigitMax);
                var current = index;
                var offset = _DigitMax;
                while (current > 0) {
                    sb[--offset] = _Digits[--current % _ColumnBase];
                    current /= _ColumnBase;
                }
                return sb.ToString(offset, _DigitMax - offset);
            }

            private void SetGameObjectName(GameObject o, string newName) {
                o.name = newName;
                EditorSceneManager.MarkSceneDirty(o.scene);
            }

            private MegaPintAsset SetFileName(MegaPintAsset file, string newName, bool finalStep, bool useNewNameAsPath) {

                file.ShortName = newName;
                if (!finalStep) return file;
                
                var args = file.AssetPath.Split("/").ToList();
                args[^1] = newName;
                var newPath = "";
                for (var i = 0; i < args.Count - 1; i++) {
                    newPath += $"/{args[i]}";
                }
                newPath += $"/{args[^1]}{file.Ending}";
                newPath = newPath.Remove(0, 1);

                if (useNewNameAsPath) newPath = newName;

                var oldPath = file.AssetPath;
                if (useNewNameAsPath) {
                    oldPath = newName.Replace(newName.Split("/")[^1], file.AssetPath.Split("/")[^1]);
                }
                
                if (!AssetDatabase.ValidateMoveAsset(oldPath, newPath).Equals(""))
                    newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

                AssetDatabase.MoveAsset(oldPath, newPath);
                AssetDatabase.Refresh();

                return LoadAssetAtPath(newPath);
            }

            private MegaPintAsset SetFolderName(MegaPintAsset folder, string newName, bool finalStep) {

                folder.ShortName = newName;
                if (!finalStep) return folder;
                
                var args = folder.AssetPath.Split("/").ToList();
                var result = folder.AssetPath.Replace(args[^1], newName);

                if (!AssetDatabase.ValidateMoveAsset(folder.AssetPath, result).Equals("")) 
                    result = AssetDatabase.GenerateUniqueAssetPath(result);
                
                AssetDatabase.MoveAsset(folder.AssetPath, result);
                AssetDatabase.Refresh();

                if (MegaPint.Interface.bulkRenamingSelectedFiles != null) {
                    for (var i = 0; i < MegaPint.Interface.bulkRenamingSelectedFiles.Count; i++) {
                        var file = MegaPint.Interface.bulkRenamingSelectedFiles[i];
                        if (!file.AssetPath.StartsWith(folder.AssetPath)) continue;
                        var newFileName = file.AssetPath.Replace(folder.AssetPath, result);
                        MegaPint.Interface.bulkRenamingSelectedFiles[i] = SetFileName(file, newFileName, true, true);
                    }   
                }

                return LoadAssetAtPath(result);
            }
        }
        
        public static void SortSelection() {
            var objs = Selection.gameObjects;
            var lowestIndex = objs[0].transform.GetSiblingIndex();
            lowestIndex = objs.Select(o => o.transform.GetSiblingIndex()).Prepend(lowestIndex).Min();

            var sortedObjs = objs.OrderBy(o=>o.name).ToList();
            for (var i = 0; i < sortedObjs.Count; i++) {
                sortedObjs[i].transform.SetSiblingIndex(lowestIndex + i);
            }
        }
    }
}