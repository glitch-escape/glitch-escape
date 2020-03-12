using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Autowire))]
public class AutowireEditor : Editor {
    public override void OnInspectorGUI() {
        var t = (Autowire) target;
        if (GUILayout.Button("Autoconfigure")) {
            t.Run();
        }
        if (GUILayout.Button("Clear")) {
            foreach (var component in t.GetComponents<Component>()) {
                if (component is Transform || component is Autowire || component is AutoWireTest) continue;
                DestroyImmediate(component);
            }
            t.info = t.log = t.warnings = t.errors = "";
        }
        if (t.info != "") GUILayout.Label("info:\n"+t.info);
        if (t.log != "") GUILayout.Label("log:\n"+t.log);
        if (t.warnings != "") GUILayout.Label("warnings:\n"+t.warnings);
        if (t.errors != "") GUILayout.Label("errors:\n"+t.errors);
    }
}
