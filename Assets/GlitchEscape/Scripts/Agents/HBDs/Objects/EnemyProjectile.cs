using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy-spawned projectile.
/// Implementation in <see cref="Projectile{TDerivedProjectile,TProjectileConfig}"/>
/// Config data in <see cref="PlayerProjectileConfig"/>
/// </summary>
public class EnemyProjectile : Projectile<EnemyProjectile, EnemyProjectileConfig> {
    public override AgentType targetType => AgentType.Player;
}