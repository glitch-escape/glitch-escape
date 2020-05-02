using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerSpawnController : PlayerComponent {
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private Vector3 cameraRespawnPosition;
    private Quaternion cameraRespawnRotation;
    private Camera sceneCam;

    private void OnEnable() {
        player.OnKilled += Respawn;
    }
    private void OnDisable() {
        player.OnKilled -= Respawn;
    }
    public void SetSpawnPosition(Vector3 position, Quaternion rotation) {
        Debug.Log("Set spawn position: "+position);
        respawnPosition = position;
        respawnRotation = rotation;
        cameraRespawnPosition = sceneCam.transform.position;
        cameraRespawnRotation = sceneCam.transform.rotation;
    }
    public void SetSpawnPosition(MazeSwitch mazeSwitch) {
        // TODO: FIX MAZE ORIENTATION!!!
        // PREFAB ROOTS SHOULD NOT HAVE NON-ZERO DEFAULT ROTATIONS!!!!
        // (fix this by moving maze trigger graphics one level lower (w/ rotation) OR clearing default rotation and
        // editing the .blend file so the mandala is facing flat on the floor, NOT vertical. problem: this may
        // require editing EVERY maze switch instance to have correct rotation (and if this wasn't necessary I'd just do
        // this right now)
        // Shitty fix: completely ignore maze switch rotation b/c as is this will result in the player getting respawned
        // sideways, aaghhhhh....
        // (note: we prev did not use rotation, but this is an easy way to determine what direction player should be
        // facing on respawn)
        SetSpawnPosition(
            mazeSwitch.transform.position + Vector3.up * player.config.spawnHeight, 
            Quaternion.identity);
    }
    public void SetSpawnPosition(SavePoint savePoint) {
        SetSpawnPosition(
            savePoint.transform.position + Vector3.up * player.config.spawnHeight, 
            savePoint.transform.rotation);   
    }
    private void Respawn() {
        Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false);
        player.transform.position = respawnPosition;
        player.transform.rotation = respawnRotation;
        Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.SetActive(true);
    }
    private void Awake() {
        sceneCam = Camera.main;
        SetSpawnPosition(player.transform.position, player.transform.rotation);
    }
}
