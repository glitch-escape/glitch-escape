using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerHealth))]
public class Player : BaseAgent<Player, PlayerConfig, PlayerHealth, PlayerHealth> {

    /// <summary>
    /// Reference to this script's PlayerController (should be parented to this)
    /// </summary>
    public PlayerController controller => this.GetEnforcedComponentReferenceInParent(ref m_controller);
    private PlayerController m_controller;
    
    /// <summary>
    /// Reference to the player's rigidbody
    /// </summary>
    public new Rigidbody rigidbody => this.GetEnforcedComponentReference(ref m_rigidbody);
    private Rigidbody m_rigidbody;
    
    /// <summary>
    /// Reference to the player's animator
    /// </summary>
    public new Animator animator => this.GetEnforcedComponentReference(ref m_animator);
    private Animator m_animator;
    
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
        ResetStats();
        soundSource = GetComponent<AudioSource>();
    }
    #endregion

    #region PlayerInteractionCallbacks
    public delegate void PlayerCallback (Player player);
    
    /// <summary>
    /// Used by PlayerInteractionController
    /// </summary>
    public PlayerCallback interactListeners;
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
        } else {
            transform.position = initPosition;
            transform.rotation = initRotation;
        }
        m_rigidbody.velocity = Vector3.zero;
        ResetStats();
    }
    
    /// <summary>
    /// Respawns the player at their last registered location (calls PlayerController.RespawnPlayer())
    /// </summary>
    public void Respawn () { controller.RespawnPlayer(); }
    #endregion
    #region HealthDamageAndStaminaImplementation

    public float maxStamina = 100f;
    public float maxHealth = 100f;

    [Tooltip("stamina regen / sec")] [Range(0f, 100f)]
    public float staminaRegenPerSec = 10f;

    [Tooltip("delay after ability use before stamina begins regenerating (seconds)")] [Range(0f, 10f)]
    public float staminaRegenDelay = 0f;

    [Tooltip("health regen / sec")] [Range(0f, 100f)]
    public float healthRegenPerSec = 10f;

    [Tooltip("delay after ability use before health begins regenerating (seconds)")] [Range(0f, 10f)]
    public float healthRegenDelay = 0f;

    [Tooltip("starting stamina %")] [Range(0f, 1f)]
    public float startingStaminaPercent = 1f;

    [Tooltip("starting health %")] [Range(0f, 1f)]
    public float startingHealthPercent = 1f;

    public float lowStaminaFlashDuration = 0.5f;
    public float lowStaminaFlashCount = 1.0f;

    public AnimationCurve staminaRegenCurve;
    public AnimationCurve healthRegenCurve;
    
    private float lowStaminaFlashingDuration { get {
        return lowStaminaFlashDuration * lowStaminaFlashCount;
    } }

    public bool isHealthFlashing {
        get { return shouldFlashHealth;  }
    }
    public bool isStaminaFlashing {
        get { return shouldFlashStamina;  }
    }
    
    
    private float m_health;
    private float m_stamina;

    // public float health {
    //     get { return m_health; }
    // }
    // public float stamina {
    //     get { return m_stamina;  }
    // }
    private void ResetStats() {
        m_health = maxHealth * startingHealthPercent;
        m_stamina = maxStamina * startingStaminaPercent;
        lastStaminaUseTime = Time.time - Mathf.Max(staminaRegenDelay, 0f);
        lastTimeTookDamage = Time.time - Mathf.Max(healthRegenDelay, 0f);
        timeUntilStopFlashingHealth = Time.time;
        timeUntilStopFlashingStamina = Time.time;
    }
    private float lastStaminaUseTime = -10f;
    private float lastTimeTookDamage = -10f;
    private float timeUntilStopFlashingStamina = 0f;
    private float timeUntilStopFlashingHealth = 0f;
    private bool shouldFlashStamina {
        get { return timeUntilStopFlashingStamina > Time.time; }
    }
    private bool shouldFlashHealth {
        get { return timeUntilStopFlashingHealth > Time.time; }
    }
    
    void Update() {
        if (Time.time > lastTimeTookDamage + healthRegenDelay && m_health < maxHealth) {
            var timeToFullHealthRegen = maxHealth / healthRegenPerSec;
            var t = Mathf.Clamp01(lastTimeTookDamage / timeToFullHealthRegen);
            var healthRegen = healthRegenCurve.Evaluate(t) * healthRegenPerSec;
            m_health = Mathf.Clamp(m_health + healthRegen * Time.deltaTime, 0f, maxHealth);
        }
        if (Time.time > lastStaminaUseTime + staminaRegenDelay && m_stamina < maxStamina) {
            var timeToFullStaminaRegen = maxStamina / staminaRegenPerSec;
            var t = Mathf.Clamp01(lastStaminaUseTime / timeToFullStaminaRegen);
            var staminaRegen = staminaRegenCurve.Evaluate(t) * staminaRegenPerSec;
            m_stamina = Mathf.Clamp(m_stamina + staminaRegen * Time.deltaTime, 0f, maxStamina);
        }
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
    public void PlaySound(int soundIndex)
    {
        soundSource.PlayOneShot(soundfx[soundIndex]);
    }
    #endregion
}
