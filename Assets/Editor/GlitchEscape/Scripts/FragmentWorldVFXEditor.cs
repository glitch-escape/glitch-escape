﻿using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(FragmentWorldVFX))]
public class FragmentWorldVFXEditor : Editor {
    public override void OnInspectorGUI() {
        var particleSystem = (FragmentWorldVFX) target;
        var num = particleSystem.numParticles;
        var radius = particleSystem.spawnRadius;
        var height = particleSystem.spawnHeight;
        var power = particleSystem.spawnHeightPowerBias;
        var volume = particleSystem.spawnVolume;
        var yOffset = particleSystem.yOffset;
        var yOffsetRange = particleSystem.yOffsetRange;
        var timeRange = particleSystem.timeOffsetRange;
        var speed = particleSystem.speed;
        var speedRange = particleSystem.speedRange;
        
        if (GUILayout.Button("Spawn particles")) {
            particleSystem.Respawn();
        }
        base.OnInspectorGUI();
        
        if (particleSystem.initialized && (
            num != particleSystem.numParticles ||
            radius != particleSystem.numParticles ||
            height != particleSystem.spawnHeight ||
            power != particleSystem.spawnHeightPowerBias ||
            volume != particleSystem.spawnVolume ||
            yOffset != particleSystem.yOffset ||
            yOffsetRange != particleSystem.yOffsetRange ||
            timeRange != particleSystem.timeOffsetRange ||
            speed != particleSystem.speed ||
            speedRange != particleSystem.speedRange
        )) {
            particleSystem.RecalculateSpawnPositions();
        }
    }

    public void OnSceneGUI() {
        if (!EditorApplication.isPlaying && ((FragmentWorldVFX)target).simulateInEditMode) {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
    }
}
#endif