﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

    public float acceleration;
    public float speed, damage;
    public float lifetime;
    public float growthRate;
    public float tracking; // in degrees for easier adjustments

    private Vector3 direction;
    private Transform playerPos;
    private Vector3 origin;

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
        origin = transform.position;
        tracking *= Mathf.Deg2Rad;
        m_rigidbody = rigidbody;
        Destroy(gameObject, lifetime);

        // Get the projectile moving
        direction = transform.forward;
        direction.y = 0;
        m_rigidbody.velocity = Vector3.Normalize(direction) * speed;
    }

    void Update() {
        // Apply acceleration
        m_rigidbody.AddForce(direction * acceleration);

        // Apply projectile size growth
        Vector3 growth = Vector3.one * growthRate * Time.deltaTime;
        transform.localScale += growth;

        // Apply tracking
        direction = transform.forward;
        Vector3 targetDir = playerPos.position - origin;
        direction = Vector3.RotateTowards(direction, targetDir, tracking, 0);
        direction.y = 0;
        m_rigidbody.velocity = Vector3.Normalize(direction) * speed;
        
    }

    // Deal damage to player
    void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<Player>();
        if (player != null) { player.TakeDamage(damage); }
    }

    // Allows projectile to track player position
    public void SetPlayerPos(Transform player) {
        playerPos = player;
    }

    
}
