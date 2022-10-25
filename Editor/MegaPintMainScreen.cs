using MegaPint.Editor.Categories;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    public class MegaPintMainScreen : EditorWindow {

        private bool _foundSettings;
        private readonly string[] _menuCategories = {"Applications", "Utility", "Settings"};
        private int _categoryIndex;

        // --- CONTEXT FUNCTIONS ---
        [MenuItem("MegaPint/Open", false, 0)]
        private static void Init() {
            var window = GetWindow(typeof(MegaPintMainScreen));
            window.Show();
        }
        
        [MenuItem("MegaPint/Close All", false, 11)]
        private static void CloseAll() {
            Debug.Log("close All processes");
        }
        
        // --- BUILD IN ---
        private void OnEnable() => _foundSettings = MegaPintSettingsData.CheckForExistingDataAsset() != null;

        private void OnFocus() => _foundSettings = MegaPintSettingsData.CheckForExistingDataAsset() != null;

        private void OnGUI() {
            if (!_foundSettings) {
                DrawNoSettingsData();
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            
                EditorGUILayout.BeginVertical(CustomGUIUtility.Background(.19f), GUILayout.ExpandHeight(true), GUILayout.Width(200));
                    _categoryIndex = GUILayout.Toolbar(_categoryIndex, _menuCategories, GUILayout.ExpandWidth(true));
                    
                    CustomGUIUtility.GuiLine(1);
                    
                    switch (_categoryIndex) {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            MegaPintSettingsCategory.DrawMenu();
                            break;
                    }
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginVertical(CustomGUIUtility.Background(.22f), GUILayout.ExpandHeight(true));
                EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
        
        // --- FUNCTIONS ---
        private void DrawNoSettingsData() {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("No settings data found!", CustomGUIUtility.Headline(20) );
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("MegaPint stores Data in ScriptableObjects. Because of that you need a data asset to access many functions.", MessageType.Info);
            EditorGUILayout.HelpBox("Do not change the default name of the asset.", MessageType.Warning);
            CustomGUIUtility.GuiLine(1);
            EditorGUILayout.Separator();
                
            if (!GUILayout.Button("Create data asset")) return;

            var path = EditorUtility.SaveFilePanelInProject( "Create a new data asset", "MegaPintSettingsDataAsset", "asset", "" );
            if (path.Equals("")) return;
            var data = CreateInstance<MegaPintSettingsData>( );
                         
            AssetDatabase.CreateAsset( data, path );
            AssetDatabase.SaveAssets(  );
            AssetDatabase.Refresh();
                         
            EditorUtility.FocusProjectWindow(  );

            _foundSettings = true;
            Selection.activeObject = data;
        }
    }
}
