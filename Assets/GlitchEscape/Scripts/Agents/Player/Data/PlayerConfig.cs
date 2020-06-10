using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Player/PlayerConfig", order = 1)]
public class PlayerConfig : ScriptableObject {

    [Header("Controls")] 
    public AnimationCurve gamepadInputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve mouseInputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public float mouseSensitivity = 1f;

    [Header("Gravity")] 
    public float gravity = 9.81f;

    [Header("Movement")]
    [Tooltip("Player movement speed (meters / second)")]
    [Range(0, 20)] public float runSpeed = 10f;
    [Tooltip("Player movement speed (degrees / second)")]
    public float turnSpeed;
    
    [Tooltip("Camera turn speed, in degrees / sec")]
    [Range(5f, 360)] public float cameraTurnSpeed = 180f;

    [Header("Interaction")] 
    public float interactionRadius = 10f;

    #region PlayerRaycasts

    [Header("Raycasts")] 
    public float playerRayDistanceToGround = 0.1f;
    public float wallRaycastDistance = 1f;
    #endregion
    
    #region PlayerJumpAbility
    [Header("Jump ability")] 
    
    [Tooltip("enables / disables the player's jumping ability")]
    public bool canJump = true;
    
    [Tooltip("if false, player cannot jump while in the air (effectively disables maxJumps)")]
    public bool canAirJump = true;
    
    [Tooltip("if false, player cannot wall jump and wall jumping will not reset the player's jump count")]
    public bool canWallJump = true;
    
    [Tooltip("Peak height (meters) that player will jump to when jump is pressed")]
    [Range(0, 20)] public float jumpHeight = 10f;
    
    [Tooltip("Max number of times that player can jump consecutively before needing to touch ground")]
    [Range(0, 10)] public int maxJumps = 2;

    [Tooltip("How long of a window the player has after leaving a patform to still floor jump")]
    [Range(0, 1)] public float floorJumpWindow = 0.3f;

    [Tooltip("if false, disables the effects of downGravityMultiplier + upGravityModifier")]
    public bool useGravityModifications = true;

    [Tooltip("changes the player's fall speed (multiplies gravity while falling by this factor)")]
    public float downGravityMultiplier = 2.2f;
    
    [Tooltip("changes the player's fall speed (multiplies gravity while jumping upwards by this factor)")]
    public float upGravityMultiplier = 0.8f;
    
    [Tooltip("determines the force (multiply this by normal jump force) that the player uses to push off of walls")]
    public float wallJumpMultiplier = 1.5f;
    #endregion PlayerJumpAbility
    
    
    [Header("Dash ability")]
    public FloatRange dashAbilityPressTimeRange = new FloatRange {minimum = 0.1f, maximum = 0.3f};
    public AnimationCurve dashAbilityPressCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public FloatRange dashAbilityStaminaCostRange = new FloatRange{minimum = 30f, maximum = 45f};
    public FloatRange dashAbilityMoveRange = new FloatRange {minimum = 3f, maximum = 8f};
    public float dashAbilitySpeed = 40f;
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

    [Header("Fragments in level")]
    public int fragmentTotal = 10;
    
    [Header("Rotation puzzle config(s)")]
    public RotationPuzzleConfig rotationPuzzleConfig;

    [Header("Maze fadeout")] public Material[] glitchMazeMaterials;
    
    [Header("Debug")]
    public bool enableLevelDebugNavTools = true;
}
