using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : MonoBehaviour {
    private Player player = null;
    private Transform prevParent = null;

    private void OnTriggerEnter(Collider other) {
        player = other.GetComponent<Player>();
        if (player != null) {
            prevParent = player.transform.parent;
            player.transform.parent = transform;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (player != null) {
            player.transform.parent = prevParent;
            prevParent = null;
            player = null;
        }
    }
}
