using System.Linq;
using UnityEditor;

namespace MegaPint.Editor.Utility.MaterialSets {
    public class MegaPintMaterialSets {
        public const string Path = "Packages/com.tiogiras.megapint/Resources/MaterialSets";

        public static void UpdateMaterialSetsTemp() {
            var found = AssetDatabase.LoadAllAssetsAtPath(Path);
            var list = found.Where(o => o.GetType() == typeof(MegaPintMaterialSet)).Cast<MegaPintMaterialSet>().ToList();
            MegaPint.Settings.materialSets = list;
        }
    }
}