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
    private Camera _camera;

    // contrived fix to get the main camera, b/c Camera.current + Camera.main
    // MAY BE NULL IN START / AWAKE?!!!!!!
    Camera camera {
        get {
            if (_camera == null) {
                _camera = 
                    Camera.main ?? 
                    Camera.current ??
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                if (_camera == null) {
                    Debug.LogError("COULD NOT LOCATE MAIN CAMERA?????");
                }
            }
            return _camera;
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
