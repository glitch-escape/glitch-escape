using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerAnimationControllerTest : BaseAnimationControllerTest {
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isDashing = false;
    private bool isGrounded = false;
    private bool isNearWall = false;
    private float runSpeed = 1f;
    public float minRunSpeed = 0.5f;
    public float maxRunSpeed = 2f;
    
    /// <summary>
    /// Use this to control test animator
    /// </summary>
    void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Player animation controller test");
        GUILayout.Label("                                ");
        GUILayout.Label("                                ");
        GUILayout.EndHorizontal();
        
        var wasRunning = isRunning;
        ShowAnimatorBoolean("isRunning", ref isRunning);
        if (isRunning && !wasRunning) animator.SetTrigger("startRunning");
        if (wasRunning && !isRunning) animator.SetTrigger("stopRunning");
        
        var wasJumping = isJumping;
        ShowAnimatorBoolean("isJumping", ref isJumping);
        if (isJumping && !wasJumping) animator.SetTrigger("startJumping");
        if (wasJumping && !isJumping) animator.SetTrigger("stopJumping");
        
        var wasDashing = isDashing;
        ShowAnimatorBoolean("isDashing", ref isDashing);
        if (isDashing && !wasDashing) animator.SetTrigger("startDashing");
        if (wasDashing && !isDashing) animator.SetTrigger("stopDashing");
        
        ShowAnimatorFloat("runSpeed", ref runSpeed, 1f, minRunSpeed, maxRunSpeed);
        ShowAnimatorTrigger("do some transition");
    }
}
