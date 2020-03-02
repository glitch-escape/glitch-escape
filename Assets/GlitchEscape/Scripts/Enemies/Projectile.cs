using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

     private Vector3 direction;
    public float acceleration;
    public float speed, damage;
    public float lifetime;

    void Awake() {
        Destroy(gameObject, lifetime);
        GetComponent<Rigidbody>().velocity = Vector3.forward * speed;
    }

    void Update() {
        GetComponent<Rigidbody>().AddForce(Vector3.forward * acceleration);

    }

    void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<Player>();
        if (player != null) { player.TakeDamage(damage); }
    }

    
    public void SetDirection(Vector3 dir) {
        direction = dir;
    }
    
}
