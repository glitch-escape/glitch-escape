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

    //audio management variables
    public AudioClip[] soundfx;

    void OnEnable() {}
    void OnDisable() {}

    #region UnityUpdateAndAwake

    void Awake() {
        input.Enable();
        SetInitialSpawnLocation(transform.position, transform.rotation);
    }

    #endregion

    #region PlayerRespawnAtImplementation

    // save initial position + rotation for player respawns
    private Vector3 initPosition;
    private Quaternion initRotation;
    public float playerSpawnHeight = 1f;

    /// <summary>
    /// Sets initial spawn location that RespawnAt() uses.
    /// Shouldn't need to call this externally, but it's here if / as needed.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    public void SetInitialSpawnLocation(Vector3 pos, Quaternion rot) {
        initPosition = pos;
        initRotation = rot;
    }

    /// <summary>
    /// Respawn player at a specific transform location, or their starting location if the passed in transform is null.
    /// Used by PlayerController.RespawnPlayer(), which handles tracking spawn locations.
    /// </summary>
    /// <param name="savePoint">Respawn location. If null, player respawns at their starting position / rotation</param>
    public void RespawnAt(Transform savePoint) {
        if (savePoint) {
            transform.position = savePoint.position + Vector3.up * playerSpawnHeight;
            transform.rotation = initRotation;
        }
        else {
            transform.position = initPosition;
            transform.rotation = initRotation;
        }

        rigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// Respawns the player at their last registered location (calls PlayerController.RespawnPlayer())
    /// </summary>
    public void Respawn() {
        controller.RespawnPlayer();
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

    #region Audio

    public void PlaySound(int soundIndex) {
        soundSource.PlayOneShot(soundfx[soundIndex]);
    }

    #endregion
}
