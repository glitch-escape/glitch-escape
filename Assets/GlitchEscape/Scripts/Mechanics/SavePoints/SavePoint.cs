using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour {
    [Tooltip("optional, if not set save point origin will be set to this object itself")]
    public Transform savePointTarget;
    
    [Tooltip("set true iff this is a platform and we're using a collider instead of a trigger")]
    public bool isPlatformCollider = false;
    
    [Tooltip("vertical raycast height offset (ray from center of object pointing down) used for above case")]
    public float platformRaycastHeightOffset = 5f;
    private void OnTriggerEnter(Collider other) {
        if (isPlatformCollider) return;
        other.GetComponent<Player>()?.spawn.SetSpawnPosition(this);
    }
    private void OnCollisionEnter(Collision other) {
        if (!isPlatformCollider) return;
        other.gameObject.GetComponent<Player>()?.spawn.SetSpawnPosition(this);
    }
}
