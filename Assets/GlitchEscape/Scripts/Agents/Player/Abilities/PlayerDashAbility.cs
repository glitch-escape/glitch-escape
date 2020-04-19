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

    private float duration => player.config.dashAbilityDurationRange.Lerp(dashStrength);
    private float dashStrength => 
        player.config.dashAbilityPressTimeRange.SampleCurve(
            dashPressTime,
            player.config.dashAbilityPressCurve);
    
    private float dashPressTime = 0f;
    
    // TeleportEffectGraph variables
    public const string GLITCH_MATERIAL_EMISSION_COLOR = "EmissionColor_9A7229B8";
    public const string GLITCH_MATERIAL_START_TIME = "StartTime_B9ED4C73";
    public const string GLITCH_MATERIAL_DURATION = "Duration_2B114277";

    private class DashShaderEffect : IEffectActions {
        private PlayerDashAbility dash;
        public DashShaderEffect(PlayerDashAbility dash) {
            this.dash = dash;
        }
        public void ApplyEffect() {
            var renderer = dash.gameObject.GetComponent<Renderer>();
            dash.glitchMaterial.SetFloat(GLITCH_MATERIAL_START_TIME, Time.time);
            dash.glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, dash.duration);
            dash.ApplyMaterials((ref Material material) => material = dash.glitchMaterial);
        }
        public void UnapplyEffect() {
            dash.SetDefaultMaterials();
        }
        public void UpdateEffect() {}
    }

    [InjectComponent] public PlayerMovement playerMovement;
    [InjectComponent] public PlayerAnimationController playerAnimation;
    [InjectComponent] public PlayerGravity playerGravity;
    private Effect reusedDashVisualEffect;
    private Effect GetNewResetDashVisualEffect() {
        if (reusedDashVisualEffect == null) {
            reusedDashVisualEffect = DurationEffect.MakeEffect(
                new DashShaderEffect(this),
                () => this.dashVfxDuration);
        } else {
            reusedDashVisualEffect.Reset();
        }
        return reusedDashVisualEffect;
    }
    private EffectManager effects = new EffectManager();

    protected override void OnAbilityReset() {
        effects.Clear();
    }

    private float GetCurrentDashDuration() {
        return duration;
    }

    protected override void OnAbilityStart() {
        
        // apply dash visual effect
        effects.AddEffect(GetNewResetDashVisualEffect()).Start();
        
        // increase player's move speed
        effects.AddEffect(playerMovement.IncreaseMoveSpeed(dashSpeed), GetCurrentDashDuration).Start();
        
        // play animation
        effects.AddEffect(
            playerAnimation.SetBool("isDashing", true),
            () => this.duration).Start();

        // cancel gravity
        var gravity = playerGravity.gravity;
        var disableGravityEffect = playerGravity.ApplyGravityStrengthMultiplier(0f);
        var gravityEffect = effects.AddEffect(new EffectActions {
            applyEffect = () => disableGravityEffect.active = true,
            unapplyEffect = () => disableGravityEffect.Cancel()
        }, () => this.duration);
        
        // but apply built up velocity when gravity ends
        gravityEffect.onEffectEnded += () =>
            playerMovement.ApplyJump(-gravity * gravityEffect.elapsedTime);
        gravityEffect.Start();
    }
    public float dashVfxDuration = 1.2f;

    [Tooltip("use kinematic vs velocity updates")]
    public bool useKinematic = false;

    private Vector3 savedDashVelocity = Vector3.zero;

    protected override void OnAbilityUpdate() {
        effects.Update();
    }
}
