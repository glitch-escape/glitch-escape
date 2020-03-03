using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerDashController : PlayerAbility {
    public Material glitchMaterial;

    private PlayerControls.HybridButtonControl m_inputButton;
    protected override PlayerControls.HybridButtonControl inputButton
        => m_inputButton ?? (m_inputButton = PlayerControls.instance.dash);

    /// <summary>
    /// Can trigger iff the player is already moving
    /// </summary>
    /// <returns></returns>
    protected override bool CanStartAbility() {
        return PlayerControls.instance.moveInput.magnitude > 0f;
    }

    protected override void SetupAbility() {
        animator.SetBool("isDashing", false);
    }

    protected override void OnAbilityStateChange(PlayerAbilityState prevState, PlayerAbilityState newState) {
        // Debug.Log("Setting state " + prevState + " => " + newState);
        switch (newState) {
            case PlayerAbilityState.Active: 
                SetGlitchShader();
                BeginDash();
                break;
            case PlayerAbilityState.Ending:
                glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, currentAbilityDuration + dashVfxDuration);
                ApplyMaterials((ref Material material) => material = glitchMaterial);
                EndDash();
                break;
            case PlayerAbilityState.None:
                ClearGlitchShader();
                break;
        }
    }
    protected override bool IsAbilityFinished() {
        return elapsedTime > currentAbilityDuration + dashVfxDuration;
    }

    // TeleportEffectGraph variables
    public const string GLITCH_MATERIAL_EMISSION_COLOR = "EmissionColor_9A7229B8";
    public const string GLITCH_MATERIAL_START_TIME = "StartTime_B9ED4C73";
    public const string GLITCH_MATERIAL_DURATION = "Duration_2B114277";
    
    void SetGlitchShader() {
        glitchMaterial.SetFloat(GLITCH_MATERIAL_START_TIME, Time.time);
        glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, currentAbilityDuration + dashVfxDuration);
        ApplyMaterials((ref Material material) => material = glitchMaterial);
    }
    void ClearGlitchShader() {
        SetDefaultMaterials();
    }
    public float dashVfxDuration = 1.2f;

    [Tooltip("use kinematic vs velocity updates")]
    public bool useKinematic = false;

    [Range(0f, 200f)]
    [Tooltip("dash speed (m/s)")] 
    public float dashSpeed = 10f;

    private Vector3 savedDashVelocity = Vector3.zero;

    protected override void UpdateAbility() {
        // move the player if they're currently dashing, and update vfx
        if (state == PlayerAbilityState.Active) {
            var moveDir = Vector3.forward;
            if (useKinematic) {
                player.transform.Translate(moveDir * dashSpeed * Time.deltaTime);
            } else {
                rigidbody.velocity = player.transform.rotation * moveDir * dashSpeed;
            }
        }
        // update vfx
        // TODO: any other vfx stuff goes here
    }                                                
    private void BeginDash() {
        if (!animator.GetBool("isDashing")) {
            // Debug.Log("starting dash animation");
            animator.SetBool("isDashing", true);
            animator.SetTrigger("startDashing");
        }
        // save velocity
        savedDashVelocity = rigidbody.velocity;
        if (useKinematic) {
            rigidbody.isKinematic = true;
        }
        player.PlaySound(3);//play static
    }
    private void EndDash() {
        if (animator.GetBool("isDashing")) {
            // Debug.Log("ending dash animation");
            animator.SetBool("isDashing", false);
            animator.SetTrigger("stopDashing");
        }
        // reapply velocity, plus gravity over time spent dashing
        rigidbody.velocity = savedDashVelocity +
                             Vector3.down * Mathf.Abs(Physics.gravity.y) * elapsedTime;
        if (useKinematic) {
            rigidbody.isKinematic = false;
        }
    }
}
