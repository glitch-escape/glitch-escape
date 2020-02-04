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
    private Input input;
    private new Rigidbody rigidbody;
    public void SetupControllerComponent(PlayerController controller) {
        this.controller = controller;
        player = controller.player;
        rigidbody = player.rigidbody;
        input = player.input;
        input.Controls.Dodge.performed += context => {
            bool pressed = context.ReadValue<float>() > 0f;
            dodgePressed = pressed;
        };
        defaultMaterial = player.transform.Find("Body").GetComponent<Renderer>().material;
        dodgeShader = Shader.Find("Custom/TeleportEffect");
        defaultShader = Shader.Find("Custom/Toon");
    }
    
    public float dashStaminaCost = 10f;
    
    // is the dodge button currently pressed?
    private bool dodgePressed = false;

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
            // update vfx
            UpdateDodgeVfx();
        }
    }                                                
    private void BeginDodge() {
        // do we have enough stamina to perform this action? if no, cancel
        if (!player.TryUseAbility(dashStaminaCost)) {
            return;
        }
        
        // check: can we dodge yet? if no, cancel
        if (Time.time < dodgeStartTime + dodgeCooldown) {
            // Debug.Log("dodge still on cooldown");
            return;
        }
        // check: are we moving? if no, cancel
        if (input.Controls.Move.ReadValue<Vector2>().magnitude == 0f) {
            return;
        }
        
        // if already dodging, end that + restart
        if (isDodging) {
            EndDodge();
        }
        
        // begin dodge
        // Debug.Log("Start dodge!");
        BeginDodgeVfx();
        
        // save velocity
        savedDodgeVelocity = rigidbody.velocity;
        
        if (useKinematic) {
            rigidbody.isKinematic = true;
        }
        isDodging = true; 
        dodgeStartTime = Time.time;
    }
    private void EndDodge() {
        if (isDodging) {
            // end dodge
            // Debug.Log("Stop dodge!");
            EndDodgeVfx();
            
            // reapply velocity, plus gravity over time spent dashing
            var elapsedTime = Time.time - dodgeStartTime;
            Debug.Log("Applying additional velocity change after " + elapsedTime + " seconds: "
                      + GRAVITY * elapsedTime);
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
        defaultMaterial.shader = dodgeShader;
        defaultMaterial.SetTexture("_Noise", noiseTex);
        dodgeGroundParticle.transform.rotation = this.transform.rotation;
    }
    private void EndDodgeVfx() {
        defaultMaterial.shader = defaultShader;
        animateTime = 1.0f;
        dodgeHoldTime = 0f;
        dodgeGroundParticle.transform.rotation = this.transform.rotation;
    }
    private void UpdateDodgeVfx() {
        defaultMaterial.SetFloat("_AlphaThreshold", animateTime);
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
