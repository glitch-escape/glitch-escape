using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        other.GetComponent<Player>()?.spawn.SetSpawnPosition(this);
    }
}
