using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerManager playerControls;
    private Vector3 cameraDir;
    private Quaternion playerRotation = Quaternion.identity;
    private float horizontal;
    private float vertical;
    private bool hasHorizontalInput;
    private bool hasVerticalInput;
    private bool isSprinting;
    private Vector2 movementInput;
    private Vector3 forward;
    private Vector3 right;

    void Awake()
    {
        playerControls = GetComponent<PlayerManager>();
    }

    public void Move()
    {
        movementInput = playerControls.input.Controls.Move.ReadValue<Vector2>();
        horizontal = movementInput.x;
        vertical = movementInput.y;

        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        isSprinting = hasHorizontalInput || hasVerticalInput;

        playerControls.playerAnimator.SetBool("isSprinting", isSprinting);

        forward = playerControls.mainCamera.transform.forward;
        right = playerControls.mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        cameraDir = forward * vertical + right * horizontal;

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, cameraDir, playerControls.turnSpeed * Time.deltaTime, 0f);
        playerRotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        playerControls.playerRigidbody.MovePosition(playerControls.playerRigidbody.position + cameraDir * playerControls.playerAnimator.deltaPosition.magnitude);
        playerControls.playerRigidbody.MoveRotation(playerRotation);
    }
}
