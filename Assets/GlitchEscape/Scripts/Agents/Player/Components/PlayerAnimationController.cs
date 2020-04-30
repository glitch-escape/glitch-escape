using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : PlayerComponent, IResettable {
    [InjectComponent] public PlayerMovement movement;
    [InjectComponent] public PlayerJumpAbility jump;
    [InjectComponent] public PlayerDashAbility dash;
    [InjectComponent] public Animator animator;

    public float currentAnimationSpeed => movement.moveSpeed;
    
    void OnEnable() {
        foreach (var component in GetComponentsInChildren<IPlayerEventSource>()) {
            component.OnEvent += OnPlayerEvent;
        }
        animator.applyRootMotion = false;
        animator.SetFloat("runSpeed", 0.0f);
    }
    void OnDisable() {
        foreach (var component in GetComponentsInChildren<IPlayerEventSource>()) {
            component.OnEvent -= OnPlayerEvent;
        }
    }

    public void Reset() {
        animator.SetFloat("runSpeed", 0f);
        animator.SetBool("isJumping", false);
        animator.SetBool("isDashing", false);
    }
    private void Update() {
        animator.SetFloat("runSpeed", player.input.moveInput.magnitude);
        animator.SetBool("isJumping", player.jump.isJumping || player.movement.isFalling);
        animator.SetBool("isNearWall", player.jump.isPlayerNearWall);
        // animator.SetFloat("runSpeed", player.movement.moveSpeed * player.input.moveInput.magnitude);
    }

    private void UpdateJump(bool wallJumping = false) {

        var isJumping = player.jump.isJumping;
        animator.SetBool("isNearWall", player.jump.isPlayerNearWall);
        var isDoubleJump = player.jump.jumpCount > 1;
        
        if (isJumping && isDoubleJump) {
            animator.SetBool("isJumping", true);
            animator.SetBool("isDoubleJump", true);
            animator.SetTrigger("doubleJump");
        } else if (wallJumping) {
            animator.SetBool("isDoubleJump", isDoubleJump);
            animator.SetTrigger("wallJump");
            animator.SetBool("isJumping", true);
        } else {
            animator.SetBool("isDoubleJump", isDoubleJump);
            animator.SetBool("isJumping", isJumping);
        }
    }
    
    private void OnPlayerEvent(PlayerEvent.Type eventType) {
        switch (eventType) {
            // movement
            case PlayerEvent.Type.BeginMovement:
                //animator.SetTrigger("startRunning");
                break;
            case PlayerEvent.Type.EndMovement:
                //animator.SetBool("isRunning", false);
                break;
            // jump
            case PlayerEvent.Type.FloorJump:
            case PlayerEvent.Type.AirJump:
                UpdateJump();
                break;
            case PlayerEvent.Type.WallJump:
                UpdateJump(true);
                break;
            case PlayerEvent.Type.EndJump:
                UpdateJump();
                break;
            // interact
            case PlayerEvent.Type.Interact: break;
            
            // dash
            case PlayerEvent.Type.BeginDash: 
                animator.SetBool("isDashing", true);
                break;
            case PlayerEvent.Type.EndDash: 
                animator.SetBool("isDashing", false);
                break;
            
            // shoot
            case PlayerEvent.Type.Shoot: break;
            
            // TODO: add audio clips for other player events
            
            default: break;
                // throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
        }
    }
}
