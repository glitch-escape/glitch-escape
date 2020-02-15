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

    private float yaw = 0f;
    private float pitch = 0f;
    public float snapThreshold = 2.0f; // degrees

    void Start() {
        ExplodeMesh();
        yaw = Random.Range(-180f, +180f);
        pitch = Random.Range(-180f, +180f);
    }
    void Update() {
        if (!explodedMesh) {
            explodedMesh = true;
            ExplodeMesh();
        } else if (reset) {
            reset = false;
            if (initialMesh != null)
                GetComponent<MeshFilter>().mesh = initialMesh;
        }

        var gamepad = Gamepad.current;
        if (gamepad != null) {
            var input = gamepad.rightStick.ReadValue();
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
            // if (input.y != 0f) {
            //     yaw += 
            //     
            //     transform.Rotate(Vector3.right, );   
            // }
            // if (input.x != 0f) {
            //     transform.Rotate(Vector3.up, input.x * rotationYawSpeed * Time.deltaTime);   
            // }
        }
    }
}
