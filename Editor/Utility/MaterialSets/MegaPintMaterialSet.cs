#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace MegaPint.Editor.Utility.MaterialSets {
    public class MegaPintMaterialSet : ScriptableObject {
        public string materialSetName;
        public string assetPath;
        public List<Material> materials;
    }
}

#endif