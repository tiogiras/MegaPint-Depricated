#if UNITY_EDITOR

using UnityEngine;

namespace MegaPint.Editor {
    
    /// <summary>
    /// Stores general settings
    /// Stores settings for applications
    /// </summary>
    
    public class MegaPintSettings : ScriptableObject {
        [Space, Header("Settings")] 
        [Space, Header("Visible Applications")]
        public bool visibleGeoNode = true;
        
        [Space, Header("Visible Utility")]
        public bool visibleAutoSave = true;
        public bool visibleScreenshot = true;
        public bool visibleMaterialSets = true;
        
        [Space, Header("AutoSave - Settings")]
        public int autoSaveIntervalTime = 30;
        public bool autoSaveAudioWarning = true;
        public bool autoSaveConsoleLog = true;
        public MegaPintAutoSaveMode autoSaveMode = MegaPintAutoSaveMode.SaveAsCurrent;
        public string autoSaveDuplicatePath = "";
        public enum MegaPintAutoSaveMode{SaveAsCurrent, SaveAsDuplicate}

        [Space, Header("Screenshot - Settings")]
        public string screenshotSavePath = "";
        public float screenshotStrengthNormal = 1;
        public float screenshotStrengthGlow = .5f;
    }
}

#endif