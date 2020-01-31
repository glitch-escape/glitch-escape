using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private const float GRAVITY = 9.81f;

    public void OnJump(Vector3 position, Vector3 velocity, float jumpHeight)
    {
        if (CheckOnGround(position))
            velocity = Mathf.Sqrt(jumpHeight * GRAVITY * 2) * Vector3.up;
    }

    public void ApplyFallSpeed(Vector3 velocity, float fallSpeed)
    {
        if (velocity.y < 0)
            velocity += fallSpeed * Physics.gravity.y * Vector3.up * Time.deltaTime;
    }

    public bool CheckOnGround(Vector3 position)
    {
        return Physics.Raycast(position, Vector3.down, .5f);
    }
}
