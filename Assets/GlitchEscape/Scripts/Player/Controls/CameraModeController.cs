using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

//this could be cleaned up for sure, but messing with Cinemachine is weird in general
public class CameraModeController : MonoBehaviour
{
    [InjectComponent] public Player player;
    [InjectComponent] public PlayerMovement playerMovement;
    [InjectComponent] public Transform cameraTarget;
    [InjectComponent] public PlayerJumpAbility playerJumpAbility;
    public CinemachineFreeLook freelookCamera;
    private CameraMode currentCamMode;
    private bool fixedFocus = false;
    private bool cameraRecenter = false;
    private bool dampenReset = false;
    //timer for re-enabling dampening, used for player on kill to re-enable dampening
    private float dampenTimer = 0f;
    //since the recenter camera has a tiny bit of smoothing, this is a timer for recentering
    private float cameraRecenterTimer = 0f;

    public event PlayerEvent.Event OnEvent;

    //for variable camer states and to call the specified function
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
        cameraTarget.transform.Rotate(0.0f, 0.0f, 0.0f, Space.World);
        cameraTarget.transform.position = player.transform.position;
    }

    //transforming the camera target should be in fixed update
    //this lets the position be updated more often so there's little jitter
    //all other updates are in update for better performance
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
        // Logic for camera when player is or isnt moving or jumping
        if (currentCamMode == CameraMode.HybridFollow && !cameraRecenter)
        {
            if (playerMovement.isMoving && !playerJumpAbility.isJumping)
            {
                freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
            }
            else
            {
                freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
            }
        }

        //single hard reset of camera, calls ResetCamera() that starts a timer
        //for how long to take to reset the camera position
        if (((Keyboard.current?.qKey.wasPressedThisFrame ?? false) || (Gamepad.current?.rightStickButton.wasPressedThisFrame ?? false)) 
            && currentCamMode != CameraMode.HardFollow)
        {
            ResetCamera();
        }

        if ((Keyboard.current?.rKey.wasPressedThisFrame ?? false))
        {
            freelookCamera.m_YAxis.Value = 0.5f;
            freelookCamera.m_XAxis.Value = ConvertCameraAnlge(cameraTarget.localEulerAngles.y);
        }

        //checks if the camera dampening is enabled (for instant camera transform to player position)
        if (dampenReset)
        {
            dampenTimer -= Time.deltaTime;
            //once timer hits 0, reset dampening values
            if (dampenTimer < 0f)
            {
                freelookCamera.GetRig(0).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = 3;
                freelookCamera.GetRig(1).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = 3;
                freelookCamera.GetRig(2).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = 3;
                ResetCamera();
                dampenReset = false;
            }
        }

        //the timer for camera to stop recentering and return to hybrid (or last) mode
        if (cameraRecenter)
        {
            cameraRecenterTimer += Time.deltaTime;
            freelookCamera.m_YAxis.Value = Mathf.Lerp(freelookCamera.m_YAxis.Value, 0.5f, 0.3f);
            if (cameraRecenterTimer > 0.3f)
            {
                freelookCamera.m_XAxis.Value = ConvertCameraAnlge(cameraTarget.localEulerAngles.y);
                cameraRecenter = false;
                cameraRecenterTimer = 0f;
                SetCameraMode(currentCamMode);
            }
        }
    }
    
    //function to call to set the various camera modes
    public void SetCameraMode(CameraMode newMode)
    {
        //check if the camera is in a fixed focus state, enabled by CameraModeTrigger objects
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

    //camera mode for older camera movement, ie no following
    public void NoHeadingMode()
    {
        freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
        currentCamMode = CameraMode.NoHeading;
    }
    //camera mode for tracking player
    void HybridFollowMode()
    {
        freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
        freelookCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 1;
        currentCamMode = CameraMode.HybridFollow;
    }
    //camera mode for a camera that always points to the player's heading
    //not used really, except for potential re-orientation of camera
    void HardFollowMode()
    {
        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
        freelookCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0;
        currentCamMode = CameraMode.HardFollow;
    }
    //function used to reset camera angle with a slight delay
    void ResetCamera()
    {
        // Debug.Log("reset was called");
        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
        freelookCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0.1f;
        cameraRecenter = true;
    }

    //used by the CameraModeTrigger object to call specific headings (not implemented in game)
    public void FixedFocus(float newAngle)
    {
        fixedFocus = true;
        cameraTarget.transform.rotation = Quaternion.Euler(0f, newAngle, 0f);
        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
    }

    //reset the focus back to normal
    public void ResetFocus()
    {
        fixedFocus = false;
        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
    }

    //for detecting when player respawns
    private void OnEnable()
    {
        player.OnKilled += OnKilledReset;
        SceneManager.sceneLoaded += OnLevelLoaded;
    }
    private void OnDisable()
    {
        player.OnKilled -= OnKilledReset;
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    //called on player respawn
    //sets the damping to 0, instantly moving the camera back towards the player
    //starts a delay for the camera to go back to normal dampening (delay is checked for in Update())
    private void OnKilledReset()
    {
        freelookCamera.GetRig(0).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = 0;
        freelookCamera.GetRig(1).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = 0;
        freelookCamera.GetRig(2).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = 0;

        freelookCamera.m_YAxis.Value = 0.5f;
        freelookCamera.m_XAxis.Value = ConvertCameraAnlge(cameraTarget.localEulerAngles.y);

        freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
        freelookCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0f;

        dampenReset = true;
        dampenTimer = 0.1f;
    }

    private float ConvertCameraAnlge(float cameraAngle)
    {
        if (cameraAngle > 180)
        {
            return (cameraAngle - 360f);
        }
        else
        {
            return cameraAngle;
        }
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        OnKilledReset();
    }
}
