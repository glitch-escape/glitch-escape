using UnityEngine;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    // Public Variables:
    [Header("Objects")]
    public GameObject maze;
    public GameObject glitchMaze;
    public GameObject system;
    public Transform mainCamera;
    public CinemachineFreeLook freeLookCam;

    [Header("Variables")]
    [Tooltip("degree")] [Range(60f, 360f)] public float cameraTurnSpeed = 180;
    [Range(1f, 10f)] public float jumpHeight = 2f;
    [Range(0.1f, 10f)] public float fallSpeed = 1.5f;
    [Tooltip("second")] [Range(0.1f, 1f)] public float mazeSwitchCooldown = 0.1f;
    [System.NonSerialized] public Input input;
    [System.NonSerialized] public Animator playerAnimator;
    [System.NonSerialized] public Rigidbody playerRigidbody;

    // Functional Scripts:
    [System.NonSerialized] public SystemManager systemManager;
    [System.NonSerialized] public PlayerMovementController playerMovement;
    [System.NonSerialized] public PlayerJumpController playerJump;
    [System.NonSerialized] public PlayerInteractionController playerInteraction;
    [System.NonSerialized] public PlayerCameraController playerCamera;

    // MonoBehaviour:
    void Awake()
    {
        // Get Scripts
        systemManager = system.GetComponent<SystemManager>();
        playerMovement = GetComponent<PlayerMovementController>();
        playerJump = GetComponent<PlayerJumpController>();
        playerInteraction = GetComponent<PlayerInteractionController>();
        playerCamera = GetComponent<PlayerCameraController>();

        // Get Objects
        playerAnimator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();

        input = new Input();
    }

    // void FixedUpdate() => playerMovement.Move();

    void Update()
    {
        // playerJump.ApplyFallSpeed(fallSpeed);
    }

    // Private:
    private void OnEnable() => input.Controls.Enable();
    private void OnDisable() => input.Controls.Disable();

    // No extra Function in PlayerManager, build into a seprate script
}
