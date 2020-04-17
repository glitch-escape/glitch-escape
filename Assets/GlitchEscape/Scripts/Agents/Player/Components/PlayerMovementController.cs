using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementController : PlayerComponent {
    [InjectComponent] public new Rigidbody rigidbody;
    [InjectComponent] public PlayerControls playerInput;
    [InjectComponent] public new Camera camera;
    [InjectComponent] public PlayerAnimationController playerAnimation;
    
    // Public properties

    private float moveSpeed => player.config.runSpeed;
    private float turnSpeed => player.config.turnSpeed;
    
    [Tooltip("Player movement mode")] 
    public PlayerMovementMode movementMode = PlayerMovementMode.TurnToFaceMoveDirection;
    public enum PlayerMovementMode {
        TurnToFaceMoveDirection,
        Strafing,
        Stunned
    }

    public float actualMoveSpeed => useAnimationDerivedMoveSpeed ? 
        playerAnimation.currentAnimationSpeed : moveSpeed;

    
    [Tooltip("Time (s) for player to be stunned after knockback occurs")]
    public float stunTime = 1f;
    private float currentStunTime = 0;

    private const bool useAnimationDerivedMoveSpeed = false;
    
    // Property getters
    public bool isMoving => hasMoveInput;
    
    // Input values
    private Vector2 moveInput => playerInput.moveInput;
    private bool hasMoveInput => moveInput.magnitude > 1e-6;
    private Vector3 moveInputRelativeToCamera {
        get {
            var cameraTransform = camera.transform;
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
    private bool wasRunningLastFrame = false;
    void FixedUpdate() {
        if (isMoving != wasRunningLastFrame) {
            FireEvent(isMoving ?
                PlayerEvent.Type.BeginMovement :
                PlayerEvent.Type.EndMovement);
        }
        wasRunningLastFrame = isMoving;
        
        if (!useAnimationDerivedMoveSpeed) {
            Move(moveSpeed);
        }

        // if(movementMode == PlayerMovementMode.Stunned)
        // {
        //     currentStunTime += Time.fixedDeltaTime;
        // }
        // if(currentStunTime >= stunTime)
        // {
        //     movementMode = PlayerMovementMode.TurnToFaceMoveDirection;
        //     currentStunTime = 0;
        //     rigidbody.velocity = Vector3.zero;
        // }
    }
    private void Move (float speed) {
        var cameraDir = moveInputRelativeToCamera;
        switch (movementMode) {
            case PlayerMovementMode.TurnToFaceMoveDirection: {
                var desiredForward = Vector3.RotateTowards(
                    player.transform.forward,
                    cameraDir,
                    turnSpeed * Time.deltaTime, 
                    0f);
                var playerRotation = Quaternion.LookRotation(desiredForward);

                rigidbody.MovePosition(rigidbody.position + cameraDir * speed * Time.deltaTime);
                //Vector3 tempVelocity = rigidbody.velocity;
                //tempVelocity = cameraDir * speed * Time.deltaTime * 30;
                //rigidbody.velocity = new Vector3(tempVelocity.x, rigidbody.velocity.y, tempVelocity.z);
                rigidbody.MoveRotation(playerRotation);
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
            rigidbody.velocity += -transform.forward * 3;
        }
    }

    public bool showDebugGui = false;

    void OnGUI() {
        if (!showDebugGui) return;
        GUILayout.Label("PlayerMovementController.cs:");
        GUILayout.Label("Movement mode: " + movementMode);
        GUILayout.Label("Raw player input: " + moveInput);
        GUILayout.Label("has player input? " + hasMoveInput);
        GUILayout.Label("camera-relative input: " + moveInputRelativeToCamera);
        GUILayout.Label("rigidbody velocity: " + rigidbody.velocity);
        GUILayout.Label("expected delta-v: " + (moveInputRelativeToCamera * actualMoveSpeed * Time.deltaTime));
        GUILayout.Label("expected position: " + (moveInputRelativeToCamera * actualMoveSpeed * Time.deltaTime + rigidbody.position));
        GUILayout.Label("player move speed: " + actualMoveSpeed);
        GUILayout.Label("player turn speed: " + turnSpeed);
        GUILayout.Label("config move speed: " + moveSpeed);
    }
}
