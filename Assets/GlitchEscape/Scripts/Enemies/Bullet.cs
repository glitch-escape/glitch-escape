using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Bullet : MonoBehaviour {

    private Vector3 direction;
    public float speed, damage;
    public float lifetime;

    void Awake() {
        Destroy(this, lifetime);
    }

    void Update() {
        transform.Translate(direction * speed);
    }

    void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<Player>();
        if (player != null) { player.TakeDamage(damage); }
    }

    public void SetDirection(Vector3 dir) {
        direction = dir;
    }
}
