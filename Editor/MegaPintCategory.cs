using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor {
    
    public abstract class MegaPintCategory : ScriptableObject {
        
        protected int MenuIndex;
        protected int MenuEntryIndex;
    }
}
