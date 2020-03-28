using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
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

    private GameObject triggerObject;
    private SphereCollider triggerCollider;
    private Trigger trigger;
    
    // update trigger collider radius in real time
    protected override void Update() {
        if (player.config.interactionRadius != lastInteractionRadius) { 
            triggerCollider.radius = lastInteractionRadius = player.config.interactionRadius;
        }
        base.Update();
    }
    private float lastInteractionRadius = Mathf.Infinity;

    private void OnEnable() {
        if (triggerObject == null) {
            triggerObject = new GameObject("PlayerInteractionTrigger", 
                    typeof(SphereCollider), typeof(Trigger));
            triggerObject.transform.parent = player.transform;
            var collider = triggerObject.GetComponent<SphereCollider>();
            
            trigger = triggerObject.GetComponent<Trigger>();
        }
        triggerCollider = triggerObject.GetComponent<SphereCollider>() ?? triggerObject.AddComponent<SphereCollider>();
        triggerCollider.center = Vector3.zero;
        triggerCollider.radius = lastInteractionRadius = player.config.interactionRadius;
        
        trigger = triggerObject.GetComponent<Trigger>() ?? triggerObject.AddComponent<Trigger>();
        trigger.OnEnter += OnEnter;
        trigger.OnExit += OnExit;
    }
    private void OnDisable() {
        trigger.OnEnter -= OnEnter;
        trigger.OnExit -= OnExit;
    }
    protected override void OnAbilityStart() {
        // TODO: consider whether we should move interaction impl here...?
        // (see IPlayerInteractable, InteractionTrigger)
    }
}
