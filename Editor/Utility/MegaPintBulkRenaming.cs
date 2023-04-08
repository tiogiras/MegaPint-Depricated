using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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

        [Serializable] public class MegaPintRenameCommand {
            public MegaPintRenamingCommandFunction function;

            private string _setToNewName;
            private string _removeRemoveStr;
            private int _removeAtIndex;
            private string _replaceCurrent;
            private string _replaceNew;
            private int _insertIndex;
            private string _insertString;
            
            private enum MegaPintIndexPosition { End, Start }
            private MegaPintIndexPosition _indexPosition;
            private enum MegaPintIndexMode { NumbersUp, NumbersDown, AlphabeticalUp, AlphabeticalDown }
            private MegaPintIndexMode _indexMode;
            private string _indexPrefix = "%%";
            private string _indexFormat = "(%%)";

            public void DrawContent() {
                var height = function switch {
                    MegaPintRenamingCommandFunction.Replace or MegaPintRenamingCommandFunction.Insert => 120,
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
                        break; 
                    case MegaPintRenamingCommandFunction.Index: 
                        EditorGUILayout.HelpBox("Index all selection based on the specified format", MessageType.Info);
                        MegaPintGUIUtility.Space(1);
                        
                        _indexPosition = (MegaPintIndexPosition)EditorGUILayout.EnumPopup("Positioning", _indexPosition);
                        _indexMode = (MegaPintIndexMode)EditorGUILayout.EnumPopup("Mode", _indexMode);
                        
                        MegaPintGUIUtility.Space(1);
                        
                        if (_indexPrefix.Equals("")) EditorGUILayout.HelpBox("No Prefix specified", MessageType.Error);
                        else EditorGUILayout.HelpBox($"Prefix = {_indexPrefix}", MessageType.Info);
                        
                        if (!_indexFormat.Contains(_indexPrefix) || _indexPrefix.Equals("")) EditorGUILayout.HelpBox("Prefix not found in format", MessageType.Error);
                        else EditorGUILayout.HelpBox("Prefix detected", MessageType.Info);
                        
                        MegaPintGUIUtility.Space(1);
                        
                        _indexPrefix = EditorGUILayout.TextField("Prefix", _indexPrefix);
                        _indexFormat = EditorGUILayout.TextField("Format", _indexFormat);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
                
                EditorGUILayout.EndVertical();

                MegaPintGUIUtility.SpaceVert(1);
                
                EditorGUILayout.BeginVertical(MegaPint.MegaPintGUI.GetStyle("bg3"), GUILayout.MaxWidth(1), GUILayout.ExpandHeight(true));
                MegaPintGUIUtility.Space(1);
                EditorGUILayout.EndVertical();
                
                MegaPintGUIUtility.SpaceVert(1);
                
                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(25));
                MegaPintGUIUtility.Space(1);
                if (GUILayout.Button("<-", GUILayout.MaxWidth(25))) {
                    MegaPint.Interface.moveUp = this;
                }
                
                if (GUILayout.Button("-", GUILayout.MaxWidth(25))) {
                    MegaPint.Interface.commandChainRemove.Add(this);
                }
                
                if (GUILayout.Button("->", GUILayout.MaxWidth(25))) {
                    MegaPint.Interface.moveDown = this;
                }
                EditorGUILayout.EndVertical();
                
                MegaPintGUIUtility.SpaceVert(1);
                
                EditorGUILayout.EndHorizontal();
            }

            public void Execute(List<GameObject> bulkRenamingSelectedGameObjects, List<string> bulkRenamingSelectedFiles, List<string> bulkRenamingSelectedFolders)
            {
                
            }
        }
    }
}
