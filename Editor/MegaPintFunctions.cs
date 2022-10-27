using MegaPint.Editor.Utility;
using UnityEngine;

namespace MegaPint.Editor {
    public class MegaPintFunctions : MonoBehaviour {

        public enum MegaPintFunction {
            None,AutoSave
        }
        
        public static void InvokeFunction(MegaPintFunction function) {
            switch (function) {
                case MegaPintFunction.None: break;
                case MegaPintFunction.AutoSave: InvokeAutoSave(); break;
                default:return;
            }
        }
        
        private static void InvokeAutoSave() => MegaPintAutoSave.Init();
    }
}
