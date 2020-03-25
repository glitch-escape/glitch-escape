
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Implements stateful effects and transformations
/// </summary>
public class Stateful {
    private static Stateful instance;
    public bool historyRecordingEnabled = true;
    private LinkedList<IStatefulEffect> activeEffects;
    private void ExecuteStatefulEffect(IStatefulEffect effect) {
        if (historyRecordingEnabled) {        
            effect.Execute();
            activeEffects.AddLast(effect);
        } else {
            effect.Execute();
            effect.Finish();
        }
    }
    public void PurgeEventsUpTo(float time) {
        foreach (var effect in activeEffects) {
            if (effect.endTime < time) {
                
            }
        }   
    }
    
    
    
    
    // stub impl
    public static float currentTime => Time.time;

    /// <summary>
    /// Statefully instantiate an object
    /// </summary>
    /// <param name="prefab">The object to instantiate</param>
    /// <param name="pos">Position to spawn the object at</param>
    /// <param name="rot">Starting rotation</param>
    /// <param name="ctor">Optional callback to run initialization code on this object</param>
    /// <typeparam name="T"></typeparam>
    public static void Instantiate<T>(
        T prefab, 
        Vector3 pos, 
        Quaternion rot, 
        Constructor<T>.Delegate ctor) 
        where T : Component
    {
        // placeholder implementation
        var obj = GameObject.Instantiate(prefab, pos, rot);
        ctor?.Invoke(obj);
        
        // real implementation would be something like:
        instance.ExecuteStatefulEffect(new InstantiationEffect<T>(prefab, pos, rot, null, ctor));
    }
    
    /// <summary>
    /// Statefully set the object's velocity
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="velocity"></param>
    public static void SetVelocity(Rigidbody rb, Vector3 velocity) {
        if (velocity != rb.velocity) {
            instance.ExecuteStatefulEffect(new SetVelocityEffect(rb, velocity));
        }
    }

    /// <summary>
    /// Statefully set the object's transform
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="rotation"></param>
    public static void SetRotation(Transform transform, Quaternion rotation) {
        var rot = transform.rotation;
        if (rot != rotation) {
            instance.ExecuteStatefulEffect(
                new SetObjectFieldEffect<Transform, Quaternion>(
                    transform, rot, rotation, DoSetRotation));
        }
    }
    private static void DoSetRotation(Transform transform, Quaternion rotation) { transform.rotation = rotation; }
    
    /// <summary>
    /// Statefully set the object's position
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="position"></param>
    public static void SetPosition(Transform transform, Vector3 position) {
        var pos = transform.position;
        if (pos != position) {
            instance.ExecuteStatefulEffect(
                new SetObjectFieldEffect<Transform, Vector3>(
                    transform, pos, position, DoSetPosition));
        }
    }
    private static void DoSetPosition(Transform transform, Vector3 position) { transform.position = position; }
    
    /// <summary>
    /// Statefully set the object's local scale
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="scale"></param>
    public static void SetLocalScale(Transform transform, Vector3 scale) {
        var localScale = transform.localScale;
        if (localScale != scale) {
            instance.ExecuteStatefulEffect(
                new SetObjectFieldEffect<Transform, Vector3>(
                    transform, localScale, scale, DoSetLocalScale));
        }
    }
    private static void DoSetLocalScale(Transform transform, Vector3 scale) { transform.localScale = scale; }

    interface IStatefulEffect {
        float startTime { get; }
        float endTime { get; }
        float duration { get; }
        void Execute();
        void Unexecute();
        void Finish();
    }
    
    struct InstantiationEffect<T> : IStatefulEffect where T: Component {
        public float startTime { get; private set; }
        public float endTime => startTime;
        public float duration => 0f;
        private T instance;
        private T prefab;
        private Vector3 pos;
        private Quaternion rot;
        private Transform parent;
        private Constructor<T>.Delegate ctor;
        private bool destroyed;

        public InstantiationEffect(T prefab, Vector3 pos, Quaternion rot, Transform parent, Constructor<T>.Delegate ctor) {
            startTime = currentTime;
            instance = null;
            this.prefab = prefab;
            this.pos = pos;
            this.rot = rot;
            this.parent = parent;
            this.ctor = ctor;
            this.destroyed = false;
        }
        public void Execute() {
            startTime = currentTime;
            destroyed = false;
            if (instance == null) {
                instance =
                    parent != null
                        ? GameObject.Instantiate(prefab, pos, rot, parent)
                        : GameObject.Instantiate(prefab, pos, rot); 
            } else {
                instance.gameObject.SetActive(true);
            }
        }
        public void Unexecute() {
            destroyed = true;
            if (instance != null) {
                instance.gameObject.SetActive(false);
            }
        }
        public void Finish() {
            if (instance != null && destroyed) {
                GameObject.Destroy(instance.gameObject);
                instance = null;
            }   
        }
    }

    struct SetVelocityEffect : IStatefulEffect { 
        public float startTime { get; }
        public float endTime => startTime;
        public float duration => 0f;
        private Rigidbody rigidbody;
        private Vector3 newVelocity;
        private Vector3 prevVelocity;

        public SetVelocityEffect(Rigidbody rb, Vector3 velocity) {
            rigidbody = rb;
            newVelocity = velocity;
            prevVelocity = rb.velocity;
            startTime = Time.time;
        }
        public void Execute() {
            rigidbody.velocity = newVelocity;
        }
        public void Unexecute() {
            rigidbody.velocity = prevVelocity;
        }
        public void Finish() {}
    }
    struct SetObjectFieldEffect<O, T> : IStatefulEffect where O : class {
        private O obj;
        private SetField<O, T>.Delegate setField;
        public float startTime { get; private set; }
        public float endTime => startTime;
        public float duration => 0f;
        private T initialValue;
        private T newValue;

        public SetObjectFieldEffect(O obj, T initialValue, T newValue, SetField<O, T>.Delegate setField) {
            this.obj = obj;
            this.initialValue = initialValue;
            this.newValue = newValue;
            this.setField = setField;
            startTime = currentTime;
        }
        public void Execute() { setField(obj, newValue); }
        public void Unexecute() { setField(obj, initialValue); }
        public void Finish() {}
    }
    
    
    
    /// <summary>
    /// Helper class to declare an object constructor delegate of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Constructor<T> {
        public delegate void Delegate (T value);
    }
    public static class SetField<O, T> where O: class {
        public delegate void Delegate(O obj, T value);
    }
    
}