using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [System.NonSerialized] public Vector3 savePoint;

    private PlayerManager playerControls;
    private static bool onSwitch = false;
    private float lastMazeSwitchTime = -10f;

    void Awake()
    {
        playerControls = GetComponent<PlayerManager>();
    }

    void Start()
    {
        savePoint = transform.position;
    }

    public void OnInteract()
    {
        if (onSwitch == true && Time.time >= lastMazeSwitchTime + playerControls.mazeSwitchCooldown)
        {
            lastMazeSwitchTime = Time.time;
            playerControls.maze.SetActive(!playerControls.maze.activeInHierarchy);
            playerControls.glitchMaze.SetActive(!playerControls.glitchMaze.activeInHierarchy);
            savePoint = transform.position;
            playerControls.systemManager.hudManager.hudTimer.switchTimer();
        }
    }

    public void Respawn()
    {
        transform.position = savePoint;
        playerControls.systemManager.hudManager.hudTimer.TimerReset();
        playerControls.maze.SetActive(true);
        playerControls.glitchMaze.SetActive(false);
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
