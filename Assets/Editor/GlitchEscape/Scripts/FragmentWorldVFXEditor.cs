using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
}
