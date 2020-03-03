using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour, IPlayerControllerComponent {

    private PlayerController controller;
    private CinemachineFreeLook freeLookCam;
    private new Camera camera;
    public void SetupControllerComponent(PlayerController controller) {
        this.controller = controller;
        camera = controller.camera;
        freeLookCam = controller.GetComponentInChildren<CinemachineFreeLook>();
        if (!freeLookCam) {
            Debug.LogError("PlayerCameraController.cs: missing CinemachineFreeLook component under a PlayerController object!");
        }
    }
    [Tooltip("Camera turn speed, in degrees / sec")]
    [Range(5f, 360)] public float cameraTurnSpeed = 180f;
    
    public void Update() {
        var lookInput = PlayerControls.instance.lookInput;
        freeLookCam.m_XAxis.Value = freeLookCam.m_XAxis.Value + lookInput.x * cameraTurnSpeed * Time.deltaTime;
        freeLookCam.m_YAxis.Value = freeLookCam.m_YAxis.Value - lookInput.y * Time.deltaTime;
    }
}
