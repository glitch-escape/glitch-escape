using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using GlitchEscape.Scripts.DebugUI;

public class PlayerSpawnController : PlayerComponent, IPlayerDebug {
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private Vector3 cameraRespawnPosition;
    private Quaternion cameraRespawnRotation;
    private Camera sceneCam;
    public GameObject spawnPointDebugMarker;
    public GameObject cameraPointDebugMarker;
    public bool enableSpawnPointDebugMarker = false;

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
        if (enableSpawnPointDebugMarker && spawnPointDebugMarker != null) {
            spawnPointDebugMarker.transform.position = respawnPosition;
            spawnPointDebugMarker.transform.rotation = respawnRotation;
            spawnPointDebugMarker.gameObject.SetActive(true);
        } else spawnPointDebugMarker?.SetActive(false);
        if (enableSpawnPointDebugMarker && cameraPointDebugMarker != null) {
            cameraPointDebugMarker.transform.position = cameraRespawnPosition;
            cameraPointDebugMarker.transform.rotation = cameraRespawnRotation;
            cameraPointDebugMarker.gameObject.SetActive(true);
        } else cameraPointDebugMarker?.SetActive(false);
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

    private void SetSpawnPositionAtGroundBelowPoint(Transform transform, Vector3 offset) {
        RaycastHit hit;
        var spawnPoint =
            (Physics.Raycast(transform.position + offset, Vector3.down, out hit, 25f, LayerMasks.FloorAndWalls) 
                ? hit.point 
                : transform.position)
            + Vector3.up * player.config.spawnHeight;
        SetSpawnPosition(spawnPoint, player.transform.rotation);
    }

    public void SetSpawnPosition(SavePoint savePoint) {
        if (savePoint.savePointTarget != null) {
            SetSpawnPosition(
                savePoint.savePointTarget.position + Vector3.up * player.config.spawnHeight,
                savePoint.savePointTarget.rotation);
        } else if (savePoint.isPlatformCollider) {
            SetSpawnPositionAtGroundBelowPoint(savePoint.transform, Vector3.up * savePoint.platformRaycastHeightOffset);
        } else {
            SetSpawnPositionAtGroundBelowPoint(savePoint.transform, Vector3.zero);
        }
    }
    public void Respawn() {
        FireEvent(PlayerEvent.Type.PlayerRespawn);
        Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false);
        player.transform.position = respawnPosition;
        player.transform.rotation = respawnRotation;
        Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.SetActive(true);
    }
    private void Awake() {
        sceneCam = Camera.main;
        SetSpawnPosition(player.transform.position, player.transform.rotation);
        if (PlayerPrefs.HasKey("ShowSpawnDebugMarkers")) {
            enableSpawnPointDebugMarker = PlayerPrefs.GetInt("ShowSpawnDebugMarkers",
                                              enableSpawnPointDebugMarker ? 1 : 0) != 0;
        }
        if (!enableSpawnPointDebugMarker) {
            spawnPointDebugMarker.SetActive(false);
            cameraPointDebugMarker.SetActive(false);
        }
    }
    public String debugName => GetType().Name;
    public void DrawDebugUI() {
        bool enableMarkers = GUILayout.Toggle(enableSpawnPointDebugMarker, "show spawn debug markers");
        if (enableMarkers != enableSpawnPointDebugMarker) {
            enableSpawnPointDebugMarker = enableMarkers;
            PlayerPrefs.SetInt("ShowSpawnDebugMarkers", enableSpawnPointDebugMarker ? 1 : 0);
            PlayerPrefs.Save();
            spawnPointDebugMarker?.gameObject.SetActive(enableMarkers);
            cameraPointDebugMarker?.gameObject.SetActive(enableMarkers);
        }
    }
}
