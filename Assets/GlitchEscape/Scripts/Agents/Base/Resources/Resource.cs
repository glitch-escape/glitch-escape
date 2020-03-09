using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;



public abstract class Resource<Owner, Config, T> : MonoBehaviourBorrowingConfigFrom<Owner, Config>, IResettable
    where Owner : class, IConfigurable<Config>
    where Config : ScriptableObject
    where T : IComparable
{
    public abstract string name { get; }

    /// <summary>
    /// Default resource value
    /// </summary>
    public abstract T defaultValue { get; }
    
    /// <summary>
    /// Minimum resource value
    /// </summary>
    public abstract T minimum { get; }
    
    /// <summary>
    /// Maximum resource value
    /// </summary>
    public abstract T maximum { get; }

    /// <summary>
    /// Resets value to defaultValue
    /// </summary>
    public void Reset() { value = defaultValue; }
    private void Start() { Reset(); }
    
    /// <summary>
    /// Current resource value
    /// </summary>
    public T value {
        get { return _value; }
        set {
            // clamp value to min, max
            value = Clamp(value, minimum, maximum);
            
            // skip if result did not change
            if (value.CompareTo(_value) == 0) return;
            
            // set new value
            _value = value;
            
            // call on change listener
            onChanged?.Invoke(value);
            
            // check if hit min / max + call listeners
            if (value.CompareTo(minimum) <= 0) OnDepleted?.Invoke();
            else if (value.CompareTo(maximum) >= 0) onFilled?.Invoke();
        }
    }
    private T _value;
    
    public delegate void ChangeListener (T value);
    public delegate void Listener();

    /// <summary>
    /// Called when value changes
    /// </summary>
    public event ChangeListener onChanged;
    
    /// <summary>
    /// Called when value hits zero
    /// </summary>
    public event Listener OnDepleted;
    
    /// <summary>
    /// Called when value hits maximum
    /// </summary>
    public event Listener onFilled;
    
    /// <summary>
    /// Helper function - implement Math.Clamp / Mathf.Clamp dynamically
    /// </summary>
    private static dynamic Clamp(dynamic value, dynamic min, dynamic max) {
        return value < min ? min : value > max ? max : value;
    }

    public bool showDebugGUI = false;
    private void OnGUI() {
        if (!showDebugGUI) return;
        DrawDebugGUI();
    }
    public void DrawDebugGUI() {
        GUILayout.Label(name+ " resource value: " + value);
        GUILayout.Label(name+ " resource maximum: " + maximum);
        GUILayout.Label(name+ " resource minimum: " + minimum);
        GUILayout.Label(name+ " resource minimum: " + minimum);
        GUILayout.Label(name+ " resource defaultValue: " + defaultValue);
    }
}
