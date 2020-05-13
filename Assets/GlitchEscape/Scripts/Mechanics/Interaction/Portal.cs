using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider))]
public class Portal : AInteractiveObject
{
    public string interactMessage = "[Step through the portal]";
    public Loader.Scene levelToLoad = Loader.Scene.MainMenu;
    public FloatRange focusedPortalSpeed = new FloatRange { maximum = 3f, minimum = 1f };
    [InjectComponent] public SphereCollider collider;
    [InjectComponent] public MeshRenderer renderer;
    
    /// <summary>
    /// Enable / disable this portal
    /// </summary>
    public bool active {
        get => gameObject.activeInHierarchy;
        set {
            if (active == value) return;
            gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// Is this portal currently focused by a nearby player?
    /// </summary>
    public bool focused { get; private set; } = false;
    
    public override void OnInteract(Player player) {
        if (levelToLoad != Loader.Scene.None) {
            Application.LoadLevel(levelToLoad.ToString());
        }
    }
    public override void OnFocusChanged(bool focused) {
        this.focused = focused;
    }

    private const string PORTAL_SHADER_SPEED = "Vector1_5C84EC34";
    private const string PORTAL_SHADER_COLOR = "Color_FFD634D4";
    private const string PORTAL_SHADER_START_TIME = "StartTime";
    private const float PORTAL_SHADER_PERIOD = 360f;
    private Color portalShaderColor {
        get => renderer.material.GetColor(PORTAL_SHADER_COLOR);
        set => renderer.material.SetColor(PORTAL_SHADER_COLOR, value);
    }
    private float portalShaderSpeed {
        get => renderer.material.GetFloat(PORTAL_SHADER_SPEED);
        set {
            var current = portalShaderSpeed;
            if (value == current) return;
            var start = renderer.material.GetFloat(PORTAL_SHADER_START_TIME);
            var newStartWithOffsetForSpeed = (current / value) * (Time.time - start) - Time.time;
            
            renderer.material.SetFloat(PORTAL_SHADER_START_TIME, newStartWithOffsetForSpeed);
            renderer.material.SetFloat(PORTAL_SHADER_SPEED, value);
        }
    }
    
    private float defaultPortalSpeed;
    void Awake() {
        defaultPortalSpeed = portalShaderSpeed;
    }
    private void Update() {
        if (focused) {
            var player = PlayerController.instance?.player;
            if (player == null || collider == null) return;
            
            // get distance to player but only along the x / z plane
            var toPlayer = player.transform.position - transform.position;
            var distanceToPlayer = Mathf.Sqrt(toPlayer.x * toPlayer.x + toPlayer.z * toPlayer.z);
            
            // normalize against sphere collider radius + sample from min / max focusedPortalSpeed
            var normalizedDistance = 1f - Mathf.Clamp01(distanceToPlayer / collider.radius * 1.5f);
            var portalSpeedWhenFocused = focusedPortalSpeed.Lerp(normalizedDistance);
            portalShaderSpeed = portalSpeedWhenFocused;
        }
    }
}
