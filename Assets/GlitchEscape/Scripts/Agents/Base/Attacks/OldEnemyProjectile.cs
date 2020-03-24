using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OldEnemyProjectile : MonoBehaviour {

    public enum Target { Enemy, Player };
    public Target target;

    public float acceleration;
    public float speed, damage;
    public float lifetime;
    public float growthRate;
    //public float tracking; // in degrees for easier adjustments

    private Vector3 direction;
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
       // tracking *= Mathf.Deg2Rad;
        m_rigidbody = rigidbody;
        Destroy(gameObject, lifetime);

        // Get the projectile moving
        direction = transform.forward;
        direction.y = 0;
        m_rigidbody.velocity = Vector3.Normalize(direction) * speed;
    }

    void Update() {
        // Apply acceleration
        if (acceleration != 0f)
            m_rigidbody.AddForce(direction * acceleration * Time.deltaTime);
        // Apply projectile size growth
        if (growthRate != 0f) {
            Vector3 growth = Vector3.one * growthRate * Time.deltaTime;
            transform.localScale += growth;
        }
    }

    // Deal damage to player
    void OnTriggerEnter(Collider other) {
        if (target == Target.Player) {
            var player = other.GetComponent<Player>();
            if (player != null) { player.TakeDamage(damage); }
        }
        else if (target == Target.Enemy) {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null) { enemy.TakeDamage(damage); Debug.Log("got em"); }
        }
    }

    public void SetPlayerPos(Transform player) { }
}
