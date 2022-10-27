using UnityEngine;

namespace MegaPint.Editor {
    
    /// <summary>
    /// Stores general settings
    /// Stores settings for applications
    /// </summary>
    
    public class MegaPintSettings : ScriptableObject {

        [Space, Header("Settings")]
        [Space, Header("Visible Applications")]
        
        [Space, Header("Visible Utility")]
        public bool visibleAutoSave = true;
        
        [Space, Header("AutoSave - Settings")]
        public int autoSaveIntervalTime = 30;
        public bool autoSaveAudioWarning = true;
        public MegaPintAutoSaveMode autoSaveMode = MegaPintAutoSaveMode.SaveAsCurrent;
        public string autoSaveDuplicatePath = "";
        public enum MegaPintAutoSaveMode{SaveAsCurrent, SaveAsDuplicate}
    }
}