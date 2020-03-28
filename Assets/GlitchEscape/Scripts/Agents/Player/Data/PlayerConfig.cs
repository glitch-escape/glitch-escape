using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Player/PlayerConfig", order = 1)]
public class PlayerConfig : ScriptableObject {

    [Header("Controls")] 
    public AnimationCurve gamepadInputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve mouseInputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public float mouseSensitivity = 1f;

    [Header("Movement")]
    [Tooltip("Player movement speed (meters / second)")]
    [Range(0, 20)] public float runSpeed = 10f;
    [Tooltip("Player movement speed (degrees / second)")]
    public float turnSpeed;
    
    [Tooltip("Camera turn speed, in degrees / sec")]
    [Range(5f, 360)] public float cameraTurnSpeed = 180f;

    [Header("Interaction")] 
    public float interactionRadius = 10f;

    [Header("Jump ability")] [Range(0, 20)]
    public float jumpHeight = 10f;
    [Range(0, 10)] public int maxJumps = 2;
    public bool canJump = true;
    public bool canAirJump = true;
    public bool canWallJump = true;

    [Header("Dash ability")]
    public FloatRange dashAbilityPressTimeRange = new FloatRange {minimum = 0.1f, maximum = 0.3f};
    public AnimationCurve dashAbilityPressCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public FloatRange dashAbilityStaminaCostRange = new FloatRange{minimum = 30f, maximum = 45f};
    public FloatRange dashAbilityMoveRange = new FloatRange {minimum = 3f, maximum = 8f};
    public FloatRange dashAbilityDurationRange = new FloatRange {minimum = .8f, maximum = 2.5f};
    public float dashAbilityCooldownTime = 0.05f;
    public bool dashAbilityEnabled = true;

    [Header("Shoot ability")] 
    public float shootAbilityStaminaCost = 10f;
    public float shootAbilityShotsPerSec = 3f;
    public float shootAbilityProjectileSpeed = 30f;
    public float shootAbilityProjectileDamage = 10f;
    public bool shootAbilityEnabled = true;
    public PlayerProjectileConfig shootAbilityProjectile;

    [Header("Manifest ability")] 
    public float manifestAbilityStaminaCost = 20f;
    public float manifestAbilityCooldownTime = 0.2f;
    public float manifestAbilityShieldDuration = 3f;
    public float manifestAbilityShieldDistanceFromPlayer = 1f;
    // public ManifestShield manifestAbilityShieldPrefab;
    public GameObject manifestAbilityShieldPrefab;

    [Header("Health")] 
    public FloatRange health = new FloatRange {minimum = 0f, maximum = 100f};
    public float healthRegen = 10f;
    public float healthRegenDelay = 0.5f;
    public AnimationCurve healthRegenCurve = AnimationCurve.Constant(0f, 1f, 1f);
    
    [Header("Stamina")] 
    public FloatRange stamina = new FloatRange {minimum = 0f, maximum = 100f};
    public float staminaRegen = 10f;
    public float staminaRegenDelay = 0.5f;
    public AnimationCurve staminaRegenCurve = AnimationCurve.Constant(0f, 1f, 1f);

    [Header("Spawn Height")] 
    public float spawnHeight = 1f;
}
