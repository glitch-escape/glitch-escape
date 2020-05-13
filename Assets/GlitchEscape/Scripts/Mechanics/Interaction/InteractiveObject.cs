using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interface for InteractiveObject (lists methods you should override)
/// Implemented by <see cref="AInteractiveObject"/>
/// </summary>
public interface IInteractiveObject {
    void OnInteract(Player player);
    void OnFocusChanged(bool focused);
}

/// <summary>
/// Interactive object that forwards callbacks to unity events.
/// To implement an interactive object with a single script, inheirt from <see cref="AInteractiveObject"/>.
/// For an example of object interaction, see the <see cref="InteractiveObjectTest"/> script and InteractionTest.unity.
/// </summary>
[RequireComponent(typeof(Collider))]
public class InteractiveObject : AInteractiveObject {
    [Tooltip("triggered by OnInteract()")]
    public UnityEvent onInteract;
    [Tooltip("triggered by OnFocusChanged(true)")]
    public UnityEvent onHighlightBegin;
    [Tooltip("triggered by OnFocusChanged(false)")]
    public UnityEvent onHighlightEnd;

    public override void OnInteract(Player player) {
        onInteract.Invoke();
    }
    public override void OnFocusChanged(bool focused) {
        if (focused) {
            onHighlightBegin.Invoke();
        } else {
            onHighlightEnd.Invoke();
        }
    }
}

/// <summary>
/// Implements a script that manages object interaction for rapid prototyping, etc.
/// To use this, either:
/// 1) inheirit from <see cref="AInteractiveObject"/> and implement the methods in <see cref="IInteractiveObject"/>
///   as public override methods
/// 2) use <see cref="InteractiveObject"/> and use the unity events there to link callbacks, etc.
///
/// To debug an interactive object, you can add a <see cref="InteractiveObjectTest"/> script.
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class AInteractiveObject : MonoBehaviour, IInteractiveObject {
    /// <summary>
    /// Describes conditions for triggering an "interact" event (calls <see cref="AInteractiveObject.OnInteract()"/>)
    /// </summary>
    public enum InteractTriggerType {
        /// <summary>
        /// Interaction is disabled
        /// </summary>
        None,
        
        /// <summary>
        /// Interaction will be triggered when a <see cref="Player"/> is within trigger bounds and the player presses
        /// the interact button
        /// </summary>
        PlayerPressedInteract,
        
        /// <summary>
        /// Interaction will be triggered when player first enters a physics trigger attached to this object
        /// </summary>
        PlayerEnteredTrigger,
        
        /// <summary>
        /// Interaction will be triggered when player hits a physics collider (non-trigger) attached to this object 
        /// </summary>
        PlayerHitCollider,
    }

    /// <summary>
    /// Describes conditions for triggering a focus change event (calls <see cref="AInteractiveObject.OnFocusChanged(bool)"/>)
    /// </summary>
    public enum HighlightTriggerType {
        /// <summary>
        /// Object does not gain or lose focus
        /// </summary>
        None,
        
        /// <summary>
        /// Object becomes focused when player enters the trigger, and is lost when player exits the trigger
        /// </summary>
        PlayerEnteredTrigger,
    }
    [Tooltip("condition for triggering OnInteract()")]
    public InteractTriggerType interactTrigger;
    [Tooltip("condition for triggering OnFocusChanged(bool focused)")]
    public HighlightTriggerType highlightTrigger;
    
    // helper functions / getters
    private bool interactOnTriggerEnter => interactTrigger == InteractTriggerType.PlayerEnteredTrigger;
    private bool interactOnCollisionEnter => interactTrigger == InteractTriggerType.PlayerHitCollider;
    private bool interactOnPressed => interactTrigger == InteractTriggerType.PlayerPressedInteract;
    private bool highlightOnEnterExitTrigger => highlightTrigger == HighlightTriggerType.PlayerEnteredTrigger;
    
    /// <summary>
    /// Called when player interacts with something (must be implemented by derived class)
    /// </summary>
    public abstract void OnInteract(Player player);
    
    /// <summary>
    /// Called when this object is "focused" (ie. player is within trigger radius; must be impl by derived class)
    /// </summary>
    /// <param name="focused"></param>
    public abstract void OnFocusChanged(bool focused);
    
    /// <summary>
    /// Tracks an active player ref; used to implement behavior for <see cref="InteractTriggerType.PlayerPressedInteract"/>
    /// </summary>
    public Player activePlayer;
    
    private void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<Player>();
        if (player != null) {
            if (interactOnTriggerEnter) {
                OnInteract(player);
            }
            if (interactOnPressed) {
                activePlayer = player;
            }
            if (highlightOnEnterExitTrigger) {
                OnFocusChanged(true);
            }
        }
    }
     private void OnTriggerExit(Collider other) {
        var player = other.GetComponent<Player>();
        if (player != null) {
            if (interactOnPressed || activePlayer != null) {
                activePlayer = null;
            }
            if (highlightOnEnterExitTrigger) {
                OnFocusChanged(false);
            }
        }
    }
    private void OnCollisionEnter(Collision other) {
        var player = other.gameObject.GetComponent<Player>();
        if (player != null) {
            if (interactOnCollisionEnter) {
                OnInteract(player);
            }
        }
    }

    private int interactPressCount = 0;
    protected void Update() {
        if (interactOnPressed && (activePlayer?.input.interact.wasPressedThisFrame ?? false)) {
            ++interactPressCount;
            OnInteract(activePlayer);
        }
    }

    public bool showDebugGUI = false;
    private void OnGUI() {
        if (showDebugGUI) {
            GUILayout.Label("interactOnPressed? "+interactOnPressed);
            GUILayout.Label("activePlayer? "+activePlayer);
            GUILayout.Label("interact pressed this frame? "+(activePlayer?.input.interact.wasPressedThisFrame ?? false));
            GUILayout.Label("interact press count: "+interactPressCount);
        }
    }
}
