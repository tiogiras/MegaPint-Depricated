#if UNITY_EDITOR

using System.Linq;
using UnityEngine;

namespace MegaPint.Editor.Utility.MaterialSets {
    public class MegaPintMaterialSets {
        public static void UpdateMaterialSetsTemp() {
            var found = Resources.LoadAll("MaterialSets", typeof(MegaPintMaterialSet)).Cast<MegaPintMaterialSet>().ToList();
            MegaPint.Settings.materialSets = found;
            MegaPint.Settings.materialSetsFoldouts = new bool[found.Count].ToList();
        }
    }
}

#endif