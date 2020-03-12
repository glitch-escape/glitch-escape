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
public class Player : BaseAgent<Player, PlayerConfig, PlayerHealth, PlayerStamina> {

    /// <summary>
    /// Reference to this script's PlayerController (should be parented to this)
    /// </summary>
    public PlayerController controller => this.GetEnforcedComponentReferenceInParent(ref m_controller);
    private PlayerController m_controller;

    [InjectComponent] public Rigidbody rigidbody;
    [InjectComponent] public Animator animator;
    [InjectComponent] public PlayerHealth health;
    [InjectComponent] public PlayerStamina stamina;
    
    /// <summary>
    /// Reference to the player's rigidbody
    /// </summary>
    /// 
    // public new Rigidbody rigidbody => this.GetEnforcedComponentReference(ref m_rigidbody);
    //
    // private Rigidbody m_rigidbody;

    /// <summary>
    /// Reference to the player's animator
    /// </summary>
    // public new Animator animator => this.GetEnforcedComponentReference(ref m_animator);
    //
    // private Animator m_animator;

    // input instance singleton
    public Input input => m_input ?? (m_input = new Input());
    private Input m_input;

    //audio management variables
    public AudioClip[] soundfx;
    public AudioSource soundSource;

    new void OnEnable() {
        base.OnEnable();
    }

    new void OnDisable() {
        base.OnDisable();
    }

    /// <summary>
    /// Called by BaseAgent when this player should "die"
    /// Returning false from this function cancels the despawn, and we manually "respawn" the player instead
    /// </summary>
    /// <returns></returns>
    protected override bool TryKillAgent() {
        controller.RespawnPlayer();
        PlaySound(4);
        return false;
    }

    #region UnityUpdateAndAwake

    void Awake() {
        input.Enable();
        SetInitialSpawnLocation(transform.position, transform.rotation);
        soundSource = GetComponent<AudioSource>();
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
