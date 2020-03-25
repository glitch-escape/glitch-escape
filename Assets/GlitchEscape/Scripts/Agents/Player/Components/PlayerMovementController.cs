using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementController : PlayerComponent {
    [InjectComponent] public new Rigidbody rigidbody;
    [InjectComponent] public Animator animator;
    [InjectComponent] public PlayerControls playerInput;
    public Camera camera;
    
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

    [Tooltip("Time (s) for player to be stunned after knockback occurs")]
    public float stunTime;
    private float currentStunTime = 0;

    [Tooltip("Use animator for move speed?")]
    public bool useAnimationDerivedMoveSpeed = true;
    
    // Property getters
    private float currentAnimationSpeed => animator.deltaPosition.magnitude;
    public float actualMoveSpeed => useAnimationDerivedMoveSpeed ? currentAnimationSpeed : moveSpeed;

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
    // Update + animator callbacks
    void OnAnimatorMove() {
        if (useAnimationDerivedMoveSpeed) {
            var speed = animator.deltaPosition.magnitude;
            Move(speed);
        }
    }

    private bool wasRunningLastFrame = false;
    void FixedUpdate() {
        animator.SetBool("isRunning", hasMoveInput);
        if (hasMoveInput != wasRunningLastFrame) {
            animator.SetTrigger(hasMoveInput ? "startRunning" : "stopRunning");
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
            rigidbody.velocity = Vector3.zero;
        }
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
        
    }
}
