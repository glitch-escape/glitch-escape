using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class SavePoint : MonoBehaviour, IPlayerInteractable {
    public void OnInteract(Player player) {}
    public void OnPlayerEnterInteractionRadius(Player player) {
        player.spawn.SetSpawnPosition(this);
    }
    public void OnPlayerExitInteractionRadius(Player player) {}
}
