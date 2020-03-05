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

    /// <summary>
    /// current regen value
    /// </summary>
    public float currentRegenValue => EvaluateRegenAt(Time.time - regenStartTime - regenDelay);

    float EvaluateRegenAt(float time) {
        if (time < 0f) return 0f;
        
        // TODO: apply animation curve
        
        return regenPerSec;
    }
    
    public new void Reset() {
        wasActiveLastFrame = false;
        regenStartTime = Time.time;
        base.Reset();
    }

    public void Update() {
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
}
