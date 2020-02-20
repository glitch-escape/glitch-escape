using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a collision or trigger based field that can affect the player in various ways.
///
/// Effects are implemented using IFieldEffects
/// (see DamageFieldEffect, ReduceStaminaFieldEffect, ReduceMaxHealthFieldEffect, KnockbackEffect)
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlayerAffectorField : MonoBehaviour {
    
    /// <summary>
    /// active player, if they're currently inside of this field
    /// </summary>
    [HideInInspector]
    public Player activePlayer { get; private set; } = null;

    /// <summary>
    /// collision type - explicitely manages switching between triggers + collision based effects
    /// could be extended to support other collision types
    /// </summary>
    public enum CollisionType { Trigger, Collider
    };
    private CollisionType collisionType;
    
    /// <summary>
    /// hide direct collider reference but let scripts explicitely set trigger type, if necessary, through this
    /// </summary>
    public bool isTrigger {
        get { return collisionType == CollisionType.Trigger; }
        set {
            collisionType = value ? CollisionType.Trigger : CollisionType.Collider;
            collider.isTrigger = value;
        }
    }
    
    /// <summary>
    /// collider reference - hidden from other scripts
    /// </summary>
    private Collider collider => _collider ?? Enforcements.GetComponent(this, out _collider);
    private Collider _collider = null;
    
    /// <summary>
    /// array of field effect impls - hidden from other scripts
    /// </summary>
    private IFieldEffect[] effects => _effects ?? Enforcements.GetComponents(this, out _effects);
    private IFieldEffect[] _effects = null;

    void Awake() {
        // make sure our collider reference exists, and explicitely set the trigger type so it matches if necessary
        var collider = this.collider;
        if (collider.isTrigger != isTrigger) collider.isTrigger = isTrigger;
        
        // call SetupFieldBarrier() on all scripts
        foreach (var effect in effects) {
            effect.SetupField(this);
        }
    }

    private void OnPlayerExit(Player player) {
        foreach (var effect in effects) {
            effect.OnPlayerExit(player);
        }
    }
    private void OnPlayerEnter(Player player) {
        foreach (var effect in effects) {
            effect.OnPlayerEnter(player);
        }
    }
    private void Update() {
        if (activePlayer != null) {
            foreach (var effect in effects) {
                effect.OnPlayerTick(activePlayer);
            }
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
}
