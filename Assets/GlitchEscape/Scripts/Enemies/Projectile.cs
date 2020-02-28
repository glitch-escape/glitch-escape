using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Vector3 direction;
    public float speed, damage;
    public float lifetime;

    void Awake() {
        Destroy(gameObject, lifetime);
    }

    void Update() {
        transform.Translate(direction.x * speed, 0, direction.z * speed);
    }

    void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<Player>();
        if (player != null) { player.TakeDamage(damage); }
    }

    public void SetDirection(Vector3 dir) {
        direction = dir;
    }
}
