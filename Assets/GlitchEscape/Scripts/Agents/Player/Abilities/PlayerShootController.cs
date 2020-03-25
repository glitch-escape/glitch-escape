using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerShootController : PlayerAbility {
    public override float resourceCost => player.config.shootAbilityStaminaCost;
    public override float cooldownTime => 1f / player.config.shootAbilityShotsPerSec;
    protected override PlayerControls.HybridButtonControl inputButton => PlayerControls.instance.shoot;
    
    public OldEnemyProjectile oldEnemyProjectilePrefab;
    public Transform projectileSpawnLocation;
    
    protected override void AbilityStart() { SpawnProjectile(); }
    protected override void AbilityEnd() {}
    protected override void AbilityUpdate() {}
    protected override void ResetAbility() {}
    protected override bool IsAbilityFinished() {
        return elapsedTime > currentAbilityDuration;
    }
    private void SpawnProjectile() {
        Vector3 direction = projectileSpawnLocation.forward;
        Vector3 origin = projectileSpawnLocation.position;
        // Quaternion quat = Quaternion.LookRotation(direction, Vector3.up);
        OldEnemyProjectile bullet = Instantiate(
            oldEnemyProjectilePrefab,
            projectileSpawnLocation.position,
            projectileSpawnLocation.rotation);
        Debug.Log("Spawned projectile: "+bullet.transform.position+ ", " + bullet.transform.rotation + 
                  " from spawn point " + projectileSpawnLocation.transform.position);
    }
}
