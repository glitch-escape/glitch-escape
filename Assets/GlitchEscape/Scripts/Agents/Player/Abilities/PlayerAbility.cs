using System;
using System.Collections;
using System.Collections.Generic;
using GlitchEscape.Scripts.DebugUI;
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

    /// <summary>
    /// overridden to let abilities be locked based on player variable
    /// </summary>
    public override bool canUseAbility => !isOnCooldown && CanStartAbility() && !player.lockControls;
    
    protected override void Update() {
        base.Update();
        if (inputButton?.wasPressedThisFrame ?? false) {
            UseAbility();
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
}
