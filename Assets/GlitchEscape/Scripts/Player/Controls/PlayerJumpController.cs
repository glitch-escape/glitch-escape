using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpController : MonoBehaviour, IPlayerControllerComponent
{
    private PlayerController controller;
    private new Rigidbody rigidbody;
    private Animator animator;
    private Player player;
    public void SetupControllerComponent(PlayerController controller)
    {
        this.controller = controller;
        player = controller.player;
        rigidbody = player.rigidbody;
        animator = player.animator;
        player.input.Controls.Jump.performed += OnJump;
    }

    [Tooltip("jump height (meters). Inaccurate if gravity factors != 1")]
    public float jumpHeight = 2f;

    [Tooltip("factor to increase wall jump height by")]
    public float wallJumpMultiplier = 1.5f;

    [Tooltip("maximum jumps (2 = double jump, etc)")]
    public uint maxJumpCount = 2;
    public uint jumpCount = 0;

    private float jumpStartTime = 0f;
    private bool isJumping = false;

    public AnimationCurve jumpCurve;
    public float jumpDuration = 0.64f;

    [Tooltip("factor we increase downwards gravity by")]
    public float downGravityFactor = 2.2f;

    [Tooltip("factor we increase upwards gravity by")]
    public float upGravityFactor = 0.8f;

    private float jumpVelocity;

    //used to ensure that the player doest wall jump from the same wall
    private Vector3 lastWallNormal = Vector3.zero;
    private Vector3 currentWallNormal = Vector3.zero;

    private Vector2 moveInput => player.input.Controls.Move.ReadValue<Vector2>();

    private Vector3 moveInputRelativeToCamera
    {
        get
        {
            var cameraTransform = controller.camera.transform;
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
    }

    void Update()
    {
        if (jumpCount > 0 && CheckOnGround())
        {
            jumpCount = 0;
            if (isJumping)
            {
                isJumping = false;
            }
            if (animator.GetBool("isJumping"))
            {
                animator.SetBool("isJumping", false);
                animator.SetTrigger("stopJumping");
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        bool wallCheck = CheckOnWall();
        if (!wallCheck) { Debug.Log(wallCheck); }
        if (context.performed && jumpCount + 1 < maxJumpCount && !wallCheck) //not wall jump
        {
            ++jumpCount;
            rigidbody.velocity += new Vector3(0, jumpVelocity, 0);
        }
        else if (context.performed && wallCheck) //wall jump
        {
            //this is where wall jump implementation WILL go (but not yet)
            jumpCount = 0;
            rigidbody.velocity += new Vector3(0, jumpVelocity * wallJumpMultiplier, 0) + (currentWallNormal * jumpVelocity);
        }
    }

    void FixedUpdate()
    {
        if (isJumping)
        {
            if (Time.time >= jumpStartTime + jumpDuration)
            {
                isJumping = false;
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
        bool wallHit = Physics.Raycast(rigidbody.position + Vector3.up, player.transform.forward, out wallInfo, 1.5f);
        if (wallInfo.collider == null)
            return false;
        lastWallNormal = currentWallNormal;
        currentWallNormal = wallInfo.normal;
        return true;
    }
}