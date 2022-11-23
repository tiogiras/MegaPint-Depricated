#if UNITY_EDITOR

using MegaPint.Editor.Utility;
using MegaPint.Editor.Utility.MaterialSets;
using UnityEngine;

namespace MegaPint.Editor {
    public class MegaPintFunctions : MonoBehaviour {

        public enum MegaPintFunction {
            None, AutoSave, UpdateMaterialSetsTemp
        }
        
        public static void InvokeFunction(MegaPintFunction function) {
            switch (function) {
                case MegaPintFunction.None: break;
                case MegaPintFunction.AutoSave: MegaPintAutoSave.Init(); break;
                case MegaPintFunction.UpdateMaterialSetsTemp: MegaPintMaterialSets.UpdateMaterialSetsTemp(); break;
                default:return;
            }
        }
    }
}

#endif