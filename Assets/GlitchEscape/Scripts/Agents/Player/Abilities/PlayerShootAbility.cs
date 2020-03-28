using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerShootAbility : PlayerAbility {
    /// <summary>
    /// Stamina cost per projectile
    /// </summary>
    public override float resourceCost => player.config.shootAbilityStaminaCost;
    
    /// <summary>
    /// Shoot up to n projectiles per second when fire button is pressed repeatedly
    /// </summary>
    public override float cooldownTime => 1f / player.config.shootAbilityShotsPerSec;
    
    /// <summary>
    /// Button we use to fire a projectile
    /// </summary>
    protected override PlayerControls.HybridButtonControl inputButton => PlayerControls.instance.shoot;
    
    /// <summary>
    /// this is a one-shot ability (doesn't have a duration)
    /// </summary>
    protected override float abilityDuration => 0f;
    
    /// <summary>
    /// location (on player) to spawn the projectile at
    /// Should be a child transform on the player object w/ a FirePoint (empty marker script) on it
    /// </summary>
    [InjectComponent] public FirePoint projectileSpawnLocation;
    
    /// <summary>
    /// projectile config to use (pulls from <see cref="PlayerConfig"/> on the player object)
    /// </summary>
    private PlayerProjectileConfig projectileConfig => player.config.shootAbilityProjectile;

    /// <summary>
    /// Fires a projectile when the fire button is pressed (and ability is off cooldown)
    /// </summary>
    protected override void OnAbilityStart() {
        PlayerProjectile.Spawn(projectileConfig, projectileSpawnLocation);
    }
}
