using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpController : MonoBehaviour
{
    private PlayerController controller;
    private Player player;
    private Input input;
    private Animator animator;
    private new Rigidbody rigidbody;

    public void SetupControllerComponent(PlayerController controller)
    {
        this.controller = controller;
        player = controller.player;
        rigidbody = player.rigidbody;
        animator = player.animator;
        input = player.input;
        //animator.SetBool("isDashing", false);
    }

    #region Wall Jump Implementation

    private void Update()
    {
        
    }

    #endregion
}
