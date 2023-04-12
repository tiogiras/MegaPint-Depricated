using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Windows;
using Object = UnityEngine.Object;

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
        
        public static bool DrawFileEntry(string path) {
            if (path == "") return true;
            
            var value = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) value = true;

            var args = path.Split("/");
            var name = args[^1].Split(".")[0];
            
            EditorGUILayout.LabelField(name);
            EditorGUILayout.EndHorizontal();
            return value;
        }
        
        public static bool DrawFolderEntry(string path) {
            if (path == "") return true;
            
            var value = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) value = true;

            var args = path.Split("/");

            EditorGUILayout.LabelField(args[^1]);
            EditorGUILayout.EndHorizontal();
            return value;
        }
        
        public static List<GameObject> RemoveDuplicates(IEnumerable<GameObject> objs) {
            var newList = new List<GameObject>();
            foreach (var o in objs.Where(o => !newList.Contains(o))) {
                newList.Add(o);
            }

            return newList;
        }
        
        public static List<string> RemoveDuplicates(IEnumerable<string> objs) {
            var newList = new List<string>();
            foreach (var o in objs.Where(o => !newList.Contains(o))) {
                newList.Add(o);
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

        public enum MegaPintRenamingCommandFunction { SetTo, Remove, RemoveAt, Replace, Insert, Index }

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

            private enum MegaPintIndexPosition
            {
                End,
                Start
            }

            private MegaPintIndexPosition _indexPosition;

            private enum MegaPintIndexMode
            {
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

                switch (function)
                {
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

            public void Execute(List<GameObject> gameObjects, List<string> files, List<string> folders, bool finalStep) {
                switch (function) {
                    case MegaPintRenamingCommandFunction.SetTo:
                        foreach (var o in gameObjects) {
                            SetGameObjectName(o, _setToNewName);
                        }

                        for (var i = 0; i < files.Count; i++) {
                            var file = files[i];
                            SetFileName(file, i, _setToNewName, finalStep);
                        }

                        for (var i = 0; i < folders.Count; i++) {
                            var folder = folders[i];
                            SetFolderName(folder, i, _setToNewName, files, finalStep);
                        }
                        break;
                    case MegaPintRenamingCommandFunction.Remove:
                        foreach (var o in gameObjects) {
                            var newName = o.name.Replace(_removeRemoveStr, "");
                            SetGameObjectName(o, newName);
                        }

                        for (var i = 0; i < files.Count; i++) {
                            var file = files[i];
                            var fileName = GetCurrentFileName(file);
                            var newName = fileName.Replace(_removeRemoveStr, "");
                            SetFileName(file, i, newName, finalStep);
                        }
                        
                        for (var i = 0; i < folders.Count; i++) {
                            var folder = folders[i];
                            var folderName = GetCurrentFolderName(folder);
                            var newName = folderName.Replace(_removeRemoveStr, "");
                            SetFolderName(folder, i, newName, files, finalStep);
                        }
                        break;
                    case MegaPintRenamingCommandFunction.RemoveAt:
                        foreach (var o in gameObjects) {
                            var newName = o.name.Remove(_removeAtIndex, _removeAtLength);
                            SetGameObjectName(o, newName);
                        }
                        
                        for (var i = 0; i < files.Count; i++) {
                            var file = files[i];
                            var fileName = GetCurrentFileName(file);
                            var newName = fileName.Remove(_removeAtIndex, _removeAtLength);
                            SetFileName(file, i, newName, finalStep);
                        }
                        
                        for (var i = 0; i < folders.Count; i++) {
                            var folder = folders[i];
                            var folderName = GetCurrentFolderName(folder);
                            var newName = folderName.Remove(_removeAtIndex, _removeAtLength);
                            SetFolderName(folder, i, newName, files, finalStep);
                        }
                        break;
                    case MegaPintRenamingCommandFunction.Replace:
                        foreach (var o in gameObjects) {
                            var newName = o.name.Replace(_replaceCurrent, _replaceNew);
                            SetGameObjectName(o, newName);
                        }
                        
                        for (var i = 0; i < files.Count; i++) {
                            var file = files[i];
                            var fileName = GetCurrentFileName(file);
                            var newName = fileName.Replace(_replaceCurrent, _replaceNew);
                            SetFileName(file, i, newName, finalStep);
                        }
                        
                        for (var i = 0; i < folders.Count; i++) {
                            var folder = folders[i];
                            var folderName = GetCurrentFolderName(folder);
                            var newName = folderName.Replace(_replaceCurrent, _replaceNew);
                            SetFolderName(folder, i, newName, files, finalStep);
                        }
                        break;
                    case MegaPintRenamingCommandFunction.Insert:
                        foreach (var o in gameObjects) {
                            if (_insertIndex > o.name.Length - 1) continue;
                            var newName = o.name.Insert(_insertIndex, _insertString);
                            SetGameObjectName(o, newName);
                        }
                        
                        for (var i = 0; i < files.Count; i++) {
                            var file = files[i];
                            var fileName = GetCurrentFileName(file);
                            if (_insertIndex > fileName.Length - 1) continue;
                            var newName = fileName.Insert(_insertIndex, _insertString);
                            SetFileName(file, i, newName, finalStep);
                        }
                        
                        for (var i = 0; i < folders.Count; i++) {
                            var folder = folders[i];
                            var folderName = GetCurrentFolderName(folder);
                            if (_insertIndex > folderName.Length - 1) continue;
                            var newName = folderName.Insert(_insertIndex, _insertString);
                            SetFolderName(folder, i, newName, files, finalStep);
                        }
                        break;
                    case MegaPintRenamingCommandFunction.Index:

                        var individualNames = new List<string>();
                        var individualNamesCount = new List<int>();

                        foreach (var o in gameObjects) {
                            if (individualNames.Contains(o.name)) {
                                var index = individualNames.IndexOf(o.name);
                                individualNamesCount[index]++;
                            }
                            else {
                                individualNames.Add(o.name);
                                individualNamesCount.Add(1);
                            }
                        }

                        for (var i = 0; i < gameObjects.Count; i++) {
                            var o = gameObjects[i];
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

                        individualNames = new List<string>();
                        individualNamesCount = new List<int>();
                        
                        foreach (var fileName in files.Select(GetCurrentFileName)) {
                            if (individualNames.Contains(fileName)) {
                                var index = individualNames.IndexOf(fileName);
                                individualNamesCount[index]++;
                            }
                            else {
                                individualNames.Add(fileName);
                                individualNamesCount.Add(1);
                            }
                        }
                        
                        for (var i = 0; i < files.Count; i++) {
                            var file = files[i];
                            var fileName = GetCurrentFileName(file);
                            
                            var index = _indexMode switch {
                                MegaPintIndexMode.NumbersUp => $"{i + 1}",
                                MegaPintIndexMode.NumbersDown => $"{individualNamesCount[individualNames.IndexOf(fileName)] - i}",
                                MegaPintIndexMode.AlphabeticalUp => $"{NumberToAlphabetical(i + 1)}",
                                MegaPintIndexMode.AlphabeticalDown => $"{NumberToAlphabetical(individualNamesCount[individualNames.IndexOf(fileName)] - i)}",
                                _ => ""
                            };

                            var formattedIndex = _indexFormat.Replace(_indexPrefix, index);
                            var newName = (_indexPosition == MegaPintIndexPosition.Start ? formattedIndex : "") +
                                          fileName + (_indexPosition == MegaPintIndexPosition.End ? formattedIndex : "");
                            SetFileName(file, i, newName, finalStep);
                        }
                        
                        individualNames = new List<string>();
                        individualNamesCount = new List<int>();
                        
                        foreach (var folderName in folders.Select(GetCurrentFolderName)) {
                            if (individualNames.Contains(folderName)) {
                                var index = individualNames.IndexOf(folderName);
                                individualNamesCount[index]++;
                            }
                            else {
                                individualNames.Add(folderName);
                                individualNamesCount.Add(1);
                            }
                        }
                        
                        for (var i = 0; i < folders.Count; i++) {
                            var folder = folders[i];
                            var folderName = GetCurrentFolderName(folder);
                            
                            var index = _indexMode switch {
                                MegaPintIndexMode.NumbersUp => $"{i + 1}",
                                MegaPintIndexMode.NumbersDown => $"{individualNamesCount[individualNames.IndexOf(folderName)] - i}",
                                MegaPintIndexMode.AlphabeticalUp => $"{NumberToAlphabetical(i + 1)}",
                                MegaPintIndexMode.AlphabeticalDown => $"{NumberToAlphabetical(individualNamesCount[individualNames.IndexOf(folderName)] - i)}",
                                _ => ""
                            };

                            var formattedIndex = _indexFormat.Replace(_indexPrefix, index);
                            var newName = (_indexPosition == MegaPintIndexPosition.Start ? formattedIndex : "") +
                                          folderName + (_indexPosition == MegaPintIndexPosition.End ? formattedIndex : "");
                            
                            SetFolderName(folder, i, newName, files, finalStep);
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

            private string GetCurrentFileName(string fullFileName) {
                var args = fullFileName.Split("/");
                return args[^1].Split(".")[0];
            }
            
            private void SetFileName(string fileName, int index, string newName, bool finalStep) {
                var args = fileName.Split("/");
                var result = "";
                for (var i = 0; i < args.Length - 1; i++) {
                    result += $"{args[i]}/";
                }
                
                result += $"{newName}.{args[^1].Split(".")[1]}";

                if (finalStep) {
                    if (!AssetDatabase.ValidateMoveAsset(MegaPint.Interface.bulkRenamingSelectedFilesBackUp[index], result).Equals("")) 
                        result = AssetDatabase.GenerateUniqueAssetPath(result);
                    Debug.Log($"Old: {MegaPint.Interface.bulkRenamingSelectedFilesBackUp[index]} | New: {result}");
                    AssetDatabase.MoveAsset(MegaPint.Interface.bulkRenamingSelectedFilesBackUp[index], result);
                    AssetDatabase.Refresh();
                }
                
                MegaPint.Interface.bulkRenamingSelectedFiles[index] = result;
            }

            private string GetCurrentFolderName(string fullFolderName) {
                var args = fullFolderName.Split("/");
                return args[^1];
            }

            private void SetFolderName(string folderName, int index, string newName, List<string> files, bool finalStep) {
                var args = folderName.Split("/");
                var result = folderName.Replace(args[^1], newName);

                if (finalStep) {
                    if (!AssetDatabase.ValidateMoveAsset(MegaPint.Interface.bulkRenamingSelectedFoldersBackUp[index], result).Equals("")) 
                        result = AssetDatabase.GenerateUniqueAssetPath(result);
                    AssetDatabase.MoveAsset(MegaPint.Interface.bulkRenamingSelectedFoldersBackUp[index], result);
                    AssetDatabase.Refresh();
                }
                
                for (var i = 0; i < files.Count; i++) {
                    if (files[i].StartsWith(folderName)) files[i] = files[i].Replace(folderName, result);
                }
                
                MegaPint.Interface.bulkRenamingSelectedFolders[index] = result;
                MegaPint.Interface.bulkRenamingSelectedFiles = files;
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