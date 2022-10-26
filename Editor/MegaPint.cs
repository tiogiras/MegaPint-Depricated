using MegaPint.Editor.Categories;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    
    public class MegaPint : EditorWindow {

        private readonly string[] _menuCategories = {"Applications", "Utility", "Settings"};
        private int _categoryIndex;

        private bool _isFocused;

        public static MegaPintSettingsData SettingsData;
        public static GUISkin MegaPintGUI;

        private static EditorWindow _window;

        private Vector2 _scrollPos;
        
        // --- CONTEXT FUNCTIONS ---
        
        [MenuItem("MegaPint/Open", false, 0)]
        private static void Init() {
            _window = GetWindow(typeof(MegaPint));
            _window.Show();
        }
        
        [MenuItem("MegaPint/Close All", false, 11)]
        private static void CloseAll() { }
        
        
        // --- BUILD IN ---
        
        private void OnEnable() {
            SettingsData = (MegaPintSettingsData)AssetDatabase.LoadAssetAtPath("Packages/com.tiogiras.megapint/MegaPintSettingsData.asset", typeof(MegaPintSettingsData));
            MegaPintGUI = (GUISkin)AssetDatabase.LoadAssetAtPath("Packages/com.tiogiras.megapint/MegaPint GUI.guiskin", typeof(GUISkin));
        }

        private void Update() {
            if (_window == null) return;
            if (_isFocused) _window.Repaint();
        }

        private void OnFocus() {
            _isFocused = true;
            _window = GetWindow(typeof(MegaPint));
        }

        private void OnLostFocus() => _isFocused = false;

        private void OnGUI() {
            GUI.skin = MegaPintGUI;

            EditorGUILayout.BeginVertical(MegaPintGUI.GetStyle("bg1"), GUILayout.Height(25));
                EditorGUILayout.LabelField("MegaPint of Code");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(MegaPintGUI.GetStyle("bg2"), GUILayout.ExpandHeight(true), GUILayout.Width(250));
                    _categoryIndex = GUILayout.Toolbar(_categoryIndex, _menuCategories);
                    CustomGUIUtility.GuiLine(2, .3f);
                    _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    switch (_categoryIndex)
                        {
                            case 0:
                                break;
                            case 1:
                                break;
                            case 2: //MegaPintSettingsCategory.DrawMenu();
                                break;
                        }
                    EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("CONTENT");
                EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUI.skin = null;
        }
    }
}