﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "GameConfigs/PlayerConfig", order = 1)]
public class PlayerConfig : ScriptableObject {

    [Header("Movement")]
    [Tooltip("Player movement speed (meters / second)")]
    [Range(0, 20)] public float runSpeed = 10f;
    [Tooltip("Player movement speed (degrees / second)")]
    public float turnSpeed;
    
    [Tooltip("Camera turn speed, in degrees / sec")]
    [Range(5f, 360)] public float cameraTurnSpeed = 180f;

    [Header("Jump ability")] [Range(0, 20)]
    public float jumpHeight = 10f;

    [Range(0, 10)] public int maxJumps = 2;
    public bool canJump = true;
    public bool canAirJump = true;
    public bool canWallJump = true;

    [Header("Dash ability")] 
    public bool canDash = true;
    public FloatRange dashDistance;

    [Header("Health")] 
    public FloatRange health = new FloatRange {minimum = 0f, maximum = 100f};
    public float healthRegen = 10f;
    public float healthRegenDelay = 0.5f;
    public AnimationCurve healthRegenCurve = new AnimationCurve {
        keys = new []{
            new Keyframe(0f, 1f),
            new Keyframe(1f, 1f)
        }
    };
    
    [Header("Stamina")] 
    public FloatRange stamina = new FloatRange {minimum = 0f, maximum = 100f};
    public float staminaRegen = 10f;
    public float staminaRegenDelay = 0.5f;
    public AnimationCurve staminaRegenCurve = new AnimationCurve {
        keys = new []{
            new Keyframe(0f, 1f),
            new Keyframe(1f, 1f)
        }
    };

    [Header("Spawn Height")] 
    public float spawnHeight = 1f;
}
