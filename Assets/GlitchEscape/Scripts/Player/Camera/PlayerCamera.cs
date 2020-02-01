using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    private PlayerManager playerControls;
    private Vector2 lookInput;

    void Awake()
    {
        playerControls = GetComponent<PlayerManager>();
    }
    public void Camera()
    {
        lookInput = playerControls.input.Controls.Look.ReadValue<Vector2>();
        
        // apply designer-defined input curve to make camera feel more responsive
        lookInput.x = ApplyInputCurve(lookInput.x, playerControls.cameraInputCurve);
        lookInput.y = ApplyInputCurve(lookInput.y, playerControls.cameraInputCurve);

        playerControls.freeLookCam.m_XAxis.Value = playerControls.freeLookCam.m_XAxis.Value + lookInput.x * playerControls.cameraTurnSpeed * Time.deltaTime;
        playerControls.freeLookCam.m_YAxis.Value = playerControls.freeLookCam.m_YAxis.Value - lookInput.y * Time.deltaTime;
    }

    private float ApplyInputCurve(float input, AnimationCurve curve)
    {
        float sign = input >= 0 ? 1f : -1f;
        return sign * curve.Evaluate((Mathf.Abs(input)));
    }
}
