using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {
    public enum RotationMode {
        None,
        YAxis,
        XAxis,
        FullAxes
    };
    public RotationMode rotationMode = RotationMode.FullAxes;
    private Camera camera;
    void Start() {
        if (camera == null) { 
            camera = Camera.main;
        }
    }
    void TurnToFaceCamera() {
        if (rotationMode == RotationMode.None) return;
        var toCamera = transform.position - camera.transform.position;
        switch (rotationMode) {
            case RotationMode.YAxis: toCamera.y = 0f; break;
            case RotationMode.XAxis: toCamera.x = 0f; break;
            case RotationMode.FullAxes: break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        toCamera.Normalize();
        transform.rotation = Quaternion.LookRotation(toCamera);
    }
    void Update() {
        TurnToFaceCamera();
    }
    private void OnEnable() {
        TurnToFaceCamera();
    }
}
