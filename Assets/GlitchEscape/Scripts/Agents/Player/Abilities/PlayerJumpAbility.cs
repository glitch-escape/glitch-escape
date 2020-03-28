using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpAbility : PlayerComponent {
    [InjectComponent] public new Rigidbody rigidbody;
    [InjectComponent] public Animator animator;
    [InjectComponent] public new Camera camera;

    // TODO: move to PlayerConfig
    [Tooltip("jump height (meters). Inaccurate if gravity factors != 1")]
    public float jumpHeight = 2f;

    // TODO: move to PlayerConfig
    [Tooltip("factor to increase wall jump height by")]
    public float wallJumpMultiplier = 1.5f;

    // TODO: move to PlayerConfig
    [Tooltip("maximum jumps (2 = double jump, etc)")]
    public uint maxJumpCount = 2;
    public uint jumpCount = 0;

    // TODO: move to PlayerConfig
    private float jumpStartTime = 0f;
    private bool isJumping = false;

    // TODO: move to PlayerConfig
    public AnimationCurve jumpCurve;
    
    // TODO: move to PlayerConfig
    public float jumpDuration = 0.64f;

    // TODO: move to PlayerConfig
    [Tooltip("factor we increase downwards gravity by")]
    public float downGravityFactor = 2.2f;

    // TODO: move to PlayerConfig
    [Tooltip("factor we increase upwards gravity by")]
    public float upGravityFactor = 0.8f;

    // TODO: move to PlayerConfig
    private float jumpVelocity;

    // TODO: move to PlayerConfig
    //used to ensure that the player doest wall jump from the same wall
    private Vector3 lastWallNormal = Vector3.zero;
    private Vector3 currentWallNormal = Vector3.zero;
    
    private LayerMask walls;

    private Vector2 moveInput => PlayerControls.instance.moveInput;

    private Vector3 moveInputRelativeToCamera
    {
        get
        {
            var cameraTransform = camera.transform;
            var forward = cameraTransform.forward;
            var right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            var input = moveInput;
            return forward * input.y + right * input.x;
        }
    }

    void Awake()
    {
        jumpVelocity = (float)Math.Sqrt(2 * upGravityFactor * -Physics.gravity.y * jumpHeight);
        walls = LayerMask.GetMask("Wall");
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        // TODO: refactor this
        bool wallCheck = CheckOnWall();
        if (context.performed && jumpCount + 1 < maxJumpCount && !wallCheck) //not wall jump
        {
            jumpCount++;
            if (rigidbody.velocity.y < 0f) {
                rigidbody.velocity = Vector3.up * jumpVelocity;
            } else {
                rigidbody.velocity += Vector3.up * jumpVelocity;
            }
            FireEvent(CheckOnGround() ? PlayerEvent.Type.FloorJump : PlayerEvent.Type.AirJump);
        }
        else if (context.performed && wallCheck && !CheckOnGround() && (lastWallNormal != currentWallNormal)) //wall jump
        {
            jumpCount = 0;
            if (rigidbody.velocity.y < 0f) { 
                rigidbody.velocity = Vector3.up * jumpVelocity * wallJumpMultiplier + currentWallNormal * jumpVelocity;
            } else {
                rigidbody.velocity += Vector3.up * jumpVelocity * wallJumpMultiplier + currentWallNormal * jumpVelocity;
            }
            FireEvent(PlayerEvent.Type.WallJump);
        }
        lastWallNormal = currentWallNormal;
    }

    void FixedUpdate()
    {
        // TODO: refactor this
        if(CheckOnGround())
        {
            if (jumpCount > 0)
            {
                jumpCount = 0;
                if (isJumping)
                {
                    isJumping = false;
                    FireEvent(PlayerEvent.Type.FloorJump);
                }
                if (animator.GetBool("isJumping"))
                {
                    animator.SetBool("isJumping", false);
                    animator.SetTrigger("stopJumping");
                }
            }
            currentWallNormal = Vector3.zero;
        }
        
        if (isJumping)
        {
            if (Time.time >= jumpStartTime + jumpDuration)
            {
                isJumping = false;            
                FireEvent(CheckOnGround() ? PlayerEvent.Type.FloorJump : PlayerEvent.Type.AirJump);
            }
            else
            {
                // ApplyJumpStep();
            }
        }
        if (rigidbody.velocity.y != 0f && !CheckOnGround())
        {
            if (rigidbody.velocity.y <= 0f) rigidbody.velocity += Physics.gravity * (downGravityFactor - 1f) * Time.fixedDeltaTime;
            else rigidbody.velocity -= Physics.gravity * (upGravityFactor - 1f) * Time.fixedDeltaTime;
        }
    }

    public bool CheckOnGround() => Physics.Raycast(rigidbody.position, Vector3.down, .7f);

    public bool CheckOnWall()
    {
        RaycastHit wallInfo;
        bool wallHit = Physics.Raycast(rigidbody.position + Vector3.up, player.transform.forward, out wallInfo, 1f, walls);
        if (wallInfo.collider == null)
        {
            currentWallNormal = Vector3.zero;
            return false;
        }
        lastWallNormal = currentWallNormal;
        currentWallNormal = wallInfo.normal;
        return true;
    }
}
