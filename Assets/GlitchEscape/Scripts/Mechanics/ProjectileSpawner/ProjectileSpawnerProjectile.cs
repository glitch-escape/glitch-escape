using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Projectile spawner projectile.
/// Implementation in <see cref="Projectile{TDerivedProjectile,TProjectileConfig}"/>
/// </summary>
public class ProjectileSpawnerProjectile : Projectile<ProjectileSpawnerProjectile, ProjectileSpawnerProjectileConfig> {
    public override AgentType targetType => AgentType.Player;
}
