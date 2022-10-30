using MegaPint.Editor.Applications.GeoNode;
using MegaPint.Editor.Utility;
using UnityEngine;

namespace MegaPint.Editor {
    public class MegaPintFunctions : MonoBehaviour {

        public enum MegaPintFunction {
            None, AutoSave, GeoNodeDebug
        }
        
        public static void InvokeFunction(MegaPintFunction function) {
            switch (function) {
                case MegaPintFunction.None: break;
                case MegaPintFunction.AutoSave: MegaPintAutoSave.Init(); break;
                case MegaPintFunction.GeoNodeDebug: GeoNode.DebugInit(); break;
                default:return;
            }
        }
    }
}
