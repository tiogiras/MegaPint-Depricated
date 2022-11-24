using System.Linq;
using UnityEngine;

namespace MegaPint.Editor.Utility.MaterialSets {
    public class MegaPintMaterialSets {
        public const string Path = "Packages/com.tiogiras.megapint/Resources/MaterialSets";

        public static void UpdateMaterialSetsTemp() {
            var found = Resources.LoadAll("MaterialSets", typeof(MegaPintMaterialSet)).Cast<MegaPintMaterialSet>().ToList();
            MegaPint.Settings.materialSets = found;
        }
    }
}