using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour {
    [Tooltip("optional, if not set save point origin will be set to this object itself")]
    public Transform savePointTarget;
    private void OnTriggerEnter(Collider other) {
        other.GetComponent<Player>()?.spawn.SetSpawnPosition(this);
    }
}
