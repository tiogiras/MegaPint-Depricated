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
        public int intervalTime = 30;
    }
}