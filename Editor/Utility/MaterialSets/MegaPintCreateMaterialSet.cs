#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Utility.MaterialSets {
    public class MegaPintCreateMaterialSet : EditorWindow {

        private static List<Material> Materials;
        private static string SetName;
        private static Vector2 ScrollPos;

        private static bool NameValidated;
        private static bool NameValid;

        public static void Init() {
            MegaPint.CreateMaterialSetWindow = GetWindow(typeof(MegaPintCreateMaterialSet), true, "Create Material Set");
            MegaPint.CreateMaterialSetWindow.maxSize = new Vector2(500, 350);
            MegaPint.CreateMaterialSetWindow.minSize = new Vector2(500, 350);
            MegaPint.CreateMaterialSetWindow.Show();

            MegaPintMaterialSets.UpdateMaterialSetsTemp();
            
            Materials = new List<Material>();
            SetName = "";
            ScrollPos = Vector2.zero;

            NameValidated = false;
            NameValid = false;
        }

        private void OnGUI() {
            EditorGUILayout.BeginVertical(MegaPint.MegaPintGUI.GetStyle("bg2"), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Add Material Slots to the set.", MegaPint.MegaPintGUI.GetStyle("centertext1"));
            EditorGUILayout.LabelField("Materials in the slots are applied to the MeshRenderers", MegaPint.MegaPintGUI.GetStyle("centertext1"));
            MegaPintGUIUtility.GuiLine(1, .3f);
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            SetName = EditorGUILayout.TextField("Name", SetName);
            EditorGUILayout.LabelField(NameValidated ? NameValid ? "Valid" : "Not valid" : "", GUILayout.Width(60));
            if (GUILayout.Button("Validate", GUILayout.Width(60))) {
                NameValid = ValidateName();
                NameValidated = true;
            }
            EditorGUILayout.EndHorizontal();
            
            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true), GUILayout.Height(175));
            if (Materials.Count > 0) {
                for (var i = 0; i < Materials.Count; i++) {
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginHorizontal();
                        Materials[i] = (Material)EditorGUILayout.ObjectField("Slot " + (i + 1), Materials[i], typeof(Material), false);
                        if (GUILayout.Button("Remove", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.MaxWidth(75))) {
                            Materials.RemoveAt(i);
                        }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                if (GUILayout.Button("Add Material Slot", MegaPint.MegaPintGUI.GetStyle("button1"), GUILayout.Height(25), GUILayout.Width(125))) {
                    Materials.Add(null);
                }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Separator();
            if (Materials.Count <= 0) EditorGUILayout.HelpBox("The set has no materials!", MessageType.Warning);
            else if (Materials.Any(material => material == null)) EditorGUILayout.HelpBox("A material slot is missing a material!", MessageType.Warning);
            else if (SetName.Equals("")) EditorGUILayout.HelpBox("The set needs a name!", MessageType.Warning);
            else if (!NameValidated) EditorGUILayout.HelpBox("Validate the name before saving!", MessageType.Warning);
            else if (!NameValid) EditorGUILayout.HelpBox("There already exists a set with this name! (Validate to check)", MessageType.Warning);
            else {
                if (GUILayout.Button("Create Material Set", MegaPint.MegaPintGUI.GetStyle("button1"))) {
                    CreateSetAsset();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void CreateSetAsset() {
            var asset = CreateInstance<MegaPintMaterialSet>();
            asset.materialSetName = SetName;
            asset.materials = Materials;

            var path = MegaPint.Settings.materialSetsSavePath + "/" + SetName + ".asset";
            AssetDatabase.CreateAsset(asset, path);
            asset.assetPath = path;
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            MegaPintMaterialSets.UpdateMaterialSetsTemp();
            Close();
        }

        private static bool ValidateName() {
            if (SetName.Equals("")) return false;
            if (SetName.StartsWith(" ")) return false;
            return MegaPint.Settings.materialSets.Count <= 0 || MegaPint.Settings.materialSets.All(set => !set.materialSetName.Equals(SetName));
        }
    }
}

#endif