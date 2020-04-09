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
        ShowAnimatorBoolean("isRunning (legacy)", ref isRunning);
        animator.SetBool("isRunning", isRunning);
        
        var wasJumping = isJumping;
        ShowAnimatorBoolean("isJumping", ref isJumping);
        animator.SetBool("isJumping", isJumping);

        var wasDashing = isDashing;
        ShowAnimatorBoolean("isDashing", ref isDashing);
        animator.SetBool("isDashing", isDashing);

        ShowAnimatorFloat("runSpeed", ref runSpeed, 1f, minRunSpeed, maxRunSpeed);
        ShowAnimatorTrigger("do some transition");
    }
}
