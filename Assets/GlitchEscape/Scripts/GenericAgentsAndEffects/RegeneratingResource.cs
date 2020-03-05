using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public abstract class RegeneratingResource<Owner, Config> : Resource<Owner, Config, float> 
    where Owner : class, IConfigurable<Config>
    where Config : ScriptableObject
{
    /// <summary>
    /// Is regeneration currently enabled?
    /// </summary>
    public abstract bool regenEnabled { get; }
    
    /// <summary>
    /// Regen rate, per second
    /// </summary>
    public abstract float regenPerSec { get; }
    
    /// <summary>
    /// Delay before regen starts, in seconds
    /// </summary>
    public abstract float regenDelay { get; }
    
    /// <summary>
    /// Animation curve for regen over time. May return null, in which case regen is linear.
    /// </summary>
    public abstract AnimationCurve regenCurve { get; }

    /// <summary>
    /// called when regeneration starts
    /// </summary>
    public event Listener onRegenStarted;

    /// <summary>
    /// called when regen ends
    /// </summary>
    public event Listener onRegenEnded;

    /// <summary>
    /// is this resource currently regenerating?
    /// </summary>
    public bool regenActive => regenEnabled &&
                               (regenPerSec >= 0f
                                   ? value < maximum
                                   : value > minimum);
    private bool wasActiveLastFrame = false;
    private float regenStartTime = 0f;

    public void TriggerRegen() {
        regenStartTime = Time.time; 
    }

    /// <summary>
    /// current regen value
    /// </summary>
    public float currentRegenValue => EvaluateRegenAt(Time.time - regenStartTime - regenDelay);

    float EvaluateRegenAt(float time) {
        if (time < 0f) return 0f;
        if (regenCurve == null || regenPerSec == 0f) return regenPerSec;
        
        // crappy solution
        // TODO: normalize + take curve derivative, and use that
        // (so curve can describe exact regen curve over time, and be scale agnostic)

        // very crappy approximation (depending on curve shape)
        var timeToFullRegen = Mathf.Abs(maximum - minimum) / regenPerSec;
        
        // normalize + clamp to [0, 1]
        var t = Mathf.Clamp01(time / timeToFullRegen);
        
        // return value sampled through curve
        return regenCurve.Evaluate(t) * regenPerSec;
    }
    
    public new void Reset() {
        wasActiveLastFrame = false;
        regenStartTime = Time.time;
        lastValue = defaultValue;
        base.Reset();
    }

    private float lastValue = 0f;

    public void Update() {
        // check current vs last value
        if (value != lastValue) {
            // re-trigger when decreases / goes opposite direction of signed regen value
            if ((value < lastValue) == (regenPerSec >= 0f)) {
                TriggerRegen();
            }
        }
        lastValue = value;

        // check regen state + fire off event listeners
        var active = regenActive;
        if (active != wasActiveLastFrame) {
            wasActiveLastFrame = active;
            if (active) onRegenStarted?.Invoke();
            else onRegenEnded?.Invoke();
        }
        
        // update resource using current regen if active
        if (active) {
            value = value + currentRegenValue * Time.deltaTime;
        }
    }
    void OnGUI() {
        if (!showDebugGUI) return;
        DrawDebugGUI();
    }
    new void DrawDebugGUI() {
        base.DrawDebugGUI();
        GUILayout.Label(name + " resource regen enabled " + regenEnabled);
        GUILayout.Label(name + " resource regen currently active " + regenActive);
        GUILayout.Label(name + " base regen speed " + regenPerSec);
        GUILayout.Label(name + " elapsed time since regen start " + (Time.time - regenStartTime));
        GUILayout.Label(name + " current regen speed " + currentRegenValue);
    }
}
