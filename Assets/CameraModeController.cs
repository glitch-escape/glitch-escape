using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraModeController : MonoBehaviour
{
    [InjectComponent] public Player player;
    [InjectComponent] public PlayerMovement playerMovement;
    [InjectComponent] public Transform cameraTarget;
    public CinemachineFreeLook freelookCamera;
    private CameraMode currentCamMode;
    private bool fixedFocus = false;

    public enum CameraMode
    {
        NoHeading,
        HybridFollow,
        HardFollow,
        FixedFocus,
    }

    void Start()
    {
        SetCameraMode(CameraMode.HybridFollow);
        //cameraTarget = new GameObject().transform;
        //cameraTarget.transform.position = player.transform.position;
        cameraTarget.transform.Rotate(0.0f, 0.0f, 0.0f, Space.World);
        cameraTarget.transform.position = player.transform.position;
    }

    void FixedUpdate()
    {
        cameraTarget.transform.position = player.transform.position;
        if (!fixedFocus)
        {
            cameraTarget.transform.rotation = player.transform.rotation;
        }
    }

    void Update()
    {
        /*
        cameraTarget.transform.position = player.transform.position;
        if (!fixedFocus)
        {
            cameraTarget.transform.rotation = player.transform.rotation;
        }
        else
        {
        
        }*/
        
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

        //single hard reset of camera
        if ((Keyboard.current.qKey.wasPressedThisFrame || Gamepad.current.rightStickButton.wasPressedThisFrame) 
            && currentCamMode != CameraMode.HardFollow)
        {
            ResetCamera();
            
            //SetCameraMode(currentCamMode);
        }
        if (Keyboard.current.qKey.wasReleasedThisFrame)
        {
            SetCameraMode(currentCamMode);
        }
    }
    
    public void SetCameraMode(CameraMode newMode)
    {
        if (!fixedFocus)
        {
            switch (newMode)
            {
                case CameraMode.NoHeading:
                    NoHeadingMode();
                    break;
                case CameraMode.HybridFollow:
                    HybridFollowMode();
                    break;
                case CameraMode.HardFollow:
                    HardFollowMode();
                    break;
                case CameraMode.FixedFocus:
                    //FixedFocusMode();
                    break;
            }
        }
    }

    public void NoHeadingMode()
    {
        //Debug.Log("No heading mode");
        freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
        currentCamMode = CameraMode.NoHeading;
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

    void ResetCamera()
    {
        Debug.Log("reset was called");
        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
        freelookCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0.2f;
    }

    public void FixedFocus(float newAngle)
    {
        fixedFocus = true;

        cameraTarget.transform.rotation = Quaternion.Euler(0f, newAngle, 0f);

        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
    }

    public void ResetFocus()
    {
        fixedFocus = false;
        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
    }
}
