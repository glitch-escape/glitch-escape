using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: split out into separate class
public class Trigger : MonoBehaviour {
    public delegate void TriggerEvent(GameObject gameObject);
    public event TriggerEvent OnEnter;
    public event TriggerEvent OnExit;
    public event TriggerEvent OnStay;

    private void OnTriggerEnter(Collider other) {
        OnEnter?.Invoke(other.gameObject);
    }
    private void OnTriggerExit(Collider other) {
        OnExit?.Invoke(other.gameObject);
    }
    private void OnTriggerStay(Collider other) {
        OnStay?.Invoke(other.gameObject);
    }
}

// TODO: split out into separate class
public class SphereTrigger : Trigger {
    private SphereCollider _collider = null;
    public SphereCollider collider => _collider ??
                                      (_collider = GetComponent<SphereCollider>() ??
                                                   gameObject.AddComponent<SphereCollider>());
    public float radius {
        get => collider.radius;
        set => collider.radius = value;
    }
    public static SphereTrigger GetOrCreateInChildren (GameObject obj) {
        var instance = obj.GetComponentInChildren<SphereTrigger>();
        if (instance != null) return instance;
        var emptyChild = new GameObject("Trigger", typeof(SphereCollider), typeof(SphereTrigger));
        var trigger = emptyChild.GetComponent<SphereTrigger>();
        var collider = trigger.collider;
        collider.isTrigger = true;
        collider.center = Vector3.zero;
        return trigger;
    }
    private void OnDisable() {
        _collider = null;
    }
}
public class PlayerInteractionAblity : PlayerAbility {
    public override float resourceCost => 0f;
    public override float cooldownTime => 0.1f;
    protected override float abilityDuration => 0f;
    protected override PlayerControls.HybridButtonControl inputButton => controls.interact;

    [InjectComponent] public PlayerControls controls;
    private HashSet<IPlayerInteractable> interactablesInRange = new HashSet<IPlayerInteractable>();
    
    public T GetNearestObject<T>() where T : MonoBehaviour, IPlayerInteractable {
        T nearest = null;
        float distance = Mathf.Infinity;
        foreach (var interactive in interactablesInRange) {
            switch (interactive) {
                case T obj: {
                    var dist = Vector3.Distance(obj.transform.position, transform.position);
                    if (dist < distance) {
                        distance = dist;
                        nearest = obj;
                    }
                } break;
                default: break;
            }
        }
        return nearest;
    }
    private void OnEnter (GameObject obj) {
        var interactObj = obj.GetComponent<IPlayerInteractable>();
        if (interactObj != null) {
            interactablesInRange.Add(interactObj);
            interactObj.OnPlayerEnterInteractionRadius(player);
        }
    }
    private void OnExit(GameObject obj) {
        var interactObj = obj.GetComponent<IPlayerInteractable>();
        if (interactObj != null) {
            interactablesInRange.Remove(interactObj);
            interactObj.OnPlayerExitInteractionRadius(player);
        }
    }
    private void Reset() {
        interactablesInRange.Clear();
    }

    private SphereTrigger trigger = null;
    private float lastInteractionRadius = Mathf.Infinity;
    private void SetTriggerRadius(float radius) {
        trigger.radius = lastInteractionRadius = radius; 
    }
    // update trigger collider radius in real time
    protected override void Update() {
        if (player.config.interactionRadius != lastInteractionRadius) {
            SetTriggerRadius(player.config.interactionRadius);
        }
        base.Update();
    }
    private void OnEnable() {
        if (trigger == null) {
            trigger = SphereTrigger.GetOrCreateInChildren(gameObject);
            SetTriggerRadius(player.config.interactionRadius);
        }
        trigger.OnEnter += OnEnter;
        trigger.OnExit += OnExit;
    }
    private void OnDisable() {
        trigger.OnEnter -= OnEnter;
        trigger.OnExit -= OnExit;
        trigger = null;
    }
    protected override void OnAbilityStart() {
        // TODO: consider whether we should move interaction impl here...?
        // (see IPlayerInteractable, InteractionTrigger)
    }
}
