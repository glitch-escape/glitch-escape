using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour, IPlayerControllerComponent {
    private Player player;
    public void SetupControllerComponent(PlayerController controller) {
        player = controller.player;
    }
}
