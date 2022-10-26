using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MegaPint.Editor.Utility {
    public class AutoSaveWindow : EditorWindow {
        
        private static MegaPintSettings _settingsData;
        
        private DateTime _lastSaved;
        private DateTime _nextSave;
        private float _nextInterval;

        //[MenuItem("MegaPint/Utility/AutoSave", false, 11)]
        private static void Init() {
            var window = GetWindow(typeof(AutoSaveWindow));
            window.Show();
        }

        private void OnEnable() {
            //_settingsData = MegaPintSettingsData.CheckForExistingDataAsset();
            SetNextTime();
        }

        private void OnGUI() {
            if (_settingsData == null) {
                EditorGUILayout.HelpBox("This feature requires an existing data asset!", MessageType.Error);
                return;
            }

            if (Application.isPlaying) {
                EditorGUILayout.HelpBox("Disabled during play mode.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Last saved:", _lastSaved.ToLongTimeString());
        
            var timeToSave = (int)(_nextInterval - EditorApplication.timeSinceStartup);
            EditorGUILayout.LabelField("Next Save:", _nextSave.ToLongTimeString() + " (" + (timeToSave + " Sec)") );
            Repaint();

            EditorGUILayout.BeginHorizontal();
        
            EditorGUILayout.LabelField("Current Interval:", _settingsData.intervalTime + " Sec");

            if (GUILayout.Button("Change")) ChangeIntervalWindow.InitWindow();
        
            EditorGUILayout.EndHorizontal();

            EditorUtility.SetDirty(this);
        
            if (!(EditorApplication.timeSinceStartup > _nextInterval)) return;

            EditorSceneManager.SaveOpenScenes();
            Debug.Log( _lastSaved + ": MegaPint Scene AutoSave");
            SetNextTime();
        }

        private void SetNextTime() {            
            _lastSaved = DateTime.Now;
            _nextSave = DateTime.Now.AddSeconds(_settingsData.intervalTime + 1);
            _nextInterval = (int)(EditorApplication.timeSinceStartup + _settingsData.intervalTime);
        }

        private class ChangeIntervalWindow : EditorWindow {
            public static void InitWindow() {
                var windowRect = new Rect(new Vector2(0, 0), new Vector2(400, 100));
                var window = GetWindowWithRect(typeof(ChangeIntervalWindow), windowRect, true, "Change TimeInterval");
                window.Show();
            }

            private void OnGUI() {
                if (_settingsData == null) {
                    EditorGUILayout.HelpBox("This feature requires an existing data asset!", MessageType.Error);
                    return;
                }
                
                EditorGUILayout.HelpBox("The TimeInterval sets the time(sec) after which the opened scene is saved.", MessageType.Info);
                _settingsData.intervalTime = EditorGUILayout.IntField("Current TimeInterval", _settingsData.intervalTime);
                EditorUtility.SetDirty(_settingsData);
            }
        }

        private void OnDestroy() {
            if (_settingsData == null) return;
            EditorApplication.Beep();
            Debug.LogWarning("MegaPint AutoSave disabled!");
        }
    }
}