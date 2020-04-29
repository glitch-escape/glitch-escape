using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class PolysphereTest : MonoBehaviour {
    public Mesh initialMesh;
    public float majorOffsetRange = 0.5f;
    public float minorOffsetRange = 0.05f;
    public float rotationYawSpeed = 180f;
    public float rotationPitchSpeed = 180f;

    void ExplodeMesh() {
        var mf = GetComponent<MeshFilter>();
        var mesh = initialMesh;
        var v0 = mesh.vertices;
        var t0 = mesh.triangles;
        var u0 = mesh.uv;
        var n = mesh.triangles.Length;

        var tris = new int[n];
        var verts = new Vector3[n];
        var uvs = new Vector2[n];

        float yoffset = 0f;
        var camera = Camera.main;
        for (int i = 0; i < n; ++i) {
            if (i % 3 == 0) {
                yoffset = Random.Range(-majorOffsetRange, +majorOffsetRange);
            }
            tris[i] = i;
            verts[i] = v0[t0[i]];
            var worldPoint = v0[t0[i]];
            var screenPoint = camera.WorldToScreenPoint(worldPoint);
            var ray = camera.ScreenPointToRay(screenPoint);
            var distance = Vector3.Distance(worldPoint, camera.transform.position);
            verts[i] = ray.GetPoint(distance + yoffset + Random.Range(-minorOffsetRange, +minorOffsetRange));
            uvs[i] = u0[t0[i]];
        }
        var newMesh = new Mesh();
        newMesh.vertices = verts;
        newMesh.uv = uvs;
        newMesh.triangles = tris;
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();
        newMesh.RecalculateBounds();
        mf.mesh = newMesh;
    }

    public bool reset = false;
    public bool explodedMesh = false;

    public AnimationCurve inputCurve;

    public float yaw = 0f;
    public float pitch = 0f;
    public float snapThreshold = 2.0f; // degrees
    public float winThreshold = 1f; // degrees
    private bool finishedPuzzle = false;
    private float puzzleFinishTime = 0f;
    public float puzzleFinishDelay = 2f; // seconds

    private float ApplyInputCurve(float input) {
        var sign = input >= 0f ? 1f : -1f;
        return inputCurve.Evaluate(Mathf.Abs(input)) * sign;
    }
    void Restart() {
        finishedPuzzle = false;
        yaw = Random.Range(-180f, +180f);
        pitch = Random.Range(-180f, +180f);
        transform.rotation = Quaternion.identity;
        ExplodeMesh();
        
    }
    void Start() {
        Restart(); 
    }

    void EditorDebugUpdate() {
        if (!explodedMesh) {
            explodedMesh = true;
            ExplodeMesh();
        } else if (reset) {
            reset = false;
            if (initialMesh != null)
                GetComponent<MeshFilter>().mesh = initialMesh;
        }
    }

    public float yawDist;
    public float pitchDist;
    public float distance;
    
    void Update() {
        EditorDebugUpdate();

        if (finishedPuzzle) {
            if (Time.time >= puzzleFinishTime) {
                Restart();
            }
            return;
        }
        // yawDist = yaw % 360f;
        // yawDist = Mathf.Min(yawDist, 180f - yawDist);
        // pitchDist = Mathf.Abs(pitch % 360f);
        // pitchDist = Mathf.Min(pitchDist, 180f - pitchDist);
        yawDist = Mathf.Abs(yaw) % 180f;
        pitchDist = Mathf.Abs(pitch) % 180f;
        distance = Mathf.Sqrt(yawDist * yawDist + pitchDist * pitchDist);
        if (distance < winThreshold) {
            finishedPuzzle = true;
            puzzleFinishTime = Time.time + puzzleFinishDelay;
            GetComponent<MeshFilter>().mesh = initialMesh;
            return;
        }

        var gamepad = Gamepad.current;
        if (gamepad != null) {
            var input = gamepad.rightStick.ReadValue();
            input.x = ApplyInputCurve(input.x);
            input.y = ApplyInputCurve(input.y);
            var newYaw = yaw + input.x * rotationYawSpeed * Time.deltaTime;
            var newPitch = pitch + input.y * rotationPitchSpeed * Time.deltaTime;

            if (Mathf.Abs(newYaw) < snapThreshold) {
                newYaw = 0f;
            }
            if (Mathf.Abs(newPitch) < snapThreshold) {
                newPitch = 0f;
            }
            pitch = newPitch;
            yaw = newYaw;

            transform.rotation = Quaternion.Euler(new Vector3(
                pitch, yaw, 0f
            ));
        }
    }
}
