using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using GlitchEscape.Effects;
using GlitchEscape.Scripts.DebugUI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerDashAbility : PlayerAbility, IPlayerDebug {
    [InjectComponent] public Animator animator;
    public Material glitchMaterial;

    public override float resourceCost => GetStaminaCostFromPressDuration(0f);
    public override float cooldownTime => player.config.dashAbilityCooldownTime;
    private float dashSpeed => player.config.dashAbilitySpeed;
    protected override PlayerControls.HybridButtonControl inputButton => null; // control using PlayerController instead
    public float GetAbilityDurationFromPressDuration(float pressDuration) {
        var pressStrength = player.config.dashAbilityPressTimeRange.SampleCurve(
            pressDuration, player.config.dashAbilityPressCurve);
        var distance = player.config.dashAbilityMoveRange.Lerp(pressStrength);
        var duration = distance / dashSpeed;
        return duration;
    }
    public float GetStaminaCostFromPressDuration(float pressDuration) {
        var pressStrength = player.config.dashAbilityPressTimeRange.SampleCurve(
            pressDuration, player.config.dashAbilityPressCurve);
        return player.config.dashAbilityStaminaCostRange.Lerp(pressStrength);
    }
    public float currentPressDuration { get; set; } = 0f;
    protected override float abilityDuration => GetAbilityDurationFromPressDuration(currentPressDuration);

    /// <summary>
    /// Can trigger iff the player is already moving
    /// </summary>
    /// <returns></returns>
    protected override bool CanStartAbility() {
        return PlayerControls.instance.moveInput.magnitude > 0f;
    }

    public IEffect TryUseAbilityAsEffect() {
        IEffect effect = null;
        return CanStartAbility() ? null : effect;
    }
    
    // TeleportEffectGraph variables
    public const string GLITCH_MATERIAL_EMISSION_COLOR = "EmissionColor_9A7229B8";
    public const string GLITCH_MATERIAL_START_TIME = "StartTime_B9ED4C73";
    public const string GLITCH_MATERIAL_DURATION = "Duration_2B114277";

    private class DashShaderEffect {
        private PlayerDashAbility dash;

        public DashShaderEffect(PlayerDashAbility dash) {
            this.dash = dash;
        }

        public void ApplyEffect() {
            var renderer = dash.gameObject.GetComponent<Renderer>();
            dash.glitchMaterial.SetFloat(GLITCH_MATERIAL_START_TIME, Time.time);
            dash.glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, dash.abilityDuration);
            dash.ApplyMaterials((ref Material material) => material = dash.glitchMaterial);
        }

        public void UnapplyEffect() {
            dash.SetDefaultMaterials();
        }

        public void UpdateEffect() { }
        public void Reset() { UnapplyEffect(); }
    }

    [InjectComponent] public PlayerMovement playerMovement;
    [InjectComponent] public PlayerAnimationController playerAnimation;
    [InjectComponent] public PlayerGravity playerGravity;
    private DashShaderEffect dashVfx;
    
    protected override void OnAbilityReset() {
        
    }

    private float GetCurrentDashDuration() {
        return abilityDuration;
    }

    private IEffect increaseMoveSpeedEffect;
    private IEffect disableGravityEffect;

    private float startTime = 0f;

    protected override void OnAbilityStart() {
        increaseMoveSpeedEffect = playerMovement.ApplyDashSpeed(30f).Start();
        disableGravityEffect = playerGravity.ModifyGravity(0f).Start();
        dashVfx = dashVfx ?? (dashVfx = new DashShaderEffect(this));
        dashVfx.ApplyEffect();
        startTime = Time.time;
    }

    protected override void OnAbilityEnd() {
        increaseMoveSpeedEffect?.Cancel();
        disableGravityEffect?.Cancel();
        dashVfx?.UnapplyEffect();
        var gravity = playerGravity.gravity;
        playerMovement.ApplyJump(-gravity * (Time.time - startTime));
    }
    
    public float dashVfxDuration = 1.2f;

    [Tooltip("use kinematic vs velocity updates")]
    public bool useKinematic = false;

    private Vector3 savedDashVelocity = Vector3.zero;

    protected override void OnAbilityUpdate() {
        dashVfx?.UpdateEffect();
    }

    public string debugName => this.GetType().Name;
    public void DrawDebugUI() {
        GUILayout.Label("dash speed: "+dashSpeed);
        GUILayout.Label("current dash press time: "+currentPressDuration);
        GUILayout.Label("current dash strength: "+
                        player.config.dashAbilityPressTimeRange.SampleCurve(
                            currentPressDuration, player.config.dashAbilityPressCurve));
        GUILayout.Label("current dash distance: "+(abilityDuration * dashSpeed));
        GUILayout.Label("current dash time: "+abilityDuration);
        GUILayout.Label("current total stamina cost: "+GetStaminaCostFromPressDuration(currentPressDuration));
    }
}
