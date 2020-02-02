using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour {
    
    // Getters / accessors
    [HideInInspector]
    public PlayerController controller; // set by controller.Awake()
    
    public new Rigidbody rigidbody {
        get {
            if (m_rigidbody) return m_rigidbody;
            m_rigidbody = GetComponent<Rigidbody>();
            if (!m_rigidbody) { Debug.LogError("Player missing Rigidbody!"); }
            return m_rigidbody;
        }
    }
    private Rigidbody m_rigidbody = null;

    public Animator animator {
        get {
            if (m_animator) return m_animator;
            m_animator = GetComponent<Animator>();
            if (!m_animator) { Debug.LogError(("Player missing Animator!")); }
            return m_animator;
        }
    }
    private Animator m_animator;
    
    // input instance singleton
    public Input input => m_input ?? (m_input = new Input());
    private Input m_input;

    void Awake() {
        input.Enable();
    }
}
