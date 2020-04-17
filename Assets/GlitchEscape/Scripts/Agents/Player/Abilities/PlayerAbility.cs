﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAbilityState {
    None, 
    Active,    
    Ending
}

/// <summary>
/// Base class for all active player abilities.
///
/// Includes variables for stamina costs, ability durations, and variable ability strength
/// (any / all of which can, optionally, be based on variable press durations w/ a designer driven animation curve)
///
/// Handles all button press detection internal state management (which can become somewhat complex), and provides
/// a minimal, straightforward interface to subclass from + use.
/// </summary>
public abstract class PlayerAbility : BaseAbility, IPlayerEventSource {
    /// <summary>
    /// Player reference (<see cref="Player"/>)
    /// </summary>
    [InjectComponent] public Player player;
    public override IAgent agent => player;

    /// <summary>
    /// Generic event listener (<see cref="PlayerEvent"/>)
    /// dispatches events like eg. movement started / stopped, abilities used, etc.
    /// </summary>
    public event PlayerEvent.Event OnEvent;

    /// <summary>
    /// Fire an event
    /// </summary>
    /// <see cref="OnEvent"/>
    protected void FireEvent(PlayerEvent.Type eventType) {
        OnEvent?.Invoke(eventType);
    }
    
    /// <summary>
    /// specifies the button control that this ability uses
    /// </summary>
    protected abstract PlayerControls.HybridButtonControl inputButton { get; }
    
    protected override void Update() {
        base.Update();
        if (inputButton?.wasPressedThisFrame ?? false) {
            StartAbility();
        }
    }

    // TODO: refactor out
    #region MaterialHelpers
    private Renderer[] m_renderers = null;
    private List<Material> m_defaultMaterials = null;
    private List<Material> defaultMaterials => m_defaultMaterials ?? (m_defaultMaterials = GetDefaultMaterials());

    private List<Material> GetDefaultMaterials() {
        if (m_defaultMaterials != null) m_defaultMaterials.Clear();
        else m_defaultMaterials = new List<Material>();
        foreach (var renderer in childRenderers) {
            foreach (var material in renderer.materials) {
                m_defaultMaterials.Add(material);
            }
        }
        return m_defaultMaterials;
    }
    public delegate void MaterialApplicatorCallback(ref Material material);
    public void SetDefaultMaterials() {
        int i = 0;
        ApplyMaterials((ref Material mat) => mat = m_defaultMaterials[i++]);
    }
    public void ApplyMaterials(MaterialApplicatorCallback applyMaterial) {
        // save default materials, if we haven't done so already
        if (m_defaultMaterials == null) GetDefaultMaterials();
        foreach (var renderer in childRenderers) {
            var materials = renderer.materials;
            for (int i = 0; i < materials.Length; ++i) {
                applyMaterial(ref materials[i]);
            }
            renderer.materials = materials;
        }
    }
    /// <summary>
    /// Lazily gets all renderers attached to / in children of the player object
    /// </summary>
    protected Renderer[] childRenderers => m_renderers ?? (m_renderers = player.GetComponentsInChildren<Renderer>());
    
    #endregion MaterialHelpers
    
    // public float abilityCooldown = 0f;
    //
    // [Tooltip("Max stamina cost")]
    // public float maxStaminaCost = 30f;
    //
    // [Tooltip("Min stamina cost")]
    // public float minStaminaCost = 30f;
    //
    // public float minEffectStrength = 1f;
    // public float maxEffectStrength = 1f;
    //
    // public float minDuration = 1f;
    // public float maxDuration = 1f;
    //
    // public bool varyStrengthDependingOnPressTime = true;
    // public bool varyStaminaCostDependingOnPressTime = true;
    // public bool varyDurationDependingOnPressTime = true;
    //
    // [SerializeField]
    // public float maxPressTime = 0.25f;
    //
    // [SerializeField]
    // public AnimationCurve pressTimeFunction;
    //
    // private float abilityPressTimeLimit = Mathf.Infinity;
    // public float timeElapsedSinceAbilityStart => state != State.None ? Time.time - abilityStartTime : 0f;
    //
    // private float m_abilityPressDuration;
    // public float pressDuration => Mathf.Min(m_abilityPressDuration, inputButton.pressTime);
    // private float abilityPressStrength
    //     => state == State.None ? 0f :
    //         pressTimeFunction.Evaluate(Mathf.Clamp01(pressDuration / maxPressTime));
    // private float minAbilityPressStrength => pressTimeFunction.Evaluate(0f);
    //
    // /// <summary>
    // /// Current strength of this ability (on a scale of [minEffectStrength, maxEffectStrength]) 
    // /// </summary>
    // public float currentAbilityStrength
    //     => varyStrengthDependingOnPressTime
    //         ? Mathf.Lerp( minEffectStrength, maxEffectStrength, abilityPressStrength)
    //         : maxEffectStrength;
    //
    // /// <summary>
    // /// Current duration of this ability (on a scale of [minDuration, maxDuration]) 
    // /// </summary>
    // public float currentAbilityDuration
    //     => varyDurationDependingOnPressTime
    //         ? Mathf.Lerp(minDuration, maxDuration, abilityPressStrength)
    //         : maxDuration;
    //
    // private float derivedMinimumStaminaCost
    //     => varyStaminaCostDependingOnPressTime
    //         ? Mathf.Lerp(minStaminaCost, maxStaminaCost, minAbilityPressStrength)
    //         : maxStaminaCost;
    //
    // private float currentStaminaCost
    //     => varyStaminaCostDependingOnPressTime
    //         ? Mathf.Lerp(minStaminaCost, maxStaminaCost, abilityPressStrength)
    //         : maxStaminaCost;
    //
    // private float usedStamina;

    public virtual void DrawPlayerAbilityDebugGUI() {
        GUILayout.Label("current state: " + state);
        GUILayout.Label("button pressed?: " + inputButton.isPressed);
        GUILayout.Label("button pressed duration: " + inputButton.pressTime);
        // GUILayout.Label("internal min duration: " + m_abilityPressDuration);
        // GUILayout.Label("press duration: " + pressDuration);
        // GUILayout.Label("press strength: " + abilityPressStrength);
        // GUILayout.Label("min stamina cost: " + derivedMinimumStaminaCost);
        // GUILayout.Label("current stamina cost: " + currentStaminaCost);
        // GUILayout.Label("used stamina: " + usedStamina);
        // GUILayout.Label("ability duration: " + currentAbilityDuration);
        // GUILayout.Label("ability strength: " + currentAbilityStrength);
        // GUILayout.Label("elapsed time: " + timeElapsedSinceAbilityStart);
    }

    public bool drawDebugGUI = false;
    private void OnGUI() {
        if (drawDebugGUI) DrawPlayerAbilityDebugGUI();
    }

    // public bool TryStartAbility() {
    //     if (!CanStartAbility()) return false;
    //
    //     if (Time.time - abilityStartTime < abilityCooldown) {
    //         return false;
    //     }
    //     var cost = derivedMinimumStaminaCost;
    //     if (!player.TryUseAbility(this)) return false;
    //     
    //     // Reset variables
    //     abilityStartTime = Time.time;
    //     abilityPressTimeLimit = Mathf.Infinity;
    //     if (varyStaminaCostDependingOnPressTime && cost < maxStaminaCost) {
    //         // apply the rest of the cost over time while the button is still pressed
    //         usedStamina = cost;
    //     }
    //     // update state (note: if this got called from a state callback, this will short circuit and do nothing)
    //     state = PlayerAbilityState.Active;
    //     return true;
    // }
    // private void Update() {
    //     if (inputButton.wasPressedThisFrame) {
    //         TryStartAbility();
    //     }
    //     switch (m_state) {
    //         case State.None: break;
    //         case State.ActivePressed: {
    //             // special state for when ability is active + button is still pressed
    //             if (!inputButton.isPressed || inputButton.pressTime > maxPressTime) { 
    //                 // transition out of press state
    //                 m_state = State.Active;
    //                 m_abilityPressDuration = inputButton.pressTime;
    //                 
    //             // if stamina cost varies, we may have an additional unapplied stamina cost
    //             // due to pressing this button longer
    //             } else if (varyStaminaCostDependingOnPressTime && usedStamina < maxStaminaCost) {
    //                 var cost = currentStaminaCost;
    //                 // TODO: reimplement
    //                 if (cost > usedStamina && !player.TryUseAbility(this)) {
    //                 // if (cost > usedStamina && !player.TryUseAbility(cost - usedStamina)) {
    //                     // ran out of stamina: freeze ability strength here + terminate
    //                     m_state = State.Active;
    //                     m_abilityPressDuration = inputButton.pressTime;
    //                 } else {
    //                     usedStamina = cost;
    //                 }
    //             }
    //         } goto case State.Active;
    //         case State.Active: {
    //             if (timeElapsedSinceAbilityStart > currentAbilityDuration) {
    //                 state = PlayerAbilityState.Ending;
    //                 goto case State.Ending;
    //             }
    //             UpdateAbility();
    //         } break;
    //         case State.Ending:
    //             if (IsAbilityFinished()) {
    //                 state = PlayerAbilityState.None;
    //             } else {
    //                 UpdateAbility();
    //             }
    //             break;
    //     }
    // }
}
