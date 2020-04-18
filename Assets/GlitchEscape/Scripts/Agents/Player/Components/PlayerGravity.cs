using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements player gravity.
/// Player gravity can be disabled by disabling this component or setting gravityEnabled = false.
/// Provides properties to change gravity direction + strength, which could be used to implement
/// various game effects, game mechanics, and player abilities.
/// TODO: 1) add capability to change player orientation along w/ gravity (possibly in PlayerMovement...?)
/// TODO: 2) add an effects system to implement gravity effects that temporarily change gravity properties
/// TODO: and both restore prev state AND support multiple effects simultaneously
/// </summary>
public class PlayerGravity : PlayerComponent {
    [InjectComponent] public PlayerMovementController playerMovement;

    /// <summary>
    /// Enables / disables gravity
    /// </summary>
    public bool gravityEnabled = true;

    /// <summary>
    /// Gravity multiplier - can be used to increase or decrease gravity
    /// </summary>
    public float gravityStrength = 1f;

    /// <summary>
    /// Gravity direction, relative to player
    /// </summary>
    public Vector3 gravityDirection = Vector3.down;
    
    /// <summary>
    /// Current gravity value, in meters / sec^2
    /// Changes so player has a different effective gravity when falling - used to
    /// make jumping (ie. player has y-axis velocity + is not falling) balanced while
    /// still falling quickly
    /// </summary>
    public float gravity =>
        gravityEnabled
            ? gravityStrength * (playerMovement.isFalling
                ? Mathf.Abs(Physics.gravity.y) * player.config.downGravityMultiplier
                : Mathf.Abs(Physics.gravity.y) * player.config.upGravityMultiplier)
            : 0f;

    /// <summary>
    /// Applies gravity each time step, if it is enabled
    /// </summary>
    private void FixedUpdate() {
        if (gravityEnabled) {
            playerMovement.ApplyAcceleration(gravity * Time.deltaTime * gravityDirection);
        }
        if (gravityStrength <= 0f) {
            Debug.LogWarning("Zero or negative gravity strength on "+this+": "+gravityStrength+"!");
        }
        if (Mathf.Abs(gravityDirection.magnitude) - 1f > 1e-6f) {
            Debug.LogWarning("Gravity direction is non-normalized and may be zero! on "
                             +this+": "+gravityDirection);
        }
    }
}
