using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a collision or trigger based field that can affect the player in various ways.
///
/// Effects are implemented using field effects:
/// (DamageFieldEffect, ReduceStaminaFieldEffect, ReduceMaxHealthFieldEffect, KnockbackEffect, etc)
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlayerAffectorField : MonoBehaviour {
    
    #region PublicInterface
    public delegate void FieldEvent(Player player);

    /// <summary>
    /// Called when player enters the field
    /// </summary>
    public FieldEvent OnPlayerEnter;
    
    /// <summary>
    /// Called when player exits the field
    /// </summary>
    public FieldEvent OnPlayerExit;
    
    /// <summary>
    /// Called for each frame when player is currently in the field
    /// </summary>
    public FieldEvent OnPlayerTick;

    /// <summary>
    /// Can use this to set the current trigger type while hiding the actual collider reference
    /// </summary>
    public bool isTrigger {
        get => collider.isTrigger;
        set => collider.isTrigger = value;
    }
    
    /// <summary>
    /// active player, if they're currently inside of this field
    /// </summary>
    [HideInInspector]
    public Player activePlayer { get; private set; } = null;
    
    #endregion
    
    #region InspectorProperties
    
    /// <summary>
    /// collision type - explicitely manages switching between triggers + collision based effects
    /// could be extended to support other collision types
    /// </summary>
    public enum CollisionType { Trigger, Collider
    };
    private CollisionType collisionType;
    
    #endregion
    
    #region ImplementationDetails
    /// <summary>
    /// collider reference - hidden from other scripts
    /// </summary>
    private Collider collider => _collider ?? Enforcements.GetComponent(this, out _collider);
    private Collider _collider = null;

    void Awake() {
        // make sure our collider reference exists, and explicitely set the trigger type so it matches if necessary
        var collider = this.collider;
        if (collider.isTrigger != isTrigger) collider.isTrigger = isTrigger;
    }
    private void Update() {
        if (activePlayer != null) {
            OnPlayerTick(activePlayer);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (collisionType == CollisionType.Trigger) {
            var player = other.GetComponent<Player>();
            if (player != null) { 
                if (activePlayer) { OnPlayerExit(player); }
                OnPlayerEnter(activePlayer = player);
            }
        }
    }
    private void OnCollisionEnter(Collision other) {
        if (collisionType == CollisionType.Trigger) {
            var player = other.collider.GetComponent<Player>();
            if (player != null) {
                if (activePlayer) { OnPlayerExit(player); }
                OnPlayerEnter(activePlayer = player);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (collisionType == CollisionType.Trigger) {
            var player = other.GetComponent<Player>();
            if (player == activePlayer) {
                activePlayer = null;
                OnPlayerExit(player);
            }
        }
    }
    private void OnCollisionExit(Collision other) {
        if (collisionType == CollisionType.Trigger) {
            var player = other.collider.GetComponent<Player>();
            if (player == activePlayer) {
                activePlayer = null;
                OnPlayerExit(player);
            }
        }
    }
    #endregion
}
