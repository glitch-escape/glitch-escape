using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraModeController : MonoBehaviour
{
    //[InjectComponent] Player player;
    [InjectComponent] public PlayerMovement playerMovement;
    private Vector2 lastMousePosition;
    public CinemachineFreeLook freelookCamera;
    private CameraMode currentCamMode;
    private float cameraDelay = 1f;
    private float elapsedTime = 0f;

    private enum CameraMode
    {
        NoHeading,
        SlowFollow,
        FastFollow,
        HybridFollow,
        HardFollow,
    }

    void Start()
    {
        NoHeadingMode();
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("Switching to no heading");
            SetCameraMode(CameraMode.NoHeading);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("Switching to slow follow");
            SetCameraMode(CameraMode.SlowFollow);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("Switching to fast follow");
            SetCameraMode(CameraMode.FastFollow);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.Log("Switching to Hybrid follow");
            SetCameraMode(CameraMode.HybridFollow);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            Debug.Log("Switching to hard follow");
            SetCameraMode(CameraMode.HardFollow);
        }

        // Logic for camera when player is or isnt moving
        if (currentCamMode == CameraMode.HybridFollow)
        {
            if (playerMovement.isMoving)
            {
                freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
            }
            else
            {
                freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
            }
        }
    }
    
    void SetCameraMode(CameraMode newMode)
    {
        if (newMode == currentCamMode)
        {
            Debug.Log("Already in mode");
            return;
        }
        else
        {
            switch (newMode)
            {
                case CameraMode.NoHeading:
                    NoHeadingMode();
                    break;
                case CameraMode.SlowFollow:
                    SlowFollowMode();
                    break;
                case CameraMode.FastFollow:
                    FastFollowMode();
                    break;
                case CameraMode.HybridFollow:
                    HybridFollowMode();
                    break;
                case CameraMode.HardFollow:
                    HardFollowMode();
                    break;
            }
        }
    }

    void NoHeadingMode()
    {
        //Debug.Log("No heading mode");
        freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
        currentCamMode = CameraMode.NoHeading;
    }
    void SlowFollowMode()
    {
        //Debug.Log("Slow Follow Mode");
        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
        freelookCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 2;
        currentCamMode = CameraMode.SlowFollow;
    }
    void FastFollowMode()
    {
        //Debug.Log("Fast Follow Mode");
        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
        freelookCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 1;
        currentCamMode = CameraMode.FastFollow;
    }
    void HybridFollowMode()
    {
        //Debug.Log("Hybrid Follow Mode");
        freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
        freelookCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 1;
        currentCamMode = CameraMode.HybridFollow;
    }
    void HardFollowMode()
    {
        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
        freelookCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0;
        currentCamMode = CameraMode.HardFollow;
    }
}
