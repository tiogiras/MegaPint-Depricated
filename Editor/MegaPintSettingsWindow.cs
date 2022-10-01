using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    public class MegaPintSettingsWindow : EditorWindow {

        private static MegaPintSettingsData _settingsData;
        
        [MenuItem("MegaPint/Settings", false, 0)]
        private static void Init() {
            var window = GetWindow(typeof(MegaPintSettingsWindow));
            window.Show();
        }

        private void OnEnable() => _settingsData = MegaPintSettingsData.CheckForExistingDataAsset();

        private void OnGUI() {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("MegaPint Settings", CustomGUIUtility.Headline(20) );
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("MegaPint stores Data in ScriptableObjects. Because of that you have to select a data asset to access many functions.", MessageType.Info);
            CustomGUIUtility.GuiLine(1);
            EditorGUILayout.Separator();

            if (_settingsData == null) {
                EditorGUILayout.HelpBox("No data found. Create a new one!", MessageType.Error);
                EditorGUILayout.Separator();
                
                if (!GUILayout.Button("Create data asset")) return;

                var path = EditorUtility.SaveFilePanelInProject( "Create a new data asset", "MegaPintSettingsDataAsset", "asset", "" );
                if (path.Equals("")) return;
                var data = CreateInstance<MegaPintSettingsData>( );
                         
                AssetDatabase.CreateAsset( data, path );
                AssetDatabase.SaveAssets(  );
                AssetDatabase.Refresh();
                         
                EditorUtility.FocusProjectWindow(  );

                _settingsData = data;
                Selection.activeObject = data;
            }
            else {
                EditorGUILayout.LabelField("Selected data asset:", AssetDatabase.GetAssetPath(_settingsData));
                CustomGUIUtility.GuiLine(1);
                EditorGUILayout.Separator();
            }
        }
    }
}