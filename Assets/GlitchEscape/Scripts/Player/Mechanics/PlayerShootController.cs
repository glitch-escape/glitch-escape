using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerShootController : PlayerAbility {

    private PlayerControls.HybridButtonControl m_inputButton;
    protected override PlayerControls.HybridButtonControl inputButton
        => m_inputButton ?? (m_inputButton = PlayerControls.instance.shoot);
    
    public Projectile projectilePrefab;
    public Transform projectileSpawnLocation;
    
    /// <summary>
    /// Can trigger iff the player is already moving
    /// </summary>
    /// <returns></returns>
    protected override bool CanStartAbility() {
        //return PlayerControls.instance.moveInput.magnitude > 0f;
        return true;
    }

    protected override void SetupAbility() {
        //animator.SetBool("isDashing", false);
        if (projectilePrefab == null) {
            Debug.LogError("Missing player projectile prefab.");
        }
    }

    protected override void OnAbilityStateChange(PlayerAbilityState prevState, PlayerAbilityState newState) {
        switch (newState) {
            case PlayerAbilityState.Active: 
                SpawnProj();
                break;
            case PlayerAbilityState.Ending:
                break;
            case PlayerAbilityState.None:
                break;
        }
    }
    protected override bool IsAbilityFinished() {
        return elapsedTime > currentAbilityDuration;
    }
    
    protected override void UpdateAbility() {
        // Play a projectile cration animation here?

        // update vfx
        // TODO: any other vfx stuff goes here
    }  
    private void SpawnProj() {
        Vector3 direction = projectileSpawnLocation.forward;
        Vector3 origin = projectileSpawnLocation.position;
        // Quaternion quat = Quaternion.LookRotation(direction, Vector3.up);
        Projectile bullet = Instantiate(
            projectilePrefab,
            projectileSpawnLocation.position,
            projectileSpawnLocation.rotation);
        Debug.Log("Spawned projectile: "+bullet.transform.position+ ", " + bullet.transform.rotation + 
                  " from spawn point " + projectileSpawnLocation.transform.position);
    }
}
