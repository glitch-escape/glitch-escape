using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Config/HDB/EnemyConfig", order = 1)]
public class EnemyConfig : ScriptableObject {

    [Header("Movement")]
    [Range(0, 20)] public float moveSpeed = 2f;

    [Header("Health")]
    public FloatRange health = new FloatRange { minimum = 0f, maximum = 100f };
    public float healthRegen = 10f;
    public float healthRegenDelay = 0.5f;
    public AnimationCurve healthRegenCurve = new AnimationCurve
    {
        keys = new[]{
            new Keyframe(0f, 1f),
            new Keyframe(1f, 1f)
        }
    };

    [Header("Stamina")]
    public FloatRange stamina = new FloatRange { minimum = 0f, maximum = 100f };
    public float staminaRegen = 10f;
    public float staminaRegenDelay = 0.5f;
    public AnimationCurve staminaRegenCurve = new AnimationCurve
    {
        keys = new[]{
            new Keyframe(0f, 1f),
            new Keyframe(1f, 1f)
        }
    };

    [Header("Spawn Height")]
    public float spawnHeight = 1f;

    [Header("Projectile Spwan(If Any)")]
    public float projectileStaminaCost = 0f;
    public float projectileShotsPerSecond = 1f;
    public float projectileStartup = 0f;
    public float projectileCooldown = 3f;
    public float attackDuration = 5f;
    public float shootDistance = 5f;
    public EnemyProjectileConfig attackProjectile;
}