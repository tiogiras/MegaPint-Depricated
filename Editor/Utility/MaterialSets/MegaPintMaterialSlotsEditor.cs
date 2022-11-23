using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Utility.MaterialSets {
    
    [CustomEditor(typeof(MegaPintMaterialSlots))]
    public class MegaPintMaterialSlotsEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var myTarget = (MegaPintMaterialSlots)target;

            EditorGUILayout.BeginVertical();
            if (myTarget.materialSets.Count > 0) {
                for (var i = 0; i < myTarget.materialSets.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                        myTarget.materialSets[i] = (MegaPintMaterialSet)EditorGUILayout.ObjectField("", myTarget.materialSets[i], typeof(MegaPintMaterialSet), false);
                        if (GUILayout.Button("Remove")) {
                            myTarget.materialSets.RemoveAt(i);
                        }

                        if (myTarget.materialSets[i] != null && myTarget.currentMaterialSet != myTarget.materialSets[i]) {
                            if (GUILayout.Button("Set", GUILayout.MaxWidth(10))) {
                                myTarget.currentMaterialSet = myTarget.materialSets[i];
                            }
                        }
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            if (GUILayout.Button("Add Material Set")) {
                myTarget.materialSets.Add(null);
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}