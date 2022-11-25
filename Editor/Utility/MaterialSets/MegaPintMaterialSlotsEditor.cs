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
                        EditorGUILayout.LabelField(myTarget.materialSets[i].materialSetName);
                    
                        if (myTarget.materialSets[i] != null && myTarget.currentMaterialSet != myTarget.materialSets[i]) {
                            if (GUILayout.Button("Set", GUILayout.Width(40))) {
                                myTarget.currentMaterialSet = myTarget.materialSets[i];
                                
                                if (myTarget.gameObject.GetComponent<MeshRenderer>() != null) {
                                    myTarget.gameObject.GetComponent<MeshRenderer>().sharedMaterials = myTarget.currentMaterialSet.materials.ToArray();
                                }

                                if (myTarget.gameObject.GetComponent<SkinnedMeshRenderer>() != null) {
                                    myTarget.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials = myTarget.currentMaterialSet.materials.ToArray();
                                }
                            }
                        }
                        
                        if (GUILayout.Button("Remove", GUILayout.Width(100))) {
                            myTarget.materialSets.RemoveAt(i);
                        }
                        EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}