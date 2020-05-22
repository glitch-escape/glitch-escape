using System;
using System.Collections;
using System.Collections.Generic;
using GlitchEscape.Scripts.Utility;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


[ExecuteInEditMode]
public class FragmentWorldVFX : MonoBehaviour {
    public bool simulateInEditMode = false;
    private FragmentWorldVFXParticle[] particles;
    public FragmentParticleVisualConfig particleVisualConfig;
    public uint numParticles;
    public float spawnRadius = 0.5f;
    public float spawnHeight = 1.5f;
    public float spawnHeightPowerBias = 1.5f;

    public enum SpawnVolume { Sphere, Ellipsoid, DiamondEllipsoid }
    public SpawnVolume spawnVolume;

    [Range(0.5f, 4f)]
    public float scale = 1f;
    [Range(0f, 3f)]
    public float scaleVariation = 0f;
    
    public float yOffset = 0.2f;
    public float yOffsetRange = 0.05f;

    public float timeOffsetRange = 0f;
    public float speed = 1f;
    public float speedRange = 0f;
    
    public bool initialized => particles != null;
    
    Vector3 GetRandomPosition() {
        switch (spawnVolume) {
            case SpawnVolume.Sphere:
                return Random.insideUnitSphere * spawnRadius + transform.position;
            case SpawnVolume.Ellipsoid:
                var pos = Random.insideUnitSphere;
                pos.y *= spawnHeight * 0.5f;
                pos.x *= spawnRadius;
                pos.z *= spawnRadius;
                return pos + transform.position;
            case SpawnVolume.DiamondEllipsoid:
                var t = Mathf.Pow(Random.value, spawnHeightPowerBias);
                var sign = Random.value < 0.5f ? -1f : 1f;
                var y = sign * t * spawnHeight * 0.5f;
                var xz = Random.insideUnitCircle * spawnRadius * (1f - t);
                return new Vector3(xz.x, y, xz.y) + transform.position;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    Quaternion GetRandomRotation() {
        return Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.up);
    }

    FragmentWorldVFXParticle CreateParticle() {
        var particle = new GameObject("FragmentParticle",
                typeof(MeshFilter), typeof(MeshRenderer), typeof(FragmentWorldVFXParticle))
            .GetComponent<FragmentWorldVFXParticle>();
        particle.transform.parent = transform;
        var renderer = particle.GetComponent<MeshRenderer>();
        renderer.materials = particleVisualConfig.materials;
        Init(particle);
        return particle;
    }
    void Init(FragmentWorldVFXParticle particle) {
        var meshes = particleVisualConfig.meshes;
        if (meshes != null && meshes.Length > 0) {
            var i = (int)Mathf.Clamp(Mathf.Floor(Random.value * meshes.Length - 1e-6f), 0f, meshes.Length - 1);
            particle.GetComponent<MeshFilter>().mesh = meshes[i];
        }
        particle.transform.position = GetRandomPosition();
        particle.transform.rotation = GetRandomRotation();
        particle.transform.localScale =
            particleVisualConfig.scale * ((1f + Random.Range(-1f, 1f) * scaleVariation) * scale);
        particle.Init(
            Random.value * yOffsetRange + yOffset,
            Random.value * timeOffsetRange,
            Random.value * speedRange + speed
        );
    }
    
    [SerializeField]
    private Random.State spawnSeed;
    public void Respawn() {
        startTime = Time.time;
        particles = GetComponentsInChildren<FragmentWorldVFXParticle>();
        foreach (var particle in particles) {
            DestroyImmediate(particle.gameObject);
        }
        // spawnSeed = Random.state;
        particles = new FragmentWorldVFXParticle[numParticles];
        for (var i = 0; i < numParticles; ++i) {
            particles[i] = CreateParticle();
        }
    }
    public void RecalculateSpawnPositions() {
        var seed = Random.state;
        Random.state = spawnSeed;
        if (numParticles != (particles?.Length ?? 0)) {
            Respawn();
        } else {
            foreach (var particle in particles) {
                Init(particle);
            }
        }
        Random.state = seed;
    }

    private float startTime = 0f;
    void Start() {
        startTime = Time.time;
        if (particles == null) {
            particles = GetComponentsInChildren<FragmentWorldVFXParticle>();
        }
        if (particles.Length != numParticles) {
            Respawn();
        }
    }
    public void Update() {
        foreach (var particle in particles) {
            particle.Animate(Time.time - startTime);
        }
    }
}
