#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Utility.MaterialSets {
    public static class MegaPintMaterialSets {
        public static void UpdateMaterialSetsTemp() {
            var paths = Directory.GetFiles(MegaPint.Settings.materialSetsSavePath, "*.asset");

            var found = new List<MegaPintMaterialSet>();
            foreach (var path in paths) {
                var obj = AssetDatabase.LoadAssetAtPath<MegaPintMaterialSet>(path);
                if (obj == null) continue;
                found.Add(obj);
            }
            
            Debug.Log($"found {found.Count} objects");
            MegaPint.Settings.materialSets = found;
            MegaPint.Settings.materialSetsFoldouts = new bool[found.Count].ToList();
        }
    }
}

#endif