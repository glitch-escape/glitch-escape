using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player-spawned projectile.
/// Implementation in <see cref="Projectile{TDerivedProjectile,TProjectileConfig}"/>
/// Config data in <see cref="PlayerProjectileConfig"/>
/// </summary>
public class PlayerProjectile : Projectile<PlayerProjectile, PlayerProjectileConfig> {
    public override AgentType targetType => AgentType.Enemy;
}
