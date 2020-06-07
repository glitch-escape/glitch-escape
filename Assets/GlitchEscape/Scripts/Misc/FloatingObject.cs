using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class FloatingObject : MonoBehaviour {
    private Vector3 centerPosition;
    private Quaternion initialRotation;
    private float startTime;
    private float rotationTimeOffset;

    private void OnEnable() {
        Setup();
    }
    public void Setup() {
        centerPosition = transform.position; 
        startTime = Time.time;
        if (randomizeFloatStartTimes) {
            startTime += Random.Range(-cycleDuration, +cycleDuration);
        }
        initialRotation = transform.rotation;
        rotationTimeOffset = Random.Range(-rotationCycleDuration, +rotationCycleDuration);
    }
    
    public float cycleDuration = 10f;
    public float floatOffsetDistance = 0.05f;
    public bool randomizeFloatStartTimes;
    public bool applyRotation = true;
    public float rotationCycleDuration = 4f;
    public float rotationDegrees = 15f;
    
    void Update() {
        var t = (Time.time - startTime) / cycleDuration;
        var cycle = Mathf.Sin(t * Mathf.PI * 2f);
        transform.position = centerPosition + Vector3.up * cycle * floatOffsetDistance;

        if (applyRotation) {
            var t2 = (Time.time - startTime + rotationTimeOffset) / rotationCycleDuration;
            var angle = Mathf.Sin(t2 * Mathf.PI * 2f) * rotationDegrees;
            transform.rotation = initialRotation * Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
}
