using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(Animator))]
// [RequireComponent(typeof(AudioSource))]
// [RequireComponent(typeof(PlayerHealth))]
// [RequireComponent(typeof(PlayerStamina))]
public class Player : BaseAgent<Player, PlayerConfig> {
    // Agent info (abstract properties from BaseAgent)
    public override AgentType agentType => AgentType.Player;
    public override bool isTargetableBy(AgentType type) { return type != agentType; }
    protected override Resource<Player, PlayerConfig, float> healthResource => health;
    protected override Resource<Player, PlayerConfig, float> staminaResource => stamina;
    protected override KillType killType => KillType.KillAndResetAgent;
    public bool lockControls => dialog.PreventMovement();

    // object references
    [InjectComponent] public new Camera camera;

    // additional player components
    [InjectComponent] public PlayerHealth health;
    [InjectComponent] public PlayerStamina stamina;
    [InjectComponent] public PlayerControls              input;
    [InjectComponent] public PlayerSpawnController       spawn;
    [InjectComponent] public PlayerMovement    movement;
    [InjectComponent] public PlayerCameraController      cameraController;
    [InjectComponent] public PlayerAudioController       audioController;
    [InjectComponent] public PlayerDialogController      dialog;
    [InjectComponent] public PlayerMazeController        maze;
    [InjectComponent] public new PlayerAnimationController animation;
    [InjectComponent] public PlayerGravity                 gravity;
    [InjectComponent] public PlayerVirtueFragments             fragments;
    
    // player abilities
    [InjectComponent] public PlayerDashAbility           dash;
    [InjectComponent] public PlayerJumpAbility           jump;
    [InjectComponent] public PlayerInteractionAbility    interact;
    //[InjectComponent] public PlayerShootAbility          shoot;
    //[InjectComponent] public PlayerManifestAbility       manifest;
}
