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

    private DurationEffect dashShaderEffect;
    private PlayerMovement.Effect dashMovementEffect;
    
    
    
    

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

    private class DashShaderEffect : ADurationEffect {
        public override float duration {
            get => dash.duration + dash.dashVfxDuration;
            set { }
        }
        private PlayerDashAbility dash;
        public DashShaderEffect(PlayerDashAbility dash) {
            this.dash = dash;
        }
        protected override void ApplyEffect() {
            var renderer = dash.gameObject.GetComponent<Renderer>();
            dash.glitchMaterial.SetFloat(GLITCH_MATERIAL_START_TIME, Time.time);
            dash.glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, duration);
            dash.ApplyMaterials((ref Material material) => material = dash.glitchMaterial);
        }
        protected override void UnapplyEffect() {
            dash.SetDefaultMaterials();
        }
        public override void UpdateEffect() {}
    }

    [InjectComponent] public PlayerMovement playerMovement;
    [InjectComponent] public PlayerAnimationController playerAnimation;
    [InjectComponent] public PlayerGravity playerGravity;
    private DashShaderEffect dashVisualEffect;
    private EffectManager<ABaseEffect> effects = new EffectManager<ABaseEffect>();

    protected override void OnAbilityReset() {
        effects.Clear();
    }
    protected override void OnAbilityStart() {
        // reuse cached visual effect
        if (dashVisualEffect == null) dashVisualEffect = new DashShaderEffect(this);
        else dashVisualEffect.Reset();
        
        // apply effects:
        
        // apply dash visual effect
        effects.ApplyEffect(dashVisualEffect, () => this.duration + this.dashVfxDuration);
        
        // increase player's move speed
        effects.ApplyEffect(playerMovement.SetMoveSpeedEffect(dashSpeed), () => this.duration);
        
        // play animation
        effects.ApplyEffect(playerAnimation.SetBoolEffect("isDashing", true), () => this.duration);

        // cancel gravity
        var gravity = playerGravity.gravity;
        var gravityEffect = playerGravity.SetGravityEffect(0f);
        
        // but apply built up velocity when gravity ends
        gravityEffect.OnEffectEnd += () =>
            playerMovement.ApplyJump(-gravity * gravityEffect.elapsedTime);
        effects.ApplyEffect(gravityEffect, () => this.duration);
    }
    public float dashVfxDuration = 1.2f;

    [Tooltip("use kinematic vs velocity updates")]
    public bool useKinematic = false;

    private Vector3 savedDashVelocity = Vector3.zero;

    protected override void OnAbilityUpdate() {
        if (dashVisualEffect?.active ?? false) {
            dashVisualEffect.UpdateEffect();
        }
    }
}
