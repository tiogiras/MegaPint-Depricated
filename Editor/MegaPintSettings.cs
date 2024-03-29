#if UNITY_EDITOR

using System.Collections.Generic;
using MegaPint.Editor.Utility.MaterialSets;
using UnityEngine;

namespace MegaPint.Editor {
    
    /// <summary>
    /// Stores general settings
    /// Stores settings for applications
    /// </summary>
    
    public class MegaPintSettings : ScriptableObject {
        [Space, Header("Settings")] 
        public bool warnOnPreviewToolUse = true;

        [Space, Header("Visible Applications")]
        public bool visibleGeoNode;
        
        [Space, Header("Visible Utility")]
        public bool visibleAutoSave = true;
        public bool visibleScreenshot = true;
        public bool visibleMaterialSets = true;
        public bool visibleBulkRenaming = true;
        
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

        [Space, Header("Material Sets - Settings")]
        public List<MegaPintMaterialSet> materialSets;
        public List<bool> materialSetsFoldouts;
        public string materialSetsSavePath;
    }
}

#endif