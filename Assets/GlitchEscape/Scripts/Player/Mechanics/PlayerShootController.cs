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
        if (projPrefab == null) {
            Debug.Log("Missing player projectile prefab.");
        }
    }

    protected override void OnAbilityStateChange(PlayerAbilityState prevState, PlayerAbilityState newState) {
        // Debug.Log("Setting state " + prevState + " => " + newState);
        switch (newState) {
            case PlayerAbilityState.Active: 
             //   SetGlitchShader();
               // BeginDash();
                SpawnProj();
                Debug.Log("WEEEE");
                break;
            case PlayerAbilityState.Ending:
            //    glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, currentAbilityDuration + dashVfxDuration);
          //      ApplyMaterials((ref Material material) => material = glitchMaterial);
                //EndDash();
                break;
            case PlayerAbilityState.None:
                if (prevState == PlayerAbilityState.Active) {
                  //  EndDash();
                }
            //    ClearGlitchShader();
                break;
        }
    }
    protected override bool IsAbilityFinished() {
        return elapsedTime > currentAbilityDuration;
    }

    public Projectile projPrefab;
    public float projYShift;

    protected override void UpdateAbility() {
        // Play a projectile cration animation here?

        // update vfx
        // TODO: any other vfx stuff goes here
    }  
    private void SpawnProj() {
        Vector3 direction = player.transform.forward;
        Vector3 origin = player.transform.position;

        // Spawn bullet
        origin.y += projYShift;
        Quaternion quat = Quaternion.LookRotation(direction, Vector3.up);
        Projectile bullet = Instantiate(projPrefab, origin, quat);
        bullet.gameObject.SetActive(true);
        bullet.SetPlayerPos(player.transform);
    }
}
