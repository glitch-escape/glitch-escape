using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

    public float acceleration;
    public float speed, damage;
    public float lifetime;
    public float growthRate;

    private Vector3 direction;

    public new Rigidbody rigidbody {
        get {
            if (m_rigidbody) return m_rigidbody;
            m_rigidbody = GetComponent<Rigidbody>();
            if (!m_rigidbody) { Debug.LogError("Projectile missing Rigidbody!"); }
            return m_rigidbody;
        }
    }
    private Rigidbody m_rigidbody;

    void Awake() {
        m_rigidbody = rigidbody;
        direction = transform.rotation * Vector3.forward;

        Destroy(gameObject, lifetime);
        m_rigidbody.velocity = direction * speed;
    }

    void Update() {
        m_rigidbody.AddForce(direction * acceleration);
        Vector3 growth = Vector3.one * growthRate * Time.deltaTime;
        transform.localScale += growth; 
    }

    void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<Player>();
        if (player != null) { player.TakeDamage(damage); }
    }


    
}
