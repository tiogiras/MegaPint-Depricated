using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Utility {
    public class MegaPintBulkRenaming : MonoBehaviour {
        public static bool DrawGameObjectEntry(GameObject o) {
            if (o == null) return true;
            
            var value = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) value = true;
            EditorGUILayout.LabelField(o.name);
            EditorGUILayout.EndHorizontal();
            return value;
        }
        
        public static bool DrawFileEntry(string path) {
            if (path == "") return true;
            
            var value = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) value = true;

            var args = path.Split("/");
            var name = args[^1].Split(".")[0];
            
            EditorGUILayout.LabelField(name);
            EditorGUILayout.EndHorizontal();
            return value;
        }
        
        public static bool DrawFolderEntry(string path) {
            if (path == "") return true;
            
            var value = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20))) value = true;

            var args = path.Split("/");

            EditorGUILayout.LabelField(args[^1]);
            EditorGUILayout.EndHorizontal();
            return value;
        }
        
        public static List<GameObject> RemoveDuplicates(IEnumerable<GameObject> objs) {
            var newList = new List<GameObject>();
            foreach (var o in objs.Where(o => !newList.Contains(o))) {
                newList.Add(o);
            }

            return newList;
        }
        
        public static List<string> RemoveDuplicates(IEnumerable<string> objs) {
            var newList = new List<string>();
            foreach (var o in objs.Where(o => !newList.Contains(o))) {
                newList.Add(o);
            }

            return newList;
        }
    }
}
