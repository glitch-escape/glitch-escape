using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Config data for a projectile from the projectile spawner.
/// Implementation in <see cref="ProjectileConfig{TProjectile,TProjectileConfig}"/>
/// </summary>
[CreateAssetMenu(fileName = "ProjectileSpawnerProjectile", menuName = "Config/Enemy/ProjectileSpawnerProjectileConfig", order = 1)]
public class ProjectileSpawnerProjectileConfig : ProjectileConfig<ProjectileSpawnerProjectile, ProjectileSpawnerProjectileConfig> {}