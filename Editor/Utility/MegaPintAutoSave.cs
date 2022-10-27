using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MegaPint.Editor.Utility {
    public class MegaPintAutoSave : EditorWindow {

        private DateTime _lastSaved;
        private DateTime _nextSave;
        private float _nextInterval;
        
        public static void Init() {
            MegaPint.AutoSaveWindow = GetWindow(typeof(MegaPintAutoSave), false, "AutoSave");
            MegaPint.AutoSaveWindow.maxSize = new Vector2(300, 75);
            MegaPint.AutoSaveWindow.minSize = new Vector2(300, 75);
            MegaPint.AutoSaveWindow.Show();
        }

        private void OnEnable() => SetNextTime();

        private void OnGUI() {
            EditorGUILayout.BeginVertical(MegaPint.MegaPintGUI.GetStyle("bg2"), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                EditorGUILayout.Separator();
                if (Application.isPlaying) {
                    EditorGUILayout.HelpBox("Disabled during play mode.", MessageType.Warning);
                    return;
                }
                EditorGUILayout.LabelField("Last saved:", _lastSaved.ToLongTimeString());
                var timeToSave = (int)(_nextInterval - EditorApplication.timeSinceStartup);
                EditorGUILayout.LabelField("Next Save:", _nextSave.ToLongTimeString() + " (" + (timeToSave + " Sec)") );
                EditorGUILayout.LabelField("Current Interval:", MegaPint.Settings.autoSaveIntervalTime + " Sec");
            EditorGUILayout.EndVertical();
            
            Repaint();
            EditorUtility.SetDirty(this);
        
            if (!(EditorApplication.timeSinceStartup > _nextInterval)) return;
            if (MegaPint.Settings.autoSaveMode == MegaPintSettings.MegaPintAutoSaveMode.SaveAsCurrent) EditorSceneManager.SaveOpenScenes();
            else {
                string path;
                var time = DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second;
                if (MegaPint.Settings.autoSaveDuplicatePath.Equals("")) path = SceneManager.GetActiveScene().path;
                else path = MegaPint.Settings.autoSaveDuplicatePath + "/" + SceneManager.GetActiveScene().name + time + ".unity";
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path, true);
                AssetDatabase.Refresh();
            }
            if (MegaPint.Settings.autoSaveConsoleLog) Debug.Log( _lastSaved + ": MegaPint Scene AutoSave");
            
            SetNextTime();
        }

        private void SetNextTime() {            
            _lastSaved = DateTime.Now;
            _nextSave = DateTime.Now.AddSeconds(MegaPint.Settings.autoSaveIntervalTime + 1);
            _nextInterval = (int)(EditorApplication.timeSinceStartup + MegaPint.Settings.autoSaveIntervalTime);
        }

        private void OnDestroy() {
            if (MegaPint.Settings.autoSaveAudioWarning) EditorApplication.Beep();
            if (MegaPint.Settings.autoSaveConsoleLog) Debug.LogWarning("MegaPint AutoSave disabled!");
        }
    }
}