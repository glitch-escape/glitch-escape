using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpController : MonoBehaviour, IPlayerControllerComponent
{
    private PlayerController controller;
    private new Rigidbody rigidbody;
    public void SetupControllerComponent(PlayerController controller) {
        this.controller = controller;
        var player = controller.player;
        rigidbody = player.rigidbody;
        player.input.Controls.Jump.performed += OnJump;
    }

    [Tooltip("jump height (meters)")]
    public float jumpHeight = 10f;
    public float fallAccelerationRate = 1.5f;
    public uint maxJumpCount = 2;
    public uint jumpCount = 0;

    private float jumpStartTime = 0f;
    private bool  isJumping = false;

    public AnimationCurve jumpCurve;
    public float jumpDuration;
    public float downGravityFactor = 1.5f;
    public float upGravityFactor = 1.5f;

    private float CalculateJumpHeight(float height) {
        return Mathf.Sqrt(Mathf.Abs(height * Physics.gravity.y * 2));
    }

    void Update() {
        if (jumpCount > 0 && CheckOnGround()) {
            jumpCount = 0;
        }
    }
    public void OnJump(InputAction.CallbackContext context) {
        if (context.performed && jumpCount + 1 < maxJumpCount) {
            ++jumpCount;
            var deltaV = Vector3.up * CalculateJumpHeight(jumpHeight);
            if (rigidbody.velocity.y <= 0) rigidbody.velocity = deltaV;
            else rigidbody.velocity += deltaV;
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
