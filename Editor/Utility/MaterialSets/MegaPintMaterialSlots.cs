#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace MegaPint.Editor.Utility.MaterialSets {
    public class MegaPintMaterialSlots : MonoBehaviour {
        public List<MegaPintMaterialSet> materialSets = new ();
        public MegaPintMaterialSet currentMaterialSet;
    }
}

#endif