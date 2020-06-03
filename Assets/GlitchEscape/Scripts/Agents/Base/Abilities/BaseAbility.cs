using System;
using UnityEngine;

public abstract class BaseAbility : MonoBehaviour, IAgentAbility { 
    public abstract IAgent agent { get; }
    public abstract float resourceCost { get; }
    public abstract float cooldownTime { get; }
    protected abstract float abilityDuration { get; }

    public enum State { None, Active }
    public State state { get; private set; } = State.None;

    public bool canUseAbility => !isOnCooldown && CanStartAbility();
    public bool isAbilityActive => state == State.Active;
    
    private float abilityStartTime { get; set; } = -10f;
    protected float timeElapsedSinceAbilityStart => 
        isAbilityActive ? Time.time - abilityStartTime : 0f;
    
    protected bool isOnCooldown => Time.time < abilityStartTime + cooldownTime;

    /// <summary>
    /// Called whan an ability is started
    /// </summary>
    protected abstract void OnAbilityStart();

    protected virtual void OnAbilityUpdate() { }
    protected virtual void OnAbilityEnd() { }
    protected virtual void OnAbilityReset() { }
    protected virtual bool CanStartAbility() { return true; }

    public bool UseAbility() {
        return agent.TryUseAbility(this);
    }
    public void Internal_StartAbility() {
        if (!canUseAbility)
            throw new Exception(""+this+" ability precondition violated");
        if (isAbilityActive) {
            OnAbilityEnd();
        }
        state = State.Active;
        abilityStartTime = Time.time;
        OnAbilityStart();
    }
    public void CancelAbility() {
        if (isAbilityActive) {
            OnAbilityEnd();
        }
        state = State.None;
    }
    public void Reset() {
        CancelAbility();
        abilityStartTime = Time.time - cooldownTime - 1f;
        OnAbilityReset();
    }
    protected virtual void Update() {
        if (isAbilityActive) {
            if (Time.time >= abilityStartTime + abilityDuration) {
                OnAbilityEnd();
                state = State.None;
            } else {
                OnAbilityUpdate();    
            }
        }
    }
}