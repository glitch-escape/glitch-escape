using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class FloatingObject : MonoBehaviour {
    private Vector3 centerPosition;
    private float startTime;

    private void OnEnable() {
        Setup();
    }
    public void Setup() {
        centerPosition = transform.position; 
        startTime = Time.time;
        if (randomizeFloatStartTimes) {
            startTime += Random.Range(-cycleDuration, +cycleDuration);
        }
    }
    
    public float cycleDuration = 1f;
    public float floatOffsetDistance = 0.2f;
    public bool randomizeFloatStartTimes;
    
    void Update() {
        var t = (Time.time - startTime) / cycleDuration;
        var cycle = Mathf.Sin(t * Mathf.PI * 2f);
        transform.position = centerPosition + Vector3.up * cycle * floatOffsetDistance;
    }
}
