using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public interface IPlayerInteractable {
    void OnInteract(Player player);
    void OnPlayerEnterInteractionRadius(Player player);
    void OnPlayerExitInteractionRadius(Player player);
    Transform transform { get; }
    bool isInteractive { get; }
    void OnSelected(Player player);
    void OnDeselected(Player player);
}