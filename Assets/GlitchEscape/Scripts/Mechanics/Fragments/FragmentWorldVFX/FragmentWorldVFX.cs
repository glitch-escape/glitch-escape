using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FragmentWorldVFX : MonoBehaviour {
    private FragmentWorldVFXParticle[] particles;
    public FragmentWorldVFXParticle particle;
    public uint numParticles;
    
    public float spawnRadius = 0.5f;
    public float spawnHeight = 1.5f;
    public float spawnHeightPowerBias = 1.5f;

    public enum SpawnVolume { Sphere, Ellipsoid, DiamondEllipsoid }
    public SpawnVolume spawnVolume;

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
                var xz = Random.insideUnitCircle * spawnRadius;
                return new Vector3(xz.x, y, xz.y) + transform.position;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    Quaternion GetRandomRotation() {
        return Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.up);
    }
    private Random.State spawnSeed;
    public void Respawn() {
        particles = GetComponentsInChildren<FragmentWorldVFXParticle>();
        foreach (var particle in particles) {
            DestroyImmediate(particle.gameObject);
        }
        spawnSeed = Random.state;
        particles = new FragmentWorldVFXParticle[numParticles];
        for (var i = 0; i < numParticles; ++i) {
            particles[i] = Instantiate(this.particle, 
                GetRandomPosition(),
                GetRandomRotation(),
                transform);
        }
    }
    public void RecalculateSpawnPositions() {
        var seed = Random.state;
        Random.state = spawnSeed;
        if (numParticles != (particles?.Length ?? 0)) {
            Respawn();
        } else {
            foreach (var particle in particles) {
                particle.transform.position = GetRandomPosition();
                particle.transform.rotation = GetRandomRotation();
            }
        }
        Random.state = seed;
    }
    
    void Start() {
        if (particles == null) {
            Respawn();
        }
    }
    void Update() {
        foreach (var particle in particles) {
            
        }
    }
}
