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
        }
        GUILayout.Label("info:\n"+t.info);
        GUILayout.Label("log:\n"+t.log);
        GUILayout.Label("warnings:\n"+t.warnings);
        GUILayout.Label("errors:\n"+t.errors);
    }
}
