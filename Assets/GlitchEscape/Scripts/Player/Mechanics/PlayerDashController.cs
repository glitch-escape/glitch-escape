using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerDashController : MonoBehaviour, IPlayerControllerComponent
{
    const float GRAVITY = 9.81f; // m/s^2

    private PlayerController controller;
    private Player player;
    private Animator animator;
    private new Rigidbody rigidbody;
    private List<Material> defaultMaterials;
    private Renderer[] renderers;
    public Material glitchMaterial;
    public float glitchEffectDuration = 3f;
    private PlayerControls.HybridButtonControl dashInput;
    
    public void SetupControllerComponent(PlayerController controller) {
        this.controller = controller;
        dashInput = PlayerControls.instance.dash;
        player = controller.player;
        rigidbody = player.rigidbody;
        animator = player.animator;
        renderers = player.GetComponentsInChildren<Renderer>();
        defaultMaterials = new List<Material>();
        foreach (var renderer in renderers) {
            foreach (var material in renderer.materials) {
                defaultMaterials.Add(material);
            }
        }
        animator.SetBool("isDashing", false);
    }

    // TeleportEffectGraph variables
    public const string GLITCH_MATERIAL_EMISSION_COLOR = "EmissionColor_9A7229B8";
    public const string GLITCH_MATERIAL_START_TIME = "StartTime_B9ED4C73";
    public const string GLITCH_MATERIAL_DURATION = "Duration_2B114277";
    
    void SetGlitchShader() {
        glitchMaterial.SetFloat(GLITCH_MATERIAL_START_TIME, Time.time);
        glitchMaterial.SetFloat(GLITCH_MATERIAL_DURATION, glitchEffectDuration);
        foreach (var renderer in renderers) {
            var materials = renderer.materials;
            for (int i = 0; i < renderer.materials.Length; ++i) {
                materials[i] = glitchMaterial;
            }
            renderer.materials = materials;
        }
    }
    void ClearGlitchShader() {
        int matIndex = 0;
        foreach (var renderer in renderers) {
            var materials = renderer.materials;
            for (int i = 0; i < renderer.materials.Length; ++i) {
                materials[i] = defaultMaterials[matIndex++];
            }

            renderer.materials = materials;
        }
    }

    public float dashStaminaCost = 10f;

    private bool isVfxActive = false;
    public float dashVfxDuration = 1.2f;

    #region DashMechanics
    #region ScriptProperties
    
    [Tooltip("use kinematic vs velocity updates")]
    public bool useKinematic = false;
    
    [Range(0f, 20f)]
    [Tooltip("minimum dash distance (meters)")]
    public float minDashLength = 2f;

    [Range(0f, 20f)]
    [Tooltip("maximum dash distance (meters)")]
    public float maxDashLength = 4f;

    [Range(0f, 200f)]
    [Tooltip("dash speed (m/s)")] 
    public float dashSpeed = 10f;

    [Range(0f, 10f)]
    [Tooltip("max dash hold time (seconds), at which length of dash will be capped")]
    public float maxDashPressDuration = 1.5f;

    public AnimationCurve dashPressCurve;

    [Range(0f, 2f)]
    [Tooltip("minimum time between dashs")]
    public float dashCooldown = 0.2f;
    
    #endregion
    #region PrivateVariablesAndDerivedProperties

    // is player currently dashing?
    private bool isDashing = false;
    public float dashPressStrength =>
        dashPressCurve.Evaluate(Mathf.Clamp(dashInput.pressTime / maxDashPressDuration, 0f, 1f));
    public float currentDashDuration =>
        Mathf.Lerp(dashPressStrength, minDashLength, maxDashLength) / dashSpeed;
    private float dashStartTime;
    private Vector3 savedDashVelocity = Vector3.zero;
    
    #endregion
    #region DashImplementation

    private bool isDashPressed = false;
    private float dashPressTime = 0f;

    public void Update() {
        if (dashInput.wasPressedThisFrame) {
            BeginDash();
        }
        // terminate dash effect after max dash time
        if (Time.time > dashStartTime + currentDashDuration) {
            EndDash();
        }
        if (Time.time > dashStartTime + dashVfxDuration) {
            isVfxActive = false;
            EndDashVfx();
        }
        // move the player if they're currently dashing, and update vfx
        if (isDashing) {
            var moveDir = Vector3.forward;
            if (useKinematic) {
                player.transform.Translate(moveDir * dashSpeed * Time.deltaTime);
            } else {
                rigidbody.velocity = player.transform.rotation * moveDir * dashSpeed;
            }
            
        }
        if (isVfxActive) {
            // update vfx
            UpdateDashVfx();
        }
    }                                                
    private void BeginDash() {
        // check: can we dash yet? if no, cancel
        if (Time.time < dashStartTime + dashCooldown) {
            // Debug.Log("dash still on cooldown");
            return;
        }
        // check: are we moving? if no, cancel
        if (PlayerControls.instance.moveInput.magnitude < 1e-4f) {
            return;
        }
        // do we have enough stamina to perform this action? if no, cancel
        if (!player.TryUseAbility(dashStaminaCost)) {
            return;
        }
        
        // if already dashing, end that + restart
        if (isDashing) {
            EndDash();
        }
        if (!animator.GetBool("isDashing")) {
            // Debug.Log("starting dash animation");
            animator.SetBool("isDashing", true);
            animator.SetTrigger("startDashing");
        }
        // begin dash
        // Debug.Log("Start dash!");
        BeginDashVfx();
        isVfxActive = true;
        
        // save velocity
        savedDashVelocity = rigidbody.velocity;
        
        if (useKinematic) {
            rigidbody.isKinematic = true;
        }
        isDashing = true; 
        dashStartTime = Time.time;
    }
    private void EndDash() {
        if (animator.GetBool("isDashing")) {
            // Debug.Log("ending dash animation");
            animator.SetBool("isDashing", false);
            animator.SetTrigger("stopDashing");
        }
        if (isDashing) {

            // end dash
            // Debug.Log("Stop dash!");
            // EndDashVfx();
            
            // reapply velocity, plus gravity over time spent dashing
            var elapsedTime = Time.time - dashStartTime;
            // Debug.Log("Applying additional velocity change after " + elapsedTime + " seconds: "
            //           + GRAVITY * elapsedTime);
            rigidbody.velocity = savedDashVelocity +
                                   Vector3.down * GRAVITY * elapsedTime;
            if (useKinematic) {
                rigidbody.isKinematic = false;
            }
        }
        isDashing = false;
    }
    #endregion
    #endregion
    
    #region DashVfx
    #region ScriptProperties
    #endregion
    #region PrivateVariables
        private Material defaultMaterial;
        private Shader dashShader;
        private Shader defaultShader;
        private float animateTime = 1.0f;
        private float dashHoldTime = 0f;
        #endregion
    #region VfxImplementation
    private void BeginDashVfx() {
        SetGlitchShader();
    }
    private void EndDashVfx() {
        ClearGlitchShader();
    }
    private void UpdateDashVfx() {
    }
    #endregion
    #endregion DashVfx
}
