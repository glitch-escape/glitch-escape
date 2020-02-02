using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathTrigger : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<Player>();
        if (player != null) {
            player.KillPlayer();
        }
    }
}
