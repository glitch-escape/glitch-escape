using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// interface for object that implement State structs that can be serialized / deserialized
public interface IPersistentData<TState> where TState : struct {
    ref TState GetStateRef(); // get a reference to this object's state (can read / write to / from this reference)
    string GetObjectID();     // get a unique objectPersistencyId for this object
}

/// acts as a store to manage persistent data for all objects across the scene
public class PersistentDataStore {
    public static PersistentDataStore instance { get; } = new PersistentDataStore();
    private Dictionary<string, string> serializedDataByObjectID = new Dictionary<string, string>();
    
    public bool TryLoadValue<T>(string name, ref T value) {
        if (!serializedDataByObjectID.ContainsKey(name)) return false;
        value = JsonUtility.FromJson<T>(serializedDataByObjectID[name]);
        return true;
    }
    public bool TryStoreValue<T>(string name, T value) {
        if (name == null || name == "") return false;
        serializedDataByObjectID[name] = JsonUtility.ToJson(value);
        return true;
    }
    // tbd: add struct serialization to / from json
    public bool TryRestoreState<TState>(IPersistentData<TState> obj) where TState : struct {
        var id = obj.GetObjectID();
        if (id == null || id == "") {
            Debug.LogError("attempted to load object of type "+obj.GetType().FullName+" with empty objectPersistencyId. "
                           + "cancelling TryRestoreState() + returning false");
            return false;
        }
        if (serializedDataByObjectID.ContainsKey(id)) {
            var state = JsonUtility.FromJson<TState>(serializedDataByObjectID[id]);
            // Debug.Log("got state: " + state);
            obj.GetStateRef() = state;
            return true;
        }
        return false;
    }

    public bool TrySaveState<TState>(IPersistentData<TState> obj) where TState : struct {
        var id = obj.GetObjectID();
        if (id == null || id == "") {
            Debug.LogError("attempted to save object of type "+obj.GetType().FullName+" with empty objectPersistencyId. "
                           + "cancelling TrySaveState() with data " + JsonUtility.ToJson(obj.GetStateRef()));
            return false;
        }
        // Debug.Log("Saving object "+obj+" with key "+obj.GetObjectID() + ", data = " + JsonUtility.ToJson(obj.GetStateRef()));
        serializedDataByObjectID[id] = JsonUtility.ToJson(obj.GetStateRef());
        return true;
    }
    public string GetSavedObjectDataAsJson() {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        foreach (var entry in serializedDataByObjectID) {
            sb.AppendLine("    \""+ entry.Key + "\": " + entry.Value);
        }
        sb.AppendLine("}");
        return sb.ToString();
        // return JsonUtility.ToJson(serializedDataByUnityGuid);
    }
}

/// boilerplate for handling objects that can be saved / unsaved using state structs
public abstract class PersistentObject<TState> : MonoBehaviour, IPersistentData<TState> where TState : struct {
    public TState state;
    public ref TState GetStateRef() { return ref state; }
    public virtual string GetObjectID() { return typeof(TState).FullName+" "+gameObject.GetInstanceID(); }  // can override this if you want to provide your own object IDs
    protected bool TrySaveState() { return PersistentDataStore.instance?.TrySaveState(this) ?? false; }
    protected bool TryRestoreState() {
        var stateRestored = PersistentDataStore.instance?.TryRestoreState(this) ?? false;
        if (stateRestored) OnStateRestored();
        return stateRestored;
    }
    protected void OnEnable() { TryRestoreState(); }
    protected void OnDisable() { TrySaveState(); }
    protected abstract void OnStateRestored();
}

/// boilerplate for handling objects that can be saved / unsaved using state structs (and inheirits from AInteractiveObject)
public abstract class PersistentInteractiveObject<TState> : AInteractiveObject, IPersistentData<TState> where TState : struct {
    public TState state;
    public ref TState GetStateRef() { return ref state; }
    public virtual string GetObjectID() { return typeof(TState).FullName+" "+gameObject.GetInstanceID(); }  // can override this if you want to provide your own object IDs
    protected bool TrySaveState() { return PersistentDataStore.instance?.TrySaveState(this) ?? false; }
    protected bool TryRestoreState() {
        var stateRestored = PersistentDataStore.instance?.TryRestoreState(this) ?? false;
        if (stateRestored) OnStateRestored();
        return stateRestored;
    }

    protected void OnEnable() { TryRestoreState(); }
    protected void OnDisable() { TrySaveState(); }
    protected abstract void OnStateRestored();
}

/// example of an object that can be collected and that will be persistent across scenes
public class PersistentDataExample : PersistentInteractiveObject<PersistentDataExample.State> {
    
    // object references and immutable data (does not change during runtime) goes here
    [InjectComponent] public MeshRenderer blah;
    
    // all mutable object state (variables, etc) MUST go here
    [System.Serializable]
    public struct State {
        public bool collected; // = false;         <- be aware of value defaults as structs can't have user specified default values!
    }
    protected override void OnStateRestored() {
        if (state.collected) {
            gameObject.SetActive(false);
        }
    }
    public override void OnInteract(Player player) {
        // player picked us up - update state and disable self 
        state.collected = true;
        gameObject.SetActive(false);
        
        // state will get saved either at the above call (setting active false should call OnDisable()?), or, at minimum,
        // when the scene changes (all active objects will be disabled)
    }
    public override void OnFocusChanged(bool focused) {
        // show a highlight effect or something
    }
}
