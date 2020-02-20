using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public enum ResourceType {
    Health, Stamina
}

/// <summary>
/// Implements a resource (ie. health, stamina, etc)
/// </summary>
public struct Resource {
    [SerializeField]
    public ResourceType resourceType { get; private set; }

    [SerializeField] public float maxValue;
    [SerializeField] public float minValue;
    [SerializeField] public float currentValue;
    [SerializeField] public float regenDelay;
    [SerializeField] public float regenRate;
    [SerializeField] public AnimationCurve regenRateCurve;
    [SerializeField] private float regenStartTime;
    public Resource(ResourceType type) { resourceType = type; }
    
    public float currentMaxValue => maxValue;
    public float currentMinValue => minValue;
    public float currentValuePct => 
        (currentValue - currentMaxValue) / (currentMaxValue - currentMinValue);
    
    
    public void Reset() {
        currentValue = maxValue;
        currentMaxValue = maxValue;
        currentMinValue = minValue;
    }
    public void Update() {
        
    }
}


class ResourceBoostEffect {
    public float minValue;
    public float maxValue;
    public float boostTime;
    public float unboostTime;
    public AnimationCurve boostCurve;
    public AnimationCurve unboostCurve;
    public bool useBoostCurve;
    public bool useUnboostCurve;
    public bool useSeparateUnboostValues;
    
    enum State { None, Boosting, Boosted, Unboosting }
    private State state;
    private float startTime;

    /// <summary>
    /// Resets the current effect
    /// </summary>
    public void Reset() { state = State.None; }
    
    /// <summary>
    /// Starts the current effect
    /// </summary>
    public void Start() {
        if (boostTime > 0f) {
            state = State.Boosting;
            startTime = Time.time;
        } else {
            state = State.Boosted;
        }
    }
    /// <summary>
    /// Ends the current effect
    /// </summary>
    public void Terminate() {
        if (unboostTime > 0f) {
            state = State.Unboosting;
            startTime = Time.time;
        } else {
            state = State.None;
        }
    }
    
    /// <summary>
    /// Immediately ends the current effect (skips any animations / tweens / etc)
    /// </summary>
    public void TerminateImmediately() {
        state = State.None;
    }

    /// <summary>
    /// Get current value using internal state
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public float currentValue {
        get {
            switch (state) {
                case State.None: return minValue;
                case State.Boosted: return maxValue;
                
                // Use one case for both boosting + unboosting,
                // since unboosting may just use boosting values if useSeparateUnboostValues is false
                case State.Unboosting:
                case State.Boosting: {
                    // get appropriate parameters depending on what mode we're in
                    float duration;
                    bool useCurve;
                    AnimationCurve curve = null;
                    if (state == State.Boosting || !useSeparateUnboostValues) {
                        duration = boostTime;
                        useCurve = useBoostCurve && (curve = boostCurve) != null;
                    } else {
                        duration = unboostTime;
                        useCurve = useUnboostCurve && (curve = unboostCurve) != null;
                    }
                    
                    // get elapsed time + normalize it based on the duration of this effect
                    // (boosting or unboosting)
                    var t = (Time.time - startTime) / duration;
                    
                    // switch to a finish state if:
                    // - no duration (<= 0)
                    // - timer finished (Time.time > startTime), ie. t >= 1f
                    if (duration <= 0f || t >= 1f) {
                        if (state == State.Boosting) {
                            state = State.Boosted;
                            return maxValue;
                        } else {
                            state = State.Unboosting;
                            return minValue;
                        }
                    }
                    
                    // pass t through an arbitrary animation curve if we have an animation curve and it's
                    // currently enabled
                    if (useCurve) {
                        t = curve.Evaluate(t);
                    }
                    
                    // current value is an interpolation between min + max values given t
                    return minValue + (maxValue - minValue) * t;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    /// effect is currently active iff it's not yet finished
    /// </summary>
    public bool active => state != State.None;
}













