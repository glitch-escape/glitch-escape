using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// put this on a root object above fragments in the scene
/// exists to provide editor functionality to:
/// - assign fragments unique IDs
/// - bulk assign fragment virtue types
/// </summary>
public class SceneFragmentManager : MonoBehaviour {
    public Virtue virtueType;
}

[CustomEditor(typeof(SceneFragmentManager))]
public class SceneFragmentManagerEditor : Editor {
    private StringBuilder infoLog = new StringBuilder();
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var t = (SceneFragmentManager) target;
        var fragments = GameObject.FindObjectsOfType<FragmentPickup>();
        if (GUILayout.Button("Assign virtue type + fragment IDs")) {
            infoLog.Clear();
            if (fragments.Length == 0) {
                infoLog.AppendLine("no fragments in scene!");
            }
            string sceneName = SceneManager.GetActiveScene().name;
            for (int i = 0; i < fragments.Length; ++i) {
                string id = sceneName + " fragment " + i;
                // infoLog.AppendLine("Set " + fragments[i] + " id = " + id + ", virtue type = " + t.virtueType);
                fragments[i].id = id;
                fragments[i].virtueType = t.virtueType;
            }
        }
        if (GUILayout.Button("Clear IDs")) {
            foreach (var fragment in fragments) {
                fragment.id = "";
            }
        }
        for (int i = 0; i < fragments.Length; ++i) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(fragments[i].id != "" ? fragments[i].id : "NO ID ASSIGNED");
            fragments[i] = (FragmentPickup)EditorGUILayout.ObjectField(fragments[i], fragments[i].GetType());
            fragments[i].virtueType = (Virtue)EditorGUILayout.EnumPopup(fragments[i].virtueType);
            GUILayout.EndHorizontal();
        }
        GUILayout.Label(infoLog.ToString());
    }
}
