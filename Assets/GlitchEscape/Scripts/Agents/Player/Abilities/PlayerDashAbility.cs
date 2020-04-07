using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerDashAbility : PlayerAbility {
    [InjectComponent] public Animator animator;
    public Material glitchMaterial;

    public override float resourceCost => player.config.dashAbilityStaminaCostRange.minimum;
    public override float cooldownTime => player.config.dashAbilityCooldownTime;
    
    // TODO: reimplement variable-length button presses
    protected override float abilityDuration => player.config.dashAbilityDurationRange.minimum;
    
    // TODO: reimplement variable-length button presses
    private float dashSpeed => player.config.dashAbilityMoveRange.minimum / abilityDuration;

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
    protected override void OnAbilityReset() {
        animator.SetBool("isDashing", false);
        ClearGlitchShader();
    }
    protected override void OnAbilityStart() {
        SetGlitchShader();
        BeginDash();
        FireEvent(PlayerEvent.Type.BeginDash);
    }

    protected override void OnAbilityEnd() {
        glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, abilityDuration + dashVfxDuration);
        ApplyMaterials((ref Material material) => material = glitchMaterial);
        EndDash();
        FireEvent(PlayerEvent.Type.EndDash);
    }
    // protected override void OnAbilityStateChange(PlayerAbilityState prevState, PlayerAbilityState newState) {
    //     // Debug.Log("Setting state " + prevState + " => " + newState);
    //     switch (newState) {
    //         case PlayerAbilityState.Active: 
    //             
    //             break;
    //         case PlayerAbilityState.Ending:
    //             glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, abilityDuration + dashVfxDuration);
    //             ApplyMaterials((ref Material material) => material = glitchMaterial);
    //             EndDash();
    //             break;
    //         case PlayerAbilityState.None:
    //             if (prevState == PlayerAbilityState.Active) {
    //                 EndDash();
    //             }
    //             ClearGlitchShader();
    //             break;
    //     }
    // }

    // TeleportEffectGraph variables
    public const string GLITCH_MATERIAL_EMISSION_COLOR = "EmissionColor_9A7229B8";
    public const string GLITCH_MATERIAL_START_TIME = "StartTime_B9ED4C73";
    public const string GLITCH_MATERIAL_DURATION = "Duration_2B114277";
    
    void SetGlitchShader() {
        var renderer = GetComponent<Renderer>();
        
        glitchMaterial.SetFloat(GLITCH_MATERIAL_START_TIME, Time.time);
        glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, abilityDuration + dashVfxDuration);
        ApplyMaterials((ref Material material) => material = glitchMaterial);
    }
    void ClearGlitchShader() {
        SetDefaultMaterials();
    }
    public float dashVfxDuration = 1.2f;

    [Tooltip("use kinematic vs velocity updates")]
    public bool useKinematic = false;

    private Vector3 savedDashVelocity = Vector3.zero;

    protected override void OnAbilityUpdate() {
        // move the player if they're currently dashing, and update vfx
        if (isAbilityActive) {
            var moveDir = Vector3.forward;
            if (useKinematic) {
                player.transform.Translate(moveDir * dashSpeed * Time.deltaTime);
            } else {
                GetComponent<Rigidbody>().velocity = player.transform.rotation * moveDir * dashSpeed;
            }
        }
    }                                                
    private void BeginDash() {
        if (!animator.GetBool("isDashing")) {
            // Debug.Log("starting dash animation");
            animator.SetBool("isDashing", true);
            animator.SetTrigger("startDashing");
        }
        // save velocity
        savedDashVelocity = GetComponent<Rigidbody>().velocity;
        if (useKinematic) {
            GetComponent<Rigidbody>().isKinematic = true;
        }        
        FireEvent(PlayerEvent.Type.BeginDash);
    }
    private void EndDash() {
        if (animator.GetBool("isDashing")) {
            // Debug.Log("ending dash animation");
            animator.SetBool("isDashing", false);
            animator.SetTrigger("stopDashing");
        }
        // reapply velocity, plus gravity over time spent dashing
        GetComponent<Rigidbody>().velocity = savedDashVelocity +
                             Vector3.down * Mathf.Abs(Physics.gravity.y) * timeElapsedSinceAbilityStart;
        if (useKinematic) {
            GetComponent<Rigidbody>().isKinematic = false;
        }        
        FireEvent(PlayerEvent.Type.EndDash);
    }
}

public class NoiseLowPolyDeform {
    public Material material { get; set; }
    
    public float Depth {
        get => material.GetFloat("Vector1_FC657AF4");
        set => material.SetFloat("Vector1_FC657AF4", value);
    }
    public Color BaseColor {
        get => material.GetColor("Color_A73DBC10");
        set => material.SetColor("Color_A73DBC10", value);
    }
    public Color EmissionColor {
        get => material.GetColor("Color_C3F57BAC");
        set => material.SetColor("Color_C3F57BAC", value);
    }
    public Color Color {
        get => material.GetColor("Color_BF62A8F6");
        set => material.SetColor("Color_BF62A8F6", value);
    }

    public NoiseLowPolyDeform (Material material) {
        this.material = material;
    }
    public void UseMaterial(Material material) {
        this.material = material;
    }
    public void UseMaterial(Renderer renderer, int index = 0) {
        this.material = renderer.materials[index];
    }
    public void UseSharedMaterial(Renderer renderer, int index = 0) {
        this.material = renderer.sharedMaterials[index];
    }
    public void UseMaterial(GameObject obj, int index = 0) {
        UseMaterial(obj.GetComponent<Renderer>(), index);
    }
    public void UseSharedMaterial(GameObject obj, int index = 0) {
        UseSharedMaterial(obj.GetComponent<Renderer>(), index);
    }
}

