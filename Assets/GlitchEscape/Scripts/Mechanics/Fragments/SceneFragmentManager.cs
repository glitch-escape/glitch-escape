using System.Collections;
using System.Collections.Generic;
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
                // infoLog.AppendLine("Set " + fragments[i] + " objectPersistencyId = " + objectPersistencyId + ", virtue type = " + t.virtueType);
                // fragments[i].objectPersistencyId = id;
                // fragments[i].virtueType = t.virtueType;
                var obj = new SerializedObject(fragments[i]);
                obj.FindProperty("objectPersistencyId").stringValue = id;
                obj.FindProperty("virtueType").enumValueIndex = (int)t.virtueType;
                obj.ApplyModifiedProperties();
            }
        }
        if (GUILayout.Button("Clear IDs")) {
            foreach (var fragment in fragments) {
                fragment.objectPersistencyId = "";
            }
        }
        for (int i = 0; i < fragments.Length; ++i) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(fragments[i].objectPersistencyId != "" ? fragments[i].objectPersistencyId : "NO ID ASSIGNED");
            EditorGUILayout.ObjectField(fragments[i], fragments[i].GetType());
            var vt = (Virtue)EditorGUILayout.EnumPopup(fragments[i].virtueType);
            if (vt != fragments[i].virtueType) {
                var obj = new SerializedObject(fragments[i]);
                obj.FindProperty("virtueType").enumValueIndex = (int) vt;
                obj.ApplyModifiedProperties();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.Label(infoLog.ToString());

        // render the player fragment editor (has controls for simulating picking up fragments, etc)
        // iff a player is present
        var player = GameObject.FindObjectOfType<Player>();
        var playerFragments = player?.fragments;
        if (playerFragments != null) {
            GUILayout.Label("Player fragment pickup debug controls:");
            PlayerFragmentEditor.RenderEditorGUI(playerFragments);
        }
    }
}
