using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementController : MonoBehaviorUsingConfig<Player, PlayerConfig>, IPlayerControllerComponent {

    // Object references (null-checks implemented elsewhere, so can assume non-null)
    private PlayerController controller;
    private Player    player;
    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private Animator  playerAnimator;
    private PlayerControls playerInput;
    
    // Initialize player references
    public void SetupControllerComponent(PlayerController controller) {
        this.controller = controller;
        player = controller.player;
        playerTransform = player.transform;
        playerRigidbody = player.rigidbody;
        playerAnimator = player.animator;
        playerInput = PlayerControls.instance;
        
        // clear velocity
        movementMode = PlayerMovementMode.TurnToFaceMoveDirection;
        playerRigidbody.velocity = Vector3.zero;
    }
    
    // Public properties

    private float moveSpeed => config.runSpeed;
    private float turnSpeed => config.turnSpeed;
    
    [Tooltip("Player movement mode")] 
    public PlayerMovementMode movementMode = PlayerMovementMode.TurnToFaceMoveDirection;
    public enum PlayerMovementMode {
        TurnToFaceMoveDirection,
        Strafing,
        Stunned
    }

    [Tooltip("Time (s) for player to be stunned after knockback occurs")]
    public float stunTime;
    private float currentStunTime = 0;

    [Tooltip("Use animator for move speed?")]
    public bool useAnimationDerivedMoveSpeed = true;
    
    // Property getters
    private float currentAnimationSpeed => playerAnimator.deltaPosition.magnitude;
    public float actualMoveSpeed => useAnimationDerivedMoveSpeed ? currentAnimationSpeed : moveSpeed;

    // Input values
    private Vector2 moveInput => playerInput.moveInput;
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

    private bool wasRunningLastFrame = false;
    void FixedUpdate() {
        playerAnimator.SetBool("isRunning", hasMoveInput);
        if (hasMoveInput != wasRunningLastFrame) {
            playerAnimator.SetTrigger(hasMoveInput ? "startRunning" : "stopRunning");
        }
        wasRunningLastFrame = hasMoveInput;
        
        if (!useAnimationDerivedMoveSpeed) {
            Move(moveSpeed);
        }

        if(movementMode == PlayerMovementMode.Stunned)
        {
            currentStunTime += Time.fixedDeltaTime;
        }
        if(currentStunTime >= stunTime)
        {
            movementMode = PlayerMovementMode.TurnToFaceMoveDirection;
            currentStunTime = 0;
            playerRigidbody.velocity = Vector3.zero;
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
                //Vector3 tempVelocity = playerRigidbody.velocity;
                //tempVelocity = cameraDir * speed * Time.deltaTime * 30;
                //playerRigidbody.velocity = new Vector3(tempVelocity.x, playerRigidbody.velocity.y, tempVelocity.z);
                playerRigidbody.MoveRotation(playerRotation);
            } break;
            case PlayerMovementMode.Strafing: {
                /* TODO: implement strafing controls */
            } break;
            case PlayerMovementMode.Stunned:{
                    //do nothing (for now)
            } break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Knockback"))
        {
            movementMode = PlayerMovementMode.Stunned;
            playerRigidbody.velocity += -transform.forward * 3;
        }
    }
}
