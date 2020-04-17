using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : PlayerComponent {
    [InjectComponent] public PlayerMovementController movement;
    [InjectComponent] public PlayerJumpAbility jump;
    [InjectComponent] public PlayerDashAbility dash;
    [InjectComponent] public Animator animator;
    
    public float currentAnimationSpeed => animator.deltaPosition.magnitude;
    
    void OnEnable() {
        foreach (var component in GetComponentsInChildren<IPlayerEventSource>()) {
            component.OnEvent += OnPlayerEvent;
        }
    }
    void OnDisable() {
        foreach (var component in GetComponentsInChildren<IPlayerEventSource>()) {
            component.OnEvent -= OnPlayerEvent;
        }
    }
    
    private void OnPlayerEvent(PlayerEvent.Type eventType) {
        switch (eventType) {
            // movement
            case PlayerEvent.Type.BeginMovement:
                animator.SetBool("isRunning", true);
                animator.SetTrigger("startRunning");
                break;
            case PlayerEvent.Type.EndMovement:
                animator.SetBool("isRunning", false);
                animator.SetTrigger("stopRunning");
                break;
            
            // jump
            case PlayerEvent.Type.FloorJump:
            case PlayerEvent.Type.AirJump:
            case PlayerEvent.Type.WallJump:
                animator.SetBool("isJumping", true);
                animator.SetTrigger("startJumping");
                break;
            case PlayerEvent.Type.EndJump:
                animator.SetBool("isJumping", false);
                animator.SetTrigger("stopJumping");
                break;
            
            // interact
            case PlayerEvent.Type.Interact: break;
            
            // dash
            case PlayerEvent.Type.BeginDash: 
                animator.SetBool("isDashing", true);
                animator.SetTrigger("startDashing");
                break;
            case PlayerEvent.Type.EndDash: 
                animator.SetBool("isDashing", false);
                animator.SetTrigger("stopDashing");
                break;
            
            // shoot
            case PlayerEvent.Type.Shoot: break;
            
            // TODO: add audio clips for other player events
            
            default: break;
                // throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
        }
    }
    void Update() {
        // animator.SetBool("isRunning", movement.isMoving);
    }
}
