using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PersistentObjectTest : PersistentObject<PersistentObjectTest.State> {
    public struct State {
        public string foo;
        public int bar;
        public bool baz;
    }
    protected override void OnStateRestored() {}

    public void RestoreState() {
        TryRestoreState();
    }
    public void SaveState() {
        TrySaveState();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(PersistentObjectTest))]
public class PersistentObjectTestEditor : Editor {
    public override void OnInspectorGUI() {
        var t = (PersistentObjectTest) target;
        if (GUILayout.Button("save state")) {
            t.SaveState();       
        }

        if (GUILayout.Button("restore state")) {
            t.RestoreState();
        }
        t.state.foo = EditorGUILayout.TextField("foo", t.state.foo);
        t.state.bar = EditorGUILayout.IntField("bar", t.state.bar);
        t.state.baz = EditorGUILayout.Toggle("baz", t.state.baz);
        GUILayout.Label("saved data: " + PersistentDataStore.instance.GetSavedObjectDataAsJson());
    }
}
#endif