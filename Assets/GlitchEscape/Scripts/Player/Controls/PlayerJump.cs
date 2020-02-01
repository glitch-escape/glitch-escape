using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerManager playerControls;
    private const float GRAVITY = 9.81f;

    void Awake()
    {
        playerControls = GetComponent<PlayerManager>();
    }

    public void OnJump()
    {
        if (CheckOnGround())
            playerControls.playerRigidbody.velocity = Mathf.Sqrt(playerControls.jumpHeight * GRAVITY * 2) * Vector3.up;
    }

    public void ApplyFallSpeed(float fallSpeed)
    {
        if (playerControls.playerRigidbody.velocity.y < 0)
            playerControls.playerRigidbody.velocity += fallSpeed * Physics.gravity.y * Vector3.up * Time.deltaTime;
    }

    public bool CheckOnGround() => Physics.Raycast(playerControls.playerRigidbody.position, Vector3.down, .5f);
}
