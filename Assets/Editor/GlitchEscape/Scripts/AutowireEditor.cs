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
        GUILayout.Label(t.info);
    }
}
