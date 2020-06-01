using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FragmentPickup : PersistentInteractiveObject<FragmentPickup.State> {
    public Transform floatTextArea;
    public string interactMessage = "[Collect Shard]";
    //private FloatingTextController floatingText;
    public GameObject fragmentUI;
    public Virtue virtueType;

    [SerializeField]
    public string objectPersistencyId;

    public override string GetObjectID() { return objectPersistencyId; }
    
    /// stores persistent state for this object
    public struct State {
        public bool collected; // = false
    }
    
    /// called if / when state is restored
    /// called by <see cref="PersistentInteractiveObject{TState}"/>, in OnEnable() if / when state is restored
    /// note that state is saved in OnDisable(), which at minimum will be called when another scene is loaded
    /// (and in this case will be called after gameObject.SetActive())
    protected override void OnStateRestored() {
        // directly set state + set active
        if (state.collected) {
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    ///  is this fragment currently picked up?
    /// (wraps State.collected + responsible 
    /// </summary>
    public bool collected {
        get => state.collected;
        set {
            // Debug.Log("setting collected = "+value);
            if (state.collected == value) return;
            state.collected = value;
            // Debug.Log("Set collected = " + GetStateRef().collected);
            TrySaveState();
            if (collected) {
                OnFragmentPickedUp();
            }
        }
    }
    
    /// called when player picks up fragments
    /// interact behavior set in inspector, implemented in <see cref="AInteractiveObject"/>
    public override void OnInteract(Player player) {
        // Debug.Log("picking up fragment");
        collected = true;
        player.GetComponent<FragmentPickupManager>()?.PickUpFragment(this);
    }

    /// use to implement fragment pick up transitions / effects, etc
    /// called if / when this.collected is set to true
    /// TODO: use this to implement visual fragment pick up effect / animation / etc
    private void OnFragmentPickedUp() {
        gameObject.SetActive(false);
    }
    
    /// called when player is nearby / enters some interaction radius (if focused behavior is activated and set
    /// to activate on trigger; if so, must use a separate collider for interact)
    /// TODO: use this to implement fragment highlighting when player gets close
    public override void OnFocusChanged(bool focused) {}

    public void SaveState() {
        TrySaveState(); 
    }
    public void RestoreState() {
        TryRestoreState();
    }
}

[CustomEditor(typeof(FragmentPickup))]
public class FragmentPickupEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var t = (FragmentPickup) target;
        t.collected = EditorGUILayout.Toggle("collected", t.collected);
        if (GUILayout.Button("save state")) {
            t.SaveState();
        }
        if (GUILayout.Button("restore state")) {
            t.RestoreState();
        }
        GUILayout.Label("Persistent data: " + PersistentDataStore.instance.GetSavedObjectDataAsJson());
    }
}