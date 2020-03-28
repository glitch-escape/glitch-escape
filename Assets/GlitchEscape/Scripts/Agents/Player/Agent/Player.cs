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

    #region AgentProperties
    public override AgentType agentType => AgentType.Player;
    public override bool isTargetableBy(AgentType type) { return type != agentType; }
    protected override Resource<Player, PlayerConfig, float> healthResource => health;
    protected override Resource<Player, PlayerConfig, float> staminaResource => stamina;
    protected override KillType killType => KillType.KillAndResetAgent;
    #endregion

    // references to components on the player object
    [InjectComponent] public new Rigidbody rigidbody;
    [InjectComponent] public Animator animator;
    [InjectComponent] public new Camera camera;
    [InjectComponent] public AudioSource soundSource;
    [InjectComponent] public PlayerHealth health;
    [InjectComponent] public PlayerStamina stamina;
    
    // additional player components
    [InjectComponent] public PlayerControls              input;
    [InjectComponent] public PlayerSpawnController       spawn;
    [InjectComponent] public PlayerMovementController    movement;
    [InjectComponent] public PlayerCameraController      cameraController;
    [InjectComponent] public PlayerAudioController       audioController;
    [InjectComponent] public PlayerDialogController      dialog;
    [InjectComponent] public PlayerMazeController        maze;
    
    // player abilities
    [InjectComponent] public PlayerDashAbility           dash;
    [InjectComponent] public PlayerJumpAbility           jump;
    [InjectComponent] public PlayerInteractionAbility     interact;
    [InjectComponent] public PlayerShootAbility          shoot;
    [InjectComponent] public PlayerManifestAbility       manifest;
}
