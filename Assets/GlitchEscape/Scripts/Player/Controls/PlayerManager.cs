using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Objects")]
    public GameObject maze;
    public GameObject glitchMaze;
    public Transform playerCamera;
    public CinemachineFreeLook freeLookCam;

    [Header("Variables")]
    public AnimationCurve cameraInputCurve;

    [Range(60f, 360f)]
    public float cameraTurnSpeed = 180;
    [Range(5f, 50f)]
    public float turnSpeed = 10f;


    [Range(1f, 10f)]
    public float jumpHeight = 2f;


    // [Range(1f, 15f)]
    // public float jumpVelocity = 5f;
    [Range(0.1f, 10f)]
    public float fallSpeed = 1.5f;

    private const float GRAVITY = 9.81f; // m/s^2
    private Animator playerAnimator;
    private Rigidbody playerRigidbody;
    private Vector3 cameraDir;
    private Quaternion playerRotation = Quaternion.identity;
    private Input input;
    private Vector2 movementInput;
    private static bool onSwitch = false;
    private Vector3 savePoint;
    private float horizontal;
    private float vertical;
    private bool hasHorizontalInput;
    private bool hasVerticalInput;
    private bool isSprinting;


    [Header("Scripts")]
    public HUD hud;
    public PlayerMovement playerMovement;
    public PlayerJump playerJump;

    void Awake()
    {
        input = new Input();
        playerAnimator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        savePoint = new Vector3(-2f, 7.5f, 2f);
    }

    private void OnEnable() => input.Controls.Enable();

    private void OnDisable() => input.Controls.Disable();

    void FixedUpdate() => playerMovement.Move();

    private float ApplyInputCurve(float input, AnimationCurve curve)
    {
        float sign = input >= 0 ? 1f : -1f;
        return sign * curve.Evaluate((Mathf.Abs(input)));
    }
    [System.Obsolete]
    void Update()
    {
        var lookInput = input.Controls.Look.ReadValue<Vector2>();

        // apply designer-defined input curve to make camera feel more responsive
        lookInput.x = ApplyInputCurve(lookInput.x, cameraInputCurve);
        lookInput.y = ApplyInputCurve(lookInput.y, cameraInputCurve);

        freeLookCam.m_XAxis.Value = freeLookCam.m_XAxis.Value + lookInput.x * cameraTurnSpeed * Time.deltaTime;
        freeLookCam.m_YAxis.Value = freeLookCam.m_YAxis.Value - lookInput.y * Time.deltaTime;


        playerJump.ApplyFallSpeed(playerRigidbody.velocity, fallSpeed);
    }

    public void Respawn()
    {
        transform.position = savePoint;
        hud.TimerReset();
        maze.SetActive(true);
        glitchMaze.SetActive(false);
    }

    private float lastMazeSwitchTime = -10f;    // seconds
    public float mazeSwitchCooldown = 0.1f;     // seconds

    public void OnInteract()
    {
        if (onSwitch == true && Time.time >= lastMazeSwitchTime + mazeSwitchCooldown)
        {
            lastMazeSwitchTime = Time.time;
            maze.SetActive(!maze.activeInHierarchy);
            glitchMaze.SetActive(!glitchMaze.activeInHierarchy);
            savePoint = transform.position;
            hud.switchTimer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Switch")
        {
            onSwitch = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Switch")
        {
            onSwitch = false;
        }
    }

}
