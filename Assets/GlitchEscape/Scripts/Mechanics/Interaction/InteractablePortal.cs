using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider))]
public class InteractablePortal : AInteractiveObject
{
    public string interactMessage = "[Step through the portal]";
    public Loader.Scene levelToLoad = Loader.Scene.MainMenu;
    private float defaultPortalSpeed;
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

    public float distanceToPlayer {
        get {
            var player = PlayerController.instance?.player;
            return player != null ? Vector3.Distance(player.transform.position, transform.position) : Mathf.Infinity;
        }
    }

    void Awake() {
        defaultPortalSpeed = renderer.material.GetFloat(PORTAL_SHADER_SPEED);
    }
    public override void OnInteract(Player player) {
        if (levelToLoad != Loader.Scene.None) {
            Application.LoadLevel(levelToLoad.ToString());
        }
    }
    public override void OnFocusChanged(bool focused) {
        this.focused = focused;
        if (!focused) renderer.material.SetFloat(PORTAL_SHADER_SPEED, defaultPortalSpeed);
    }

    private const string PORTAL_SHADER_SPEED = "Vector1_5C84EC34";
    private const string PORTAL_SHADER_COLOR = "Color_FFD634D4";
    public float speed;
    
    private void Update() {
        if (focused) {
            var player = PlayerController.instance.player;
            if (player == null || collider == null) return;
            var distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            var normalizedDistance = 1f - distanceToPlayer / collider.radius;
            var portalSpeedWhenFocused = focusedPortalSpeed.Lerp(normalizedDistance);
            speed = portalSpeedWhenFocused;
            renderer.material.SetFloat(PORTAL_SHADER_SPEED, portalSpeedWhenFocused);
        }
    }
}
