#if UNITY_EDITOR

using MegaPint.Editor.Utility;
using UnityEngine;

namespace MegaPint.Editor {
    public class MegaPintFunctions : MonoBehaviour {

        public enum MegaPintFunction {
            None, AutoSave
        }
        
        public static void InvokeFunction(MegaPintFunction function) {
            switch (function) {
                case MegaPintFunction.None: break;
                case MegaPintFunction.AutoSave: MegaPintAutoSave.Init(); break;
                default:return;
            }
        }
    }
}

#endif