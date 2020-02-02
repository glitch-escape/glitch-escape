using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour, IPlayerControllerComponent {

    private PlayerController controller;
    private CinemachineFreeLook freeLookCam;
    private Camera camera;
    private Input input;
    public void SetupControllerComponent(PlayerController controller) {
        Debug.Log("Setting up PlayerCameraController sub-component");
        this.controller = controller;
        camera = controller.camera;
        input = controller.player.input;
        freeLookCam = controller.GetComponentInChildren<CinemachineFreeLook>();
        if (!freeLookCam) {
            Debug.LogError("PlayerCameraController.cs: missing CinemachineFreeLook component under a PlayerController object!");
        }
    }
    
    [Tooltip("curve applied to change the responsiveness of gamepad x / y input values")]
    public AnimationCurve cameraInputCurve;
    [Range(5f, 360)] public float cameraTurnSpeed = 180f;
    
    public void Update()
    {
        var lookInput = input.Controls.Look.ReadValue<Vector2>();

        // apply designer-defined input curve to make camera feel more responsive
        lookInput.x = ApplyInputCurve(lookInput.x, cameraInputCurve);
        lookInput.y = ApplyInputCurve(lookInput.y, cameraInputCurve);

        freeLookCam.m_XAxis.Value = freeLookCam.m_XAxis.Value + lookInput.x * cameraTurnSpeed * Time.deltaTime;
        freeLookCam.m_YAxis.Value = freeLookCam.m_YAxis.Value - lookInput.y * Time.deltaTime;
    }

    private float ApplyInputCurve(float input, AnimationCurve curve)
    {
        float sign = input >= 0 ? 1f : -1f;
        return sign * curve.Evaluate((Mathf.Abs(input)));
    }
}
