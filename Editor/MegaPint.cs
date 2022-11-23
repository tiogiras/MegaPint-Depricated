#if UNITY_EDITOR

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
        public static MegaPintFunctions Functions;

        private static EditorWindow _window;
        public static EditorWindow AutoSaveWindow;

        private Vector2 _scrollPos;
        private Vector2 _contentScrollPos;
        
        
        // --- CONTEXT FUNCTIONS ---
        
        [MenuItem("MegaPint/Open", false, 0)]
        private static void Init() {
            _window = GetWindow(typeof(MegaPint));
            _window.titleContent.text = "MegaPint";
            _window.Show();
        }

        [MenuItem("MegaPint/Close All", false, 11)]
        private static void CloseAll() {
            if (_window != null) _window.Close();
            if (AutoSaveWindow != null) AutoSaveWindow.Close();
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

            EditorGUILayout.BeginVertical(MegaPintGUI.GetStyle("bg1"), GUILayout.Height(35));
                EditorGUILayout.LabelField("MegaPint of Code", MegaPintGUI.GetStyle("header"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(MegaPintGUI.GetStyle("bg2"), GUILayout.ExpandHeight(true), GUILayout.Width(250));
                    _categoryIndex = GUILayout.Toolbar(_categoryIndex, _menuCategories);
                    MegaPintGUIUtility.GuiLine(2, .3f);
                    _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        Interface.DrawCategory(_categoryIndex);
                    EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                _contentScrollPos = EditorGUILayout.BeginScrollView(_contentScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    EditorGUILayout.BeginVertical();
                        Interface.DrawContent(_categoryIndex);
                        EditorGUILayout.Separator(); EditorGUILayout.Separator();
                    EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();

            GUI.skin = null;
        }
        
        
        // --- FUNCTIONS ---
        
        public static string GetApplicationPath() {
            var localPath = Application.dataPath;
            var parts = localPath.Split("/");
            localPath = "";
            for (var i = 0; i < parts.Length - 1; i++) localPath += parts[i] + "/";
            return localPath;
        }
    }
}

#endif