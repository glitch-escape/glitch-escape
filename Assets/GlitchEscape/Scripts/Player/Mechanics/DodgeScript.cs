using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class DodgeScript : MonoBehaviour
{
    const float GRAVITY = 9.81f; // m/s^2
    
    private Input input;
    private Rigidbody m_Rigidbody;
    
    #region InputCallbacks
    
    // is the dodge button currently pressed?
    private bool dodgePressed = false;
    void SetupInputActions() {
        if (input == null) {
            input = new Input();
            input.Controls.Dodge.performed += context => {
                bool pressed = context.ReadValue<float>() > 0f;
                // Debug.Log("dodge pressed!" + pressed);
                dodgePressed = pressed;
            };
        }
    }
    #endregion

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
                transform.Translate(moveDir * dodgeSpeed * Time.deltaTime);
            } else {
                m_Rigidbody.velocity = transform.rotation * moveDir * dodgeSpeed;
            }
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
        
        // if already dodging, end that + restart
        if (isDodging) {
            EndDodge();
        }
        
        // begin dodge
        // Debug.Log("Start dodge!");
        BeginDodgeVfx();
        
        // save velocity
        savedDodgeVelocity = m_Rigidbody.velocity;
        
        if (useKinematic) {
            m_Rigidbody.isKinematic = true;
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
            m_Rigidbody.velocity = savedDodgeVelocity +
                                   Vector3.down * GRAVITY * elapsedTime;
            if (useKinematic) {
                m_Rigidbody.isKinematic = false;
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
    void Awake() {
        SetupInputActions();
        m_Rigidbody = this.GetComponent<Rigidbody>();

        defaultMaterial = this.transform.Find("Body").GetComponent<Renderer>().material;
        dodgeShader = Shader.Find("Custom/TeleportEffect");
        defaultShader = Shader.Find("Custom/Toon");
    }
    private void BeginDodgeVfx() {
        //dodgeGroundParticle.Emit(1);
        dodgeAirParticle.Emit(30);
        dodgeGroundParticle.transform.position = transform.position;
        dodgeGroundParticle.Emit(1);
        defaultMaterial.shader = dodgeShader;
        defaultMaterial.SetTexture("_Noise", noiseTex);
    }
    private void EndDodgeVfx() {
        defaultMaterial.shader = defaultShader;
        animateTime = 1.0f;
        dodgeHoldTime = 0f;
    }
    private void UpdateDodgeVfx() {
        defaultMaterial.SetFloat("_AlphaThreshold", animateTime);
        dodgeHoldTime += Time.deltaTime;
        dodgeGroundParticle.transform.position = this.transform.position + (-transform.forward * dodgeHoldTime * dodgeScaleFactor);
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
    
    private void OnEnable() => input.Controls.Enable();

    private void OnDisable() => input.Controls.Disable();
}
