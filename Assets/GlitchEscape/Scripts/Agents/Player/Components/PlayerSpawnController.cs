using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnController : PlayerComponent {
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    
    private void OnEnable() {
        player.OnKilled += Respawn;
    }
    private void OnDisable() {
        player.OnKilled -= Respawn;
    }
    public void SetSpawnPosition(Vector3 position, Quaternion rotation) {
        respawnPosition = position;
        respawnRotation = rotation;
    }
    public void SetSpawnPosition(MazeSwitch mazeSwitch) {
        SetSpawnPosition(
            mazeSwitch.transform.position + Vector3.up * player.config.spawnHeight, 
            mazeSwitch.transform.rotation);   
    }
    public void SetSpawnPosition(SavePoint savePoint) {
        SetSpawnPosition(
            savePoint.transform.position + Vector3.up * player.config.spawnHeight, 
            savePoint.transform.rotation);   
    }
    private void Respawn() {
        player.transform.position = respawnPosition;
        player.transform.rotation = respawnRotation;
    }
    private void Awake() {
        SetSpawnPosition(player.transform.position, player.transform.rotation);
    }
}
