using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractionTrigger : MonoBehaviour {
    /// <summary>
    /// Reference to the active / detected player object, if they're in interaction radius
    /// </summary>
    private Player activePlayerInInteractionRadius = null;
    
    /// <summary>
    /// Is player currently in the interaction radius?
    /// </summary>
    public bool playerInInteractionRadius => activePlayerInInteractionRadius != null;

    public enum InteractionTriggerType {
        PhysicsTrigger,
        PhysicsCollision,
    }

    public enum AffectorMode {
        AffectSelf,
        AffectSelfOrChildren,
        AffectSelfOrParent
    }
    public AffectorMode affectorMode;

    /// <summary>
    /// Mechanism that we use to detect the player: typically a physics OnTriggerEnter/Exit or OnCollisionEnter/Exit
    /// To add more trigger types, see Interactable.InteractionTriggerType
    /// </summary>
    public InteractionTriggerType interactionTriggerType = InteractionTriggerType.PhysicsTrigger;

    /// <summary>
    /// Interaction handlers on this object (or its children), implemented via classes implementing
    /// IPlayerInteractable
    /// </summary>
    private IActiveInteract[] attachedInteractionHandlers;

    public void Start() {
        switch (affectorMode) {
            case AffectorMode.AffectSelf:
                attachedInteractionHandlers = Enforcements.GetComponents<IActiveInteract>(this);
                break;
            case AffectorMode.AffectSelfOrChildren:
                attachedInteractionHandlers = Enforcements.GetComponentsInChildren<IActiveInteract>(this);
                break;
            case AffectorMode.AffectSelfOrParent:
                attachedInteractionHandlers = Enforcements.GetComponentsInParent<IActiveInteract>(this);
                break;
        }
        if (attachedInteractionHandlers.Length == 0) {
            Debug.LogError("Warning: InteractTrigger on "+gameObject+" has no interaction triggers!");
        }
    }
    
    /// <summary>
    /// Track when OnEnable() called (ie. script reload or toggled on / off)
    /// </summary>
    private bool onEnableCalledBeforeLastTriggerInteraction = false;
    private void OnEnable() {
        onEnableCalledBeforeLastTriggerInteraction = true; 
    }
    private void OnDisable() {
        if ((PlayerControls.instance?.interact ?? null) != null) {
            PlayerControls.instance.interact.onPressed -= OnInteractPressed;
        }
    }

    private PlayerControls controls;
    private bool hasListener = false;

    private void EnableInteractListener() {
        Debug.Log("starting interaction listener: "+this.gameObject);
        controls = PlayerControls.instance;
        if (!hasListener) {
            hasListener = true;
            controls.interact.onPressed += OnInteractPressed;
        }
    }
    private void DisableInteractListener() {
        Debug.Log("Stopping interaction listener: "+this.gameObject);
        if (hasListener == false || controls == null) return;
        hasListener = false;
        controls.interact.onPressed -= OnInteractPressed;
    }
    private void OnPlayerEnterInteractionRadius(Player player) {
        if (attachedInteractionHandlers.Length == 0) return;
        foreach (var handler in attachedInteractionHandlers) {
            handler?.OnPlayerEnterInteractionRadius(player);
        }
    }
    private void OnPlayerLeaveInteractionRadius(Player player) {
        if (attachedInteractionHandlers.Length == 0) return;
        foreach (var handler in attachedInteractionHandlers) {
            handler?.OnPlayerExitInteractionRadius(player);
        }
    }
    public void OnPlayerInteractPressed(Player player) {
        if (attachedInteractionHandlers.Length == 0) return;
        foreach (var handler in attachedInteractionHandlers) {
            handler?.OnInteract(player);
        }
    }
    private void OnInteractPressed() {
        Debug.Log("interact pressed!");
        if (activePlayerInInteractionRadius != null) {
            OnPlayerInteractPressed(activePlayerInInteractionRadius);
        }   
    }
    
    #region TriggerAndCollisionHandlers
    /// <summary>
    /// Detects player via presence of a Player script on the collider's attached object or one of its parents.
    /// Does not check for the existence of a Player tag.
    /// </summary>
    void OnTriggerEnter(Collider collider) {
        if (interactionTriggerType != InteractionTriggerType.PhysicsTrigger) return;
        var player = collider.GetComponentInParent<Player>();
        if (player != null && (!playerInInteractionRadius || onEnableCalledBeforeLastTriggerInteraction)) {
            activePlayerInInteractionRadius = player;
            EnableInteractListener();
            OnPlayerEnterInteractionRadius(player);
        }
        onEnableCalledBeforeLastTriggerInteraction = false;
    }
    void OnTriggerExit(Collider collider) {
        if (interactionTriggerType != InteractionTriggerType.PhysicsTrigger) return;
        var player = collider.GetComponentInParent<Player>();
        if (player == activePlayerInInteractionRadius) {
            activePlayerInInteractionRadius = null;
            DisableInteractListener();
            OnPlayerLeaveInteractionRadius(player);
        }
        onEnableCalledBeforeLastTriggerInteraction = false;
    }
    private void OnCollisionEnter(Collision other) {
        if (interactionTriggerType != InteractionTriggerType.PhysicsCollision) return;
        var player = other.collider.GetComponentInParent<Player>();
        if (player != null && (!playerInInteractionRadius || onEnableCalledBeforeLastTriggerInteraction)) {
            activePlayerInInteractionRadius = player;
            EnableInteractListener();
            OnPlayerEnterInteractionRadius(player);
        }
        onEnableCalledBeforeLastTriggerInteraction = false;
    }
    private void OnCollisionExit(Collision other) {
        if (interactionTriggerType != InteractionTriggerType.PhysicsCollision) return;
        var player = other.collider.GetComponentInParent<Player>();
        if (player == activePlayerInInteractionRadius) {
            activePlayerInInteractionRadius = null;
            DisableInteractListener();
            OnPlayerLeaveInteractionRadius(player);
        }
        onEnableCalledBeforeLastTriggerInteraction = false;
    }
    #endregion TriggerAndCollisionHandlers
}
