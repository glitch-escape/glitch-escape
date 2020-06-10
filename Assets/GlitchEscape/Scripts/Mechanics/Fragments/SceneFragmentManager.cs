using System.Collections;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using GlitchEscape.Scripts.Utility;
using UnityEditor;
#endif
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

#if UNITY_EDITOR
[CustomEditor(typeof(SceneFragmentManager))]
public class SceneFragmentManagerEditor : Editor {
    private StringBuilder infoLog = new StringBuilder();
    
    public void AssignFragmentIDs(IEnumerable<FragmentPickup> fragments, Virtue virtueType) {
        int nextId = 1;
        foreach (var fragment in fragments) {
            string id = fragment.gameObject.scene.name + " fragment " + nextId++;
            var so = new SerializedObject(fragment);
            so.FindProperty("objectPersistencyId").stringValue = id;
            so.FindProperty("virtueType").enumValueIndex = (int)virtueType;
            so.ApplyModifiedProperties();
        }
    }

    public void ClearFragmentIDs(IEnumerable<FragmentPickup> fragments) {
        if (GUILayout.Button("Clear IDs")) {
            foreach (var fragment in fragments) {
                var so = new SerializedObject(fragment);
                so.FindProperty("objectPersistencyId").stringValue = "";
            }
        }
    }

    public void RenderEditorUI(SceneFragmentManager target) {
        var fragments = GlitchEscapeEditorUtilities.FindAllObjectsInScene<FragmentPickup>();
        if (GUILayout.Button("Assign virtue type + fragment IDs")) {
            infoLog.Clear();
            if (fragments.Count == 0) {
                infoLog.AppendLine("no fragments in scene!");
            }
            AssignFragmentIDs(fragments, target.virtueType);
        }
        if (GUILayout.Button("Clear IDs")) {
            ClearFragmentIDs(fragments);
        }
        foreach (var fragment in fragments) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(fragment.objectPersistencyId != "" ? fragment.objectPersistencyId : "NO ID ASSIGNED");
            EditorGUILayout.ObjectField(fragment, fragment.GetType(), true);
            // var root = GlitchEscapeEditorUtilities.GetRootTransform(fragment.transform);
            // EditorGUILayout.ObjectField(root, root.GetType(), true);
            // GUILayout.Label("active scene: '" + root.gameObject.scene.name + "', handle = " + root.gameObject.scene.handle + ", active scene handle = " + SceneManager.GetActiveScene().handle);
            // GUILayout.Toggle(GlitchEscapeEditorUtilities.IsObjectInScene(root.gameObject, SceneManager.GetActiveScene()), "in scene?");
            var vt = (Virtue)EditorGUILayout.EnumPopup(fragment.virtueType);
            if (vt != fragment.virtueType) {
                var obj = new SerializedObject(fragment);
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
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        RenderEditorUI((SceneFragmentManager)target);
    }
}
#endif