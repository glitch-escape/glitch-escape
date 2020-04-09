using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SphericalDetectionComponent : EnemyVisionController {
    private EnemyController controller => enemy.controller;
    private new SphereCollider collider;

    public float detectionRadius = 10f;
    private float currentDetectionRadiusScaleFactor = 1.0f;
    private float currentDetectionRadius =>
        detectionRadius * currentDetectionRadiusScaleFactor;

    // initialization method
   void Start() {
        // Get references
        collider = GetComponent<SphereCollider>()
                   ?? gameObject.AddComponent<SphereCollider>();
        collider.radius = currentDetectionRadius;
        collider.isTrigger = true;

        // make sure that detection radius, if previously enabled,
        // is enabled and up to date
        DebugShowDetectionRadius(detectionRadiusViewEnabled);
    }

    public override void SetVisionDistanceFactor(float factor) {
        currentDetectionRadiusScaleFactor = factor;
        collider = GetComponent<SphereCollider>()
                   ?? gameObject.AddComponent<SphereCollider>();
        collider.radius = currentDetectionRadius;
    }

    #region DetectionRadiusVisualization
    private bool detectionRadiusViewEnabled = false;
    private static Color COLOR_NO_DETECTION => Color.grey;
    private static Color COLOR_DETECTED_PLAYER => Color.black;

    private new MeshRenderer renderer;
    private Material material;

    public override void DebugShowDetectionRadius(bool enabled) {
        detectionRadiusViewEnabled = enabled;
        if (enabled) {
            // get or add a mesh renderer if we don't have one enabled
            if (renderer == null) {
                renderer = GetComponent<MeshRenderer>()
                           ?? gameObject.AddComponent<MeshRenderer>();
                renderer.enabled = true;
                material = renderer.material;
                UpdateColor();
            }
        }
        else if (renderer != null) {
            renderer.enabled = false;
            material = null;
        }
    }
    private void UpdateColor() {
        if (material != null) {
            material.SetColor("BaseColor",
                canSeePlayer ?
                    COLOR_DETECTED_PLAYER :
                    COLOR_NO_DETECTION);
        }
    }
    #endregion

    #region VisionDetectionImplementation
    public bool canSeePlayer => activePlayerInTriggerBounds != null;
    private Player activePlayerInTriggerBounds = null;
    private bool hasLastKnownPlayerPosition = false;
    private Vector3 lastKnownPlayerPosition = Vector3.zero;
    private List<IEnemyObjectiveMarker> objectiveMarkers = new List<IEnemyObjectiveMarker>();

    public override List<IEnemyObjectiveMarker> GetKnownObjectiveMarkers() {
        return objectiveMarkers;
    }
    public override void ClearKnownObjectiveMarkers() {
        objectiveMarkers.Clear();
    }
    public override bool CanSeePlayer() {
        return activePlayerInTriggerBounds != null;
    }
    public override bool HasLastKnownPlayerPosition(out Vector3 lastPosition) {
        if (activePlayerInTriggerBounds != null) {
            lastPosition = activePlayerInTriggerBounds.transform.position;
            return true;
        }
        if (hasLastKnownPlayerPosition) {
            lastPosition = lastKnownPlayerPosition;
            return true;
        }
        lastPosition = Vector3.zero;
        return false;
    }
    void OnTriggerEnter(Collider collider) {
        // found a player?
        var player = collider.GetComponentInParent<Player>();
        if (player != null) {
                    Debug.Log("found");

            activePlayerInTriggerBounds = player;
            hasLastKnownPlayerPosition = false;
            lastKnownPlayerPosition = Vector3.zero;
            controller.OnPlayerDetected(player);
            return;
        }
        // found an objective marker?
        var objective = collider.GetComponentInParent<IEnemyObjectiveMarker>();
        if (objective != null) {
            objectiveMarkers.Add(objective);
            controller.OnObjectiveDetected(objective);
        }
    }
    void OnTriggerExit(Collider collider) {
        // had a player?
        var player = collider.GetComponentInParent<Player>();
        if (player != null) {
            hasLastKnownPlayerPosition = true;
            lastKnownPlayerPosition = player.transform.position;
            controller.OnPlayerLost();
        }
    }
    #endregion
}
