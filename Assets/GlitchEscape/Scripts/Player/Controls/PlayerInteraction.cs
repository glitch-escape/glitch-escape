using System.Collections;
using System.Collections.Generic;
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

    public void OnInteract()
    {
        if (onSwitch == true && Time.time >= lastMazeSwitchTime + playerControls.mazeSwitchCooldown)
        {
            lastMazeSwitchTime = Time.time;
            playerControls.maze.SetActive(!playerControls.maze.activeInHierarchy);
            playerControls.glitchMaze.SetActive(!playerControls.glitchMaze.activeInHierarchy);
            savePoint = transform.position;
            playerControls.hud.switchTimer();
        }
    }

    public void Respawn()
    {
        transform.position = savePoint;
        playerControls.hud.TimerReset();
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
