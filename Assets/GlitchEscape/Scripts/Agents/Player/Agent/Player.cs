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
    
    /// <summary>
    /// Reference to this script's PlayerController (should be parented to this)
    /// </summary>
    public PlayerController controller => this.GetEnforcedComponentReferenceInParent(ref m_controller);
    private PlayerController m_controller;

    // references to components on the player object
    [InjectComponent] public new Rigidbody rigidbody;
    [InjectComponent] public Animator animator;
    [InjectComponent] public AudioSource soundSource;
    [InjectComponent] public PlayerHealth health;
    [InjectComponent] public PlayerStamina stamina;
    
    // additional player components
    [InjectComponent] public PlayerSpawnController       spawn;
    [InjectComponent] public PlayerMovementController    movement;
    [InjectComponent] public PlayerAudioController       audioController;
    [InjectComponent] public PlayerDialogController      dialog;

    // player abilities
    [InjectComponent] public PlayerDashController        dash;
    [InjectComponent] public PlayerJumpController        jump;
    [InjectComponent] public PlayerMazeSwitchController  mazeSwitch;
    [InjectComponent] public PlayerInteractionController interact;
    
    // input instance singleton
    public Input input => m_input ?? (m_input = new Input());
    private Input m_input;

    #region UnityUpdateAndAwake

    void Awake() {
        input.Enable();
    }

    #endregion
    
    #region MazeSwitchImplementation

    /// <summary>
    /// Returns true if the player is currently standing on a maze switch
    /// </summary>
    public bool canMazeSwitch => activeMazeSwitch != null;

    private MazeSwitch activeMazeSwitch = null;

    /// <summary>
    /// Called by a MazeSwitch script to register itself as an active maze switch.
    /// Expects SetMazeSwitch() called in response
    /// </summary>
    public void SetActiveMazeSwitch(MazeSwitch activeSwitch) {
        if (activeMazeSwitch != null) {
            activeMazeSwitch.SetMazeSwitchActive(false);
        }

        activeMazeSwitch = activeSwitch;
        activeSwitch.SetMazeSwitchActive(true);
    }

    /// <summary>
    /// Called by a MazeSwitch script to clear itself from being an active maze switch.
    /// Expects SetMazeSwitch() called in response
    /// </summary>
    /// <param name="activeSwitch"></param>
    public void ClearActiveMazeSwitch(MazeSwitch activeSwitch) {
        if (activeMazeSwitch == activeSwitch) {
            activeMazeSwitch = null;
        }

        activeSwitch.SetMazeSwitchActive(false);
    }

    #endregion
}
