using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// class for spawning projectiles from an object
/// a simple update class that spawns a projectile from the spawner
/// </summary>

public class ProjectileSpawner : MonoBehaviour
{
    
    //the offset for firing projectiles, child transform on the object
    [InjectComponent] public FirePoint projectileSpawnLocation;

    //initialize a projectile config
    public ProjectileSpawnerProjectileConfig projectileInSpawner;

    //setting values for the projectile's cooldown and timer and if this spawner is active
    private float spawnCooldown = 2f;
    private float spawnTimer = 2f;
    public bool projectileSpawnerActive = true;

    public void Start(){}

    //spawns a projectile after the timer reaches 0 and resets
    //it also checks if the ProjectileSpawner is active or not
    public void Update()
    {
        if (spawnTimer < 0)
        {
            ProjectileSpawnerProjectile.Spawn(projectileInSpawner, projectileSpawnLocation);
            spawnTimer = spawnCooldown;
        }
        if (projectileSpawnerActive)
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    //checks if the spawner is able to spawn
    //for future implementation this can be called from the update function
    //to check if the spawner should be active or not
    public void CanSpawn()
    {

    }
}
