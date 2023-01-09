#if UNITY_EDITOR

using System.IO;
using System.Linq;
using UnityEditor;

namespace MegaPint.Editor.Utility.MaterialSets {
    public static class MegaPintMaterialSets {
        public static void UpdateMaterialSetsTemp() {
            var paths = Directory.GetFiles(MegaPint.Settings.materialSetsSavePath, "*.asset");
            var found = paths.Select(AssetDatabase.LoadAssetAtPath<MegaPintMaterialSet>).Where(obj => obj != null).ToList();
            MegaPint.Settings.materialSets = found;
            MegaPint.Settings.materialSetsFoldouts = new bool[found.Count].ToList();
        }
    }
}

#endif