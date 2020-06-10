using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlitchEscape.Scripts.Utility {
    public class GlitchEscapeEditorUtilities {
        public static Transform GetRootTransform(Transform t) {
            while (t.parent != null) t = t.parent;
            return t;
        }
        public static bool IsObjectInScene(GameObject obj, Scene scene) {
            return obj.scene.handle == scene.handle;
        }
        public static bool IsObjectOrParentInScene(GameObject obj, Scene scene) {
            return IsObjectInScene(GetRootTransform(obj.transform).gameObject, scene);
        }
        public static List<T> FindAllObjectsInScene<T>(bool includeInactiveObjects = true) where T : Component {
            return FindAllObjectsInScene<T>(SceneManager.GetActiveScene(), includeInactiveObjects);
        }
        public static List<T> FindAllObjectsInScene<T>(Scene scene, bool includeInactiveObjects = true) where T : Component {
            var objects = new List<T>();
            foreach (var obj in Resources.FindObjectsOfTypeAll<T>()) {
                var go = obj.gameObject;
                // if (!IsObjectOrParentInScene(go, scene)) continue;
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave) {
                    continue;
                }
                if (EditorUtility.IsPersistent(go)) {
                    continue;
                }
                objects.Add(obj);
            }
            return objects;
        }
    }
}
#endif