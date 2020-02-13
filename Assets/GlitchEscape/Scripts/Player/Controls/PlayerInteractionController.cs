using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour, IPlayerControllerComponent {

    private PlayerController playerController;
    private Player player;

    public void SetupControllerComponent(PlayerController controller) {
        player = controller.player;
        player.input.Controls.Interact.performed += OnInteract;
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if (!context.performed) return;

        // notify player interact listeners
        player.interactListeners(player);
    }
}
