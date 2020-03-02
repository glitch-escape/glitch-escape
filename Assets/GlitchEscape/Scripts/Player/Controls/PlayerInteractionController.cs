using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour, IPlayerControllerComponent {
    private Player player;
    public void SetupControllerComponent(PlayerController controller) {
        player = controller.player;
    }
    private void OnEnable() {
        PlayerControls.instance.interact.onPressed += OnInteract;
    }
    private void OnDisable() {
        PlayerControls.instance.interact.onPressed -= OnInteract;
    }
    public void OnInteract(bool pressed, PlayerControls.HybridButtonControl control) {
        // notify player interact listeners
        player.interactListeners(player);
    }
}
