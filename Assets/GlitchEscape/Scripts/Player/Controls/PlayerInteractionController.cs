using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour, IPlayerControllerComponent {

    private PlayerController playerController;
    
    public void SetupControllerComponent(PlayerController controller) {
        playerController = controller;
        playerController.player.input.Controls.Interact.performed += OnInteract;
    }
    private bool onSwitch = false;
    private float lastMazeSwitchTime = -10f;
    private Transform lastSwitchTransform = null;

    [Range(0f, 1f)] public float mazeSwitchCooldown = 0.2f;
    
    public void OnInteract(InputAction.CallbackContext context) {
        if (context.performed && onSwitch && Time.time >= lastMazeSwitchTime + mazeSwitchCooldown) {
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


}
