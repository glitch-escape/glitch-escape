using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

[RequireComponent(typeof(PlayerManager), typeof(PlayerStatsView))]
public class PlayerStats : MonoBehaviour {

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

    public float health {
        get { return m_health; }
    }
    public float stamina {
        get { return m_stamina;  }
    }

    private PlayerManager m_playerControls;
    void Start() {
        m_playerControls = GetComponent<PlayerManager>();
        if (m_playerControls == null) {
            Debug.LogWarning("player stats component is missing player controls component!");
        }
        Reset();
    }
    private void Reset() {
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

    public bool TryUseAbility(float staminaCost) {
        if (staminaCost <= m_stamina) {
            m_stamina -= staminaCost;
            lastStaminaUseTime = Time.time;
            return true;
        } else {
            FlashLowStamina();
            return false;
        }
    }
    public void TakeDamage(float damage) {
        m_health -= damage;
        lastTimeTookDamage = Time.time;
        if (m_health <= 0f) {
            KillPlayer();
        }
    }
    public void KillPlayer() {
        Reset();
        m_playerControls.Respawn();
    }
    void Update() {
        if (Time.time > lastTimeTookDamage + healthRegenDelay && m_health < maxHealth) {
            m_health = Mathf.Clamp(m_health + healthRegenPerSec * Time.deltaTime, 0f, maxHealth);
        }
        if (Time.time > lastStaminaUseTime + staminaRegenDelay && m_stamina < maxStamina) {
            m_stamina = Mathf.Clamp(m_stamina + staminaRegenPerSec * Time.deltaTime, 0f, maxStamina);
        }
    }
    private void FlashLowStamina() {
        timeUntilStopFlashingStamina = Time.time + lowStaminaFlashingDuration;
    }
    // private void FlashLowHealth () {
    //     timeUntilStopFlashingHealth = Time.time + lowHealthFlashDuration;
    // }
}
