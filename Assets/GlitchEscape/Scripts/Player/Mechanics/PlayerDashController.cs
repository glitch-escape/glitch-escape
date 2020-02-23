﻿using System;
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
    private Input input;
    private Animator animator;
    private new Rigidbody rigidbody;
    private List<Material> defaultMaterials;
    private Renderer[] renderers;
    public Material glitchMaterial;
    public float glitchEffectDuration = 3f;
    
    public void SetupControllerComponent(PlayerController controller) {
        this.controller = controller;
        player = controller.player;
        rigidbody = player.rigidbody;
        animator = player.animator;
        input = player.input;
        input.Controls.Dodge.performed += context => {
            bool pressed = context.ReadValue<float>() > 0f;
            dodgePressed = pressed;
        };
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
    
    // is the dodge button currently pressed?
    private bool dodgePressed = false;

    private bool isVfxActive = false;
    public float dodgeVfxDuration = 1.2f;

    #region DodgeMechanics
    #region ScriptProperties
    
    [Tooltip("use kinematic vs velocity updates")]
    public bool useKinematic = false;
    
    [Range(0f, 20f)]
    [Tooltip("minimum dodge distance (meters)")]
    public float minDodgeLength = 2f;

    [Range(0f, 20f)]
    [Tooltip("maximum dodge distance (meters)")]
    public float maxDoddgeLength = 4f;

    [Range(0f, 200f)]
    [Tooltip("dodge speed (m/s)")] 
    public float dodgeSpeed = 10f;
    
    [Range(0f, 10f)]
    [Tooltip("min dodge hold time (seconds), after which length of dodge will be extended")]
    public float minDodgeHoldTime = 0.5f;
    
    [Range(0f, 10f)]
    [Tooltip("max dodge hold time (seconds), at which length of dodge will be capped")]
    public float maxDodgeHoldTime = 1.5f;

    [Range(0f, 2f)]
    [Tooltip("minimum time between dodges")]
    public float dodgeCooldown = 0.2f;
    
    #endregion
    #region PrivateVariablesAndDerivedProperties

    // is player currently dodging?
    private bool isDodging = false;
    
    // was dodge button currently pressed as of last frame?
    private bool isDodgePressed = false;

    // time that dodge started (seconds)
    private float dodgeStartTime = -10f;
    
    // time that dodge has been pressed so far
    private float dodgePressTime = 0f;
    
    private float minDodgeDuration {
        get { return minDodgeLength / dodgeSpeed;  }
    }

    // saved velocity (when dashing)
    private Vector3 savedDodgeVelocity = Vector3.zero;

    // current strength of dodge press (depends on press time + min / max dodge hold time)
    private float getCurrentDodgePressStrength () {
        var pressTime = dodgePressTime - minDodgeHoldTime;
        var maxPressTime = maxDodgeHoldTime - minDodgeHoldTime;
        var value = (pressTime + 1e-9f) / (maxPressTime + 1e-9f);    // add small # to avoid divide by zero
        return Mathf.Clamp(value, 0f, 1f);
    }
    
    // current duration of dodge (depends on press time)
    private float getCurrentDodgeDuration () {
        var dodgeLength = getCurrentDodgePressStrength() * (maxDoddgeLength - minDodgeLength) + minDodgeLength;
        return dodgeLength / dodgeSpeed;
    }
    
    // time that dodge has been active so far
    private float elapsedDodgeTime {
        get { return Time.time - dodgeStartTime;  }
    }
    #endregion
    #region DodgeImplementation

    public void Update() {
        // handle dodge press input
        if (dodgePressed != isDodgePressed) {
            // Debug.Log("dodge state changed! "+isDodgePressed+" => "+dodgePressed);
            if (dodgePressed) {
                isDodgePressed = true;
                BeginDodge();
            } else { 
                // dodge button released
                isDodgePressed = false;
            }
        } else if (isDodgePressed) {
            // update dodge press time
            dodgePressTime = Time.time - dodgePressTime;

            // ignore additional press time after we exceed the maximum
            if (dodgePressTime > maxDodgeHoldTime) {
                dodgePressTime = maxDodgeHoldTime;
                isDodgePressed = false;
            }
        }
        
        // terminate dodge effect after max dodge time
        var dodgeDuration = getCurrentDodgeDuration();
        if (Time.time > dodgeStartTime + dodgeDuration) {
            EndDodge();
        }
        if (Time.time > dodgeStartTime + dodgeVfxDuration) {
            isVfxActive = false;
            EndDodgeVfx();
        }
        
        // move the player if they're currently dodging, and update vfx
        if (isDodging) {
            // var rawMoveInput = input.Controls.Move.ReadValue<Vector2>();
            // var moveInput = rawMoveInput.magnitude > 0f ? rawMoveInput.normalized : Vector2.up;
            // var moveDir = Vector3.forward * moveInput.y + Vector3.right * moveInput.x;
            var moveDir = Vector3.forward;
            if (useKinematic) {
                player.transform.Translate(moveDir * dodgeSpeed * Time.deltaTime);
            } else {
                rigidbody.velocity = player.transform.rotation * moveDir * dodgeSpeed;
            }
            
        }
        if (isVfxActive) {
            // update vfx
            UpdateDodgeVfx();
        }
    }                                                
    private void BeginDodge() {
        // check: can we dodge yet? if no, cancel
        if (Time.time < dodgeStartTime + dodgeCooldown) {
            // Debug.Log("dodge still on cooldown");
            return;
        }
        // check: are we moving? if no, cancel
        if (input.Controls.Move.ReadValue<Vector2>().magnitude == 0f) {
            return;
        }
        // do we have enough stamina to perform this action? if no, cancel
        if (!player.TryUseAbility(dashStaminaCost)) {
            return;
        }
        
        // if already dodging, end that + restart
        if (isDodging) {
            EndDodge();
        }
        if (!animator.GetBool("isDashing")) {
            Debug.Log("starting dash animation");
            animator.SetBool("isDashing", true);
            animator.SetTrigger("startDashing");
        }
        // begin dodge
        // Debug.Log("Start dodge!");
        BeginDodgeVfx();
        isVfxActive = true;
        
        // save velocity
        savedDodgeVelocity = rigidbody.velocity;
        
        if (useKinematic) {
            rigidbody.isKinematic = true;
        }
        isDodging = true; 
        dodgeStartTime = Time.time;
    }
    private void EndDodge() {
        if (animator.GetBool("isDashing")) {
            Debug.Log("ending dash animation");
            animator.SetBool("isDashing", false);
            animator.SetTrigger("stopDashing");
        }
        if (isDodging) {

            // end dodge
            // Debug.Log("Stop dodge!");
            // EndDodgeVfx();
            
            // reapply velocity, plus gravity over time spent dashing
            var elapsedTime = Time.time - dodgeStartTime;
            // Debug.Log("Applying additional velocity change after " + elapsedTime + " seconds: "
            //           + GRAVITY * elapsedTime);
            rigidbody.velocity = savedDodgeVelocity +
                                   Vector3.down * GRAVITY * elapsedTime;
            if (useKinematic) {
                rigidbody.isKinematic = false;
            }
        }
        isDodging = false;
    }
    #endregion
    #endregion
    
    #region DodgeVfx
    #region ScriptProperties
        public ParticleSystem dodgeGroundParticle;
        public ParticleSystem dodgeAirParticle;
        public float animateLength = .01f;
        public float dodgeScaleFactor = 15f;
        public Texture noiseTex;
    #endregion
    #region PrivateVariables
        private Material defaultMaterial;
        private Shader dodgeShader;
        private Shader defaultShader;
        private float animateTime = 1.0f;
        private float dodgeHoldTime = 0f;
        #endregion
    #region VfxImplementation
    private void BeginDodgeVfx() {
        //dodgeGroundParticle.Emit(1);
        dodgeAirParticle.Emit(30);
        dodgeGroundParticle.transform.position = transform.position;
        dodgeGroundParticle.Emit(1);
        SetGlitchShader();
        // defaultMaterial.shader = dodgeShader;
        // defaultMaterial.SetTexture("_Noise", noiseTex);
        dodgeGroundParticle.transform.rotation = this.transform.rotation;
    }
    private void EndDodgeVfx() {
        ClearGlitchShader();
        // defaultMaterial.shader = defaultShader;
        animateTime = 1.0f;
        dodgeHoldTime = 0f;
        dodgeGroundParticle.transform.rotation = this.transform.rotation;
    }
    private void UpdateDodgeVfx() {
        // defaultMaterial.SetFloat("_AlphaThreshold", animateTime);
        dodgeHoldTime += Time.deltaTime;
        dodgeGroundParticle.transform.position = this.transform.position + (-transform.forward * dodgeHoldTime * dodgeScaleFactor);
        //rotate dodgeGroundParticle
        if (animateTime > -1)
        {
            animateTime -= Time.deltaTime / animateLength;
        }
        else
        {
            animateTime = -1;
        }
    }
    #endregion
    #endregion DodgeVfx
}
