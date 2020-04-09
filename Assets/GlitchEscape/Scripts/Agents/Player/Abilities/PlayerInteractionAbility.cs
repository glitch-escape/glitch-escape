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
        emptyChild.transform.parent = obj.transform;
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

public class PlayerInteractionAbility : PlayerAbility {
    public override float resourceCost => 0f;
    public override float cooldownTime => 0.1f;
    protected override float abilityDuration => 0f;
    protected override PlayerControls.HybridButtonControl inputButton => controls.interact;

    [InjectComponent] public PlayerControls controls;

    private HashSet<IInteract> interactablesInRange = new HashSet<IInteract>();
    private HashSet<IActiveInteract> activeInRange = new HashSet<IActiveInteract>();

    private IActiveInteract lastNearestObject = null;

    protected override void OnAbilityStart()
    {
        Debug.Log("Ability Started");
        var obj = GetNearestObject<IActiveInteract>(activeInRange);
        if (obj != null && activeInRange.Contains(obj) && obj == lastNearestObject)
        {
            obj.OnInteract(player);
        }

    }

    public T GetNearestObject<T>(HashSet<T> interactInRange) where T : IInteract {
        T nearest = default(T);
        float distance = Mathf.Infinity;
        foreach (var interactObject in interactInRange) {

            switch (interactObject)
            {
                case T obj:
                    {
                        var dist = Vector3.Distance(obj.transform.position, transform.position);
                        if (dist < distance)
                        {
                            distance = dist;
                            nearest = obj;
                        }
                    }
                    break;
                default: break;
            }
        }
        return nearest;
    }

    protected override void Update() { 
        base.Update();

        var nearest = GetNearestObject<IActiveInteract>(activeInRange);
        if (nearest != lastNearestObject) {
            lastNearestObject?.OnDeselected(player);
            nearest?.OnSelected(player);
            lastNearestObject = nearest;
        }

        // update trigger collider radius in real time, if it changes
        if (Math.Round(player.config.interactionRadius, 2) != Math.Round(lastInteractionRadius, 2)) {
            SetTriggerRadius(player.config.interactionRadius);
        }
    }
    private void OnEnter (GameObject obj) {
        var interactObj = obj.GetComponent<IInteract>();
        if (interactObj != null) {
            interactablesInRange.Add(interactObj);
            interactObj.OnPlayerEnterInteractionRadius(player);
        }
    }
    private void OnExit(GameObject obj) {
        var interactObj = obj.GetComponent<IInteract>();
        if (interactObj != null) {
            interactablesInRange.Remove(interactObj);
            interactObj.OnPlayerExitInteractionRadius(player);
        }
    }
    private void Reset() {
        interactablesInRange.Clear();
        activeInRange.Clear();
        lastNearestObject?.OnDeselected(player);
        lastNearestObject = null;
    }
    private void OnEnable() {
        if (trigger == null) {
            // get sphere trigger (and set its radius)
            trigger = SphereTrigger.GetOrCreateInChildren(gameObject);
            SetTriggerRadius(player.config.interactionRadius);
        }
        // listen to on enter + on exit trigger events
        trigger.OnEnter += OnEnter;
        trigger.OnExit += OnExit;
    }
    private void OnDisable() {
        // unregister events + clear trigger (in case something reloads + refs break)
        trigger.OnEnter -= OnEnter;
        trigger.OnExit -= OnExit;
        trigger = null;
        lastNearestObject?.OnDeselected(player);
        lastNearestObject = null;
    }

    private SphereTrigger trigger = null;
    private float lastInteractionRadius = Mathf.Infinity;
    private void SetTriggerRadius(float radius) {
        trigger.radius = lastInteractionRadius = radius; 
    }
}
