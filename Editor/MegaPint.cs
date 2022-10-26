using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    
    public class MegaPint : EditorWindow
    {

        private string[] _menuCategories;
        private int _categoryIndex;
        private int _menuIndex;

        private bool _isFocused;

        public static MegaPintSettings Settings;
        public static MegaPintInterface Interface;
        public static GUISkin MegaPintGUI;

        private static EditorWindow _window;

        private Vector2 _scrollPos;
        
        
        // --- CONTEXT FUNCTIONS ---
        
        [MenuItem("MegaPint/Open", false, 0)]
        private static void Init() {
            _window = GetWindow(typeof(MegaPint));
            _window.titleContent.text = "MegaPint";
            _window.Show();
        }

        [MenuItem("MegaPint/Close All", false, 11)]
        private static void CloseAll() {
            _window.Close();
        }
        
        
        // --- BUILD IN ---
        
        private void OnEnable() {
            Settings = (MegaPintSettings)AssetDatabase.LoadAssetAtPath("Packages/com.tiogiras.megapint/Resources/MegaPintSettingsData.asset", typeof(MegaPintSettings));
            Interface = (MegaPintInterface)AssetDatabase.LoadAssetAtPath("Packages/com.tiogiras.megapint/Resources/MegaPintInterface.asset", typeof(MegaPintInterface));
            MegaPintGUI = (GUISkin)AssetDatabase.LoadAssetAtPath("Packages/com.tiogiras.megapint/Resources/MegaPint GUI.guiskin", typeof(GUISkin));

            _menuCategories = Interface.GetCategoryNames();
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
                    MegaPintGUIUtility.GuiLine(2, .3f);
                    _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    Interface.DrawCategory(_categoryIndex);
                    
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