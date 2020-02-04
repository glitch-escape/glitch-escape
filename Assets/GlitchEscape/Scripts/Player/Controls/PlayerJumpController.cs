using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Jump implementation
// Currently:
// - player can jump anywhere, has up to N jumps
// - jumps replenish when player "touches" the ground (via raycast)
// - attempted a few approaches to try to make jumps feel better; one approach (setting an explicit curve for the
// player's jump arc, and then taking the approximate differential along it), did sort of work (and allowed a fine
// degree of control), but was suboptimal and didn't trivially work for multiple jumps.
// Current appraoch, ie. to use what are effectively gravity multipliers (and *specifically* different modifiers
// on the way down / way up), seems to work well. Default values here have been set to ones that seem to work
// reasonably well.
public class PlayerJumpController : MonoBehaviour, IPlayerControllerComponent
{
    private PlayerController controller;
    private new Rigidbody rigidbody;
    private Animator animator;
    public void SetupControllerComponent(PlayerController controller) {
        this.controller = controller;
        var player = controller.player;
        rigidbody = player.rigidbody;
        animator = player.animator;
        player.input.Controls.Jump.performed += OnJump;
    }

    [Tooltip("jump height (meters). Inaccurate if gravity factors != 1")]
    public float jumpHeight = 2f;
    
    [Tooltip("maximum jumps (2 = double jump, etc)")]
    public uint maxJumpCount = 2;
    public uint jumpCount = 0;
    
    private float jumpStartTime = 0f;
    private bool  isJumping = false;

    public AnimationCurve jumpCurve;
    public float jumpDuration = 0.64f;
    
    [Tooltip("factor we increase downwards gravity by")]
    public float downGravityFactor = 2.2f;
    
    [Tooltip("factor we increase upwards gravity by")]
    public float upGravityFactor = 0.8f;

    private float CalculateJumpHeight(float height) {
        return Mathf.Sqrt(Mathf.Abs(height * Physics.gravity.y * 2));
    }

    void Update() {
        if (jumpCount > 0 && CheckOnGround()) {
            jumpCount = 0;
            if (isJumping) {
                isJumping = false;
            }
            if (animator.GetBool("isJumping")) {
                animator.SetBool("isJumping", false); 
                animator.SetTrigger("stopJumping");
            }
        }
    }
    public void OnJump(InputAction.CallbackContext context) {
        if (context.performed && jumpCount + 1 < maxJumpCount) {
            ++jumpCount;
            var deltaV = Vector3.up * CalculateJumpHeight(jumpHeight);
            if (rigidbody.velocity.y <= 0) rigidbody.velocity = deltaV;
            else rigidbody.velocity += deltaV;
            animator.SetBool("isJumping", true);
            animator.SetTrigger("startJumping");
            isJumping = true;
            jumpStartTime = Time.time;
            // ApplyJumpStep();
        }
    }

    void ApplyJumpStep() {
        // var t = (Time.time - jumpStartTime) / jumpDuration;
        // var dt = Time.fixedDeltaTime / jumpDuration;
        //
        // var y0 = jumpCurve.Evaluate(t);
        // var y1 = jumpCurve.Evaluate(t + dt);
        // var velocity = rigidbody.velocity;
        // velocity.y = (y1 - y0) * jumpHeight / dt;
        // rigidbody.velocity = velocity;
    }
    void FixedUpdate() {
        if (isJumping) {
            if (Time.time >= jumpStartTime + jumpDuration) {
                isJumping = false;
            } else {
                
                // ApplyJumpStep();
            }
        }
        if (rigidbody.velocity.y != 0f && !CheckOnGround()) {
            var velocity = rigidbody.velocity;
            if (velocity.y <= 0f) velocity += Physics.gravity * (downGravityFactor - 1f) * Time.fixedDeltaTime;
            else velocity -= Physics.gravity * (upGravityFactor - 1f) * Time.fixedDeltaTime;
            rigidbody.velocity = velocity;
        }
    }

    public bool CheckOnGround() => Physics.Raycast(rigidbody.position, Vector3.down, .5f);
    
}
