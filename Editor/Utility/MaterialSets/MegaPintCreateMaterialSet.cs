using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Utility.MaterialSets {
    public class MegaPintCreateMaterialSet : EditorWindow {

        private static List<Material> _materials;
        private static string _setName;
        private static Vector2 _scrollPos;

        private static bool _nameValidated;
        private static bool _nameValid;

        public static void Init() {
            MegaPint.CreateMaterialSetWindow = GetWindow(typeof(MegaPintCreateMaterialSet), true, "Create Material Set");
            MegaPint.CreateMaterialSetWindow.maxSize = new Vector2(500, 350);
            MegaPint.CreateMaterialSetWindow.minSize = new Vector2(500, 350);
            MegaPint.CreateMaterialSetWindow.Show();

            MegaPintMaterialSets.UpdateMaterialSetsTemp();
            
            _materials = new List<Material>();
            _setName = "";
            _scrollPos = Vector2.zero;

            _nameValidated = false;
            _nameValid = false;
        }

        private void OnGUI() {
            EditorGUILayout.BeginVertical(MegaPint.MegaPintGUI.GetStyle("bg2"), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Add Material Slots to the set.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
            EditorGUILayout.LabelField("Materials in the slots are applied to the MeshRenderers", MegaPint.MegaPintGUI.GetStyle("centertext1"));
            MegaPintGUIUtility.GuiLine(1, .3f);
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            _setName = EditorGUILayout.TextField("Name", _setName);
            EditorGUILayout.LabelField(_nameValidated ? _nameValid ? "Valid" : "Not valid" : "", GUILayout.Width(60));
            if (GUILayout.Button("Validate", GUILayout.Width(60))) {
                _nameValid = ValidateName();
                _nameValidated = true;
            }
            EditorGUILayout.EndHorizontal();
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true), GUILayout.Height(175));
            if (_materials.Count > 0) {
                for (var i = 0; i < _materials.Count; i++) {
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginHorizontal();
                        _materials[i] = (Material)EditorGUILayout.ObjectField("Slot " + (i + 1), _materials[i], typeof(Material), false);
                        if (GUILayout.Button("Remove", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.MaxWidth(75))) {
                            _materials.RemoveAt(i);
                        }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                if (GUILayout.Button("Add Material Slot", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.Height(25), GUILayout.Width(125))) {
                    _materials.Add(null);
                }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Separator();
            if (_materials.Count <= 0) EditorGUILayout.HelpBox("The set has no materials!", MessageType.Warning);
            else if (_materials.Any(material => material == null)) EditorGUILayout.HelpBox("A material slot is missing a material!", MessageType.Warning);
            else if (_setName.Equals("")) EditorGUILayout.HelpBox("The set needs a name!", MessageType.Warning);
            else if (!_nameValidated) EditorGUILayout.HelpBox("Validate the name before saving!", MessageType.Warning);
            else if (!_nameValid) EditorGUILayout.HelpBox("There already exists a set with this name! (Validate to check)", MessageType.Warning);
            else {
                if (GUILayout.Button("Create Material Set", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                    CreateSetAsset();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void CreateSetAsset() {
            var asset = CreateInstance<MegaPintMaterialSet>();
            asset.materialSetName = _setName;
            asset.materials = _materials;

            var path = MegaPintMaterialSets.Path + "/" + _setName + ".asset";
            AssetDatabase.CreateAsset(asset, path);
            asset.assetPath = path;
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            MegaPintMaterialSets.UpdateMaterialSetsTemp();
            Close();
        }

        private static bool ValidateName() {
            if (_setName.Equals("")) return false;
            if (_setName.StartsWith(" ")) return false;
            return MegaPint.Settings.materialSets.Count <= 0 || MegaPint.Settings.materialSets.All(set => !set.materialSetName.Equals(_setName));
        }
    }
}