using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface that all player controller subcomponents must implement
// Use SetupControllerComponent() instead of Awake() to get player object references
// in a deterministic manner.
public interface IPlayerControllerComponent {
    void SetupControllerComponent(PlayerController controller);
}

// Central player controller script, with behaviors broken into components (see IPlayerControllerComponent)
// This script should not be attached to the actual player object (the pawn), but to a root object w/
// children including the player, camera, UI elements, etc.
//
// As such, the PlayerController should never be destroyed (the object that the Player script is attached to can be).,
// and this is structured this way so that a full scene can be setup (and changes to the full player rig can be made
// across all scenes) by just placing / editing a single prefab containing the player, camera, UI, etc.
//
[RequireComponent(typeof(PlayerMazeSwitchController))]
[RequireComponent(typeof(PlayerControls))]
public class PlayerController : MonoBehaviour {
    
    // The actual player object (external to this object)
    public Player player;

    // The active camera
    public new Camera camera;

    [Tooltip("location to respawn the player. if null, will default to the player's starting position / rotation")]
    public Transform savePointLocation;
    
    // Maze switcher
    public PlayerMazeSwitchController playerMazeSwitcher { get; private set; } = null;

    public void SwitchMazes() {
        playerMazeSwitcher.SwitchMazes();        
    }
    public void SetSavePoint(Transform location) {
        savePointLocation = location;
    }
    public void RespawnPlayer() {
        playerMazeSwitcher.SetMazeActive(PlayerMazeSwitchController.ActiveMaze.Default);
        player.RespawnAt(savePointLocation);
        
        OnEnable();

        var animator = player.animator;
        animator.SetBool("isRunning", false);
        animator.SetBool("isDashing", false);
        animator.SetBool("isJumping", false);
        
        // reset any scripts that may need resets
        foreach (var script in GetComponentsInChildren<IResettable>()) {
            script.Reset();
        }
    }

    private bool isEnabled = false;

    void Awake() {
        // Get all references
        if (!player) { Debug.LogError("PlayerController: Player reference missing!"); }
        if (!camera) { Debug.LogError("PlayerController: Camera reference missing!"); }
        if (!camera) { camera = Camera.current; }
        playerMazeSwitcher = GetComponent<PlayerMazeSwitchController>();

        // setup player's controller reference
        // player.controller = this;

        isEnabled = true;
        SetupSubControllers(player.GetComponents<IPlayerControllerComponent>());
        SetupSubControllers(GetComponents<IPlayerControllerComponent>());
    }
    void OnEnable() {
        player.input.Enable();
        if (!isEnabled) {
            isEnabled = true;
            // Setup our sub-controllers on both this object and the player object
            SetupSubControllers(player.GetComponents<IPlayerControllerComponent>());
            SetupSubControllers(GetComponents<IPlayerControllerComponent>());
        }
    }

    private void OnDisable() {
        isEnabled = false;
        player.input.Disable();
    }

    private void SetupSubControllers(IPlayerControllerComponent[] components) {
        foreach (var component in components) {
            component.SetupControllerComponent(this);
        }
    }
}
