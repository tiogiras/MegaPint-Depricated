using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor {
    
    public class MegaPintSettingsData : ScriptableObject {

        public static MegaPintSettingsData CheckForExistingDataAsset() {
            var query = AssetDatabase.FindAssets("MegaPintSettingsDataAsset");
            return query.Length <= 0 ? null : query.Select(AssetDatabase.GUIDToAssetPath).Select(assetPath => 
                (MegaPintSettingsData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MegaPintSettingsData))).
                FirstOrDefault(asset => asset != null);
        }
        
        // AutoSave
        public int intervalTime = 30;
    }
}
