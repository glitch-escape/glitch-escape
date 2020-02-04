using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementController : MonoBehaviour, IPlayerControllerComponent {

    // Object references (null-checks implemented elsewhere, so can assume non-null)
    private PlayerController controller;
    private Player    player;
    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private Animator  playerAnimator;
    private Input playerInput;
    
    // Initialize player references
    public void SetupControllerComponent(PlayerController controller) {
        this.controller = controller;
        player = controller.player;
        playerTransform = player.transform;
        playerRigidbody = player.rigidbody;
        playerAnimator = player.animator;
        playerInput = player.input;
    }
    
    // Public properties

    [Tooltip("Player movement speed (meters / second)")]
    public float moveSpeed;
    
    [Tooltip("Player movement speed (degrees / second)")]
    public float turnSpeed;
    
    [Tooltip("Player movement mode")] 
    public PlayerMovementMode movementMode = PlayerMovementMode.TurnToFaceMoveDirection;
    public enum PlayerMovementMode {
        TurnToFaceMoveDirection,
        Strafing
    }

    [Tooltip("Use animator for move speed?")]
    public bool useAnimationDerivedMoveSpeed = true;
    
    // Property getters
    private float currentAnimationSpeed => playerAnimator.deltaPosition.magnitude;
    public float actualMoveSpeed => useAnimationDerivedMoveSpeed ? currentAnimationSpeed : moveSpeed;

    // Input values
    private Vector2 moveInput => playerInput.Controls.Move.ReadValue<Vector2>();
    private bool hasMoveInput => moveInput.magnitude > 1e-6;
    private Vector3 moveInputRelativeToCamera {
        get {
            var cameraTransform = controller.camera.transform;
            var forward = cameraTransform.forward;
            var right   = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            var input = moveInput;
            return forward * input.y + right * input.x;
        }
    }
    // Update + animator callbacks
    void OnAnimatorMove() {
        if (useAnimationDerivedMoveSpeed) {
            var speed = playerAnimator.deltaPosition.magnitude;
            Move(speed);
        }
    }
    void FixedUpdate() {
        playerAnimator.SetBool("isSprinting", hasMoveInput);
        if (!useAnimationDerivedMoveSpeed) {
            Move(moveSpeed);
        }
    }
    private void Move (float speed) {
        var cameraDir = moveInputRelativeToCamera;
        switch (movementMode) {
            case PlayerMovementMode.TurnToFaceMoveDirection: {
                var desiredForward = Vector3.RotateTowards(
                    playerTransform.forward,
                    cameraDir,
                    turnSpeed * Time.deltaTime, 
                    0f);
                var playerRotation = Quaternion.LookRotation(desiredForward);
                
                playerRigidbody.MovePosition(playerRigidbody.position + cameraDir * speed * Time.deltaTime);
                playerRigidbody.MoveRotation(playerRotation);
            } break;
            case PlayerMovementMode.Strafing: {
                /* TODO: implement strafing controls */
            } break;
        }
    }
}
