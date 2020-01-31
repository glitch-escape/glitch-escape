using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public void Move()
    {
        var movementInput = input.Controls.Move.ReadValue<Vector2>();
        horizontal = movementInput.x;
        vertical = movementInput.y;

        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        isSprinting = hasHorizontalInput || hasVerticalInput;

        playerAnimator.SetBool("isSprinting", isSprinting);

        var forward = playerCamera.transform.forward;
        var right = playerCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        cameraDir = forward * vertical + right * horizontal;

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, cameraDir, turnSpeed * Time.deltaTime, 0f);
        playerRotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        playerRigidbody.MovePosition(playerRigidbody.position + cameraDir * playerAnimator.deltaPosition.magnitude);
        playerRigidbody.MoveRotation(playerRotation);
    }
}
