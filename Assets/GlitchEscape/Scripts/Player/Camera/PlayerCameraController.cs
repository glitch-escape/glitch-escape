﻿using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviourBorrowingConfigFrom<Player, PlayerConfig> {

    public CinemachineFreeLook freeLookCam;

    void OnEnable() {
        if (freeLookCam == null) {
            freeLookCam = (transform.parent ?? transform).GetComponentInChildren<CinemachineFreeLook>();
            if (freeLookCam == null) {
                Debug.LogError("PlayerCameraController could not get CinemachineFreeLook component!");
            }
        }
    }

    // camera turn speed, defined on PlayerConfig
    private float cameraTurnSpeed => config.cameraTurnSpeed;

    public void Update() {
        var lookInput = PlayerControls.instance.lookInput;
        freeLookCam.m_XAxis.Value = freeLookCam.m_XAxis.Value + lookInput.x * cameraTurnSpeed * Time.deltaTime;
        freeLookCam.m_YAxis.Value = freeLookCam.m_YAxis.Value - lookInput.y * Time.deltaTime;
    }
}
