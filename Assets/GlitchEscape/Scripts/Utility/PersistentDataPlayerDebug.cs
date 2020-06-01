using System.Collections;
using System.Collections.Generic;
using GlitchEscape.Scripts.DebugUI;
using UnityEngine;

public class PersistentDataPlayerDebug : MonoBehaviour, IPlayerDebug {
    public void DrawDebugUI() {
        GUILayout.Label("persistent data: " + PersistentDataStore.instance.GetSavedObjectDataAsJson());
    }
    public string debugName => GetType().Name;
}
