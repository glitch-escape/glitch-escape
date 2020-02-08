using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInteractable {
    void OnInteract(Player player);
    void OnPlayerEnterInteractionRadius(Player player);
    void OnPlayerExitInteractionRadius(Player player);
}