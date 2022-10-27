using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MegaPint.Editor.Utility {
    public class MegaPintAutoSave : EditorWindow {

        private DateTime _lastSaved;
        private DateTime _nextSave;
        private float _nextInterval;
        
        public static void Init() {
            var window = GetWindow(typeof(MegaPintAutoSave));
            window.Show();
        }

        private void OnEnable() => SetNextTime();

        private void OnGUI() {
            if (Application.isPlaying) {
                EditorGUILayout.HelpBox("Disabled during play mode.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Last saved:", _lastSaved.ToLongTimeString());
        
            var timeToSave = (int)(_nextInterval - EditorApplication.timeSinceStartup);
            EditorGUILayout.LabelField("Next Save:", _nextSave.ToLongTimeString() + " (" + (timeToSave + " Sec)") );
            Repaint();

            EditorGUILayout.BeginHorizontal();
        
            EditorGUILayout.LabelField("Current Interval:", MegaPint.Settings.autoSaveIntervalTime + " Sec");

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
            _nextSave = DateTime.Now.AddSeconds(MegaPint.Settings.autoSaveIntervalTime + 1);
            _nextInterval = (int)(EditorApplication.timeSinceStartup + MegaPint.Settings.autoSaveIntervalTime);
        }

        private class ChangeIntervalWindow : EditorWindow {
            public static void InitWindow() {
                var windowRect = new Rect(new Vector2(0, 0), new Vector2(400, 100));
                var window = GetWindowWithRect(typeof(ChangeIntervalWindow), windowRect, true, "Change TimeInterval");
                window.Show();
            }

            private void OnGUI() {
                EditorGUILayout.HelpBox("The TimeInterval sets the time(sec) after which the opened scene is saved.", MessageType.Info);
                MegaPint.Settings.autoSaveIntervalTime = EditorGUILayout.IntField("Current TimeInterval", MegaPint.Settings.autoSaveIntervalTime);
                EditorUtility.SetDirty(MegaPint.Settings);
            }
        }

        private void OnDestroy() {
            EditorApplication.Beep();
            Debug.LogWarning("MegaPint AutoSave disabled!");
        }
    }
}