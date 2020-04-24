using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using GlitchEscape.Effects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerDashAbility : PlayerAbility {
    [InjectComponent] public Animator animator;
    public Material glitchMaterial;

    public override float resourceCost => player.config.dashAbilityStaminaCostRange.minimum;
    public override float cooldownTime => player.config.dashAbilityCooldownTime;

    // TODO: reimplement variable-length button presses
    private float dashStrength =>
        player.config.dashAbilityPressTimeRange.SampleCurve(
            inputButton.pressTime,
            player.config.dashAbilityPressCurve);

    private float dashDistance => player.config.dashAbilityMoveRange.Lerp(dashStrength);
    private float dashSpeed => player.config.dashAbilitySpeed;
    protected override float abilityDuration => dashDistance / dashSpeed;

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

    public override string debugName => this.GetType().Name;
    public override void DrawDebugUI() {
        base.DrawDebugUI();
        GUILayout.Label("dash speed: "+dashSpeed);
        GUILayout.Label("current dash strength: "+dashStrength);
        GUILayout.Label("current dash distance: "+dashDistance);
        GUILayout.Label("current dash time: "+abilityDuration);
    }
}
