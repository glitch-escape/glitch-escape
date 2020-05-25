using System;
using System.Collections;
using System.Collections.Generic;
using GlitchEscape.Scripts.DebugUI;
using UnityEngine;

public class VFXDebugController : MonoBehaviour, IPlayerDebug {
    private float currentLogParticleScale = 0f;
    
    public void DrawDebugUI() {
        var particleSystems = FindObjectsOfType<FragmentWorldVFX>();
        GUILayout.Label("active particle system(s): " + particleSystems.Length);
        
        currentLogParticleScale = GUILayout.HorizontalSlider(currentLogParticleScale, -4f, +4f);
        var globalParticleCountScale = Mathf.Pow(currentLogParticleScale, 2f);
        GUILayout.Label("Drawing " + Mathf.Round(globalParticleCountScale * 100f) + "% particle(s)");
        
        int i = 0;
        foreach (var particleSystem in particleSystems) {
            GUILayout.Label(
                "   particle system " + i + " '" + particleSystem.name + "' " +
                " (" + particleSystem.currentParticleCount + " active particle(s))");
            particleSystem.particleCountScale = globalParticleCountScale;
        }
    }
    public string debugName => GetType().Name;
}
