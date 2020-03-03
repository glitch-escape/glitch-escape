﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class SavePoint : MonoBehaviour, IPlayerInteractable {
    public Transform spawnPoint;
    public void OnInteract(Player player) {}
    public void OnPlayerEnterInteractionRadius(Player player) {
        if (spawnPoint != null)
            player.SetInitialSpawnLocation(spawnPoint.position, spawnPoint.rotation);
    }
    public void OnPlayerExitInteractionRadius(Player player) {}
}
