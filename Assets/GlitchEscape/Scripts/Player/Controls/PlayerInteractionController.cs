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
        
        // old maze trigger implementation
        OnMazeTriggerInteractPressed();
    }
    
    #region OldMazeTriggerImplementation
    private bool onSwitch = false;
    private float lastMazeSwitchTime = -10f;
    private Transform lastSwitchTransform = null;

    [Range(0f, 1f)] public float mazeSwitchCooldown = 0.2f;

    void OnMazeTriggerInteractPressed() {
        if (onSwitch && Time.time >= lastMazeSwitchTime + mazeSwitchCooldown) {
            lastMazeSwitchTime = Time.time;
            playerController.SetSavePoint(lastSwitchTransform);
            playerController.SwitchMazes();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Switch")
        {
            onSwitch = true;
            lastSwitchTransform = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Switch")
        {
            onSwitch = false;
        }
    }
    #endregion
}
