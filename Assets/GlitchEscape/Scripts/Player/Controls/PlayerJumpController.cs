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
        }
    }

    private void FixedUpdate() {
        // if (rigidbody.velocity.y < 0) {
        //     
        //     
        //     
        // }
    }

    public bool CheckOnGround() => Physics.Raycast(rigidbody.position, Vector3.down, .5f);
    
}
