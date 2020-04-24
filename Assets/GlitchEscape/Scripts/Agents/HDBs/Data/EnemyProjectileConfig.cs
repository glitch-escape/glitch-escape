using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Config data for aa enemy projectile.
/// Implementation in <see cref="ProjectileConfig{TProjectile,TProjectileConfig}"/>
/// </summary>
[CreateAssetMenu(fileName = "EnemyProjectile", menuName = "Config/Enemy/EnemyProjectileConfig", order = 1)]
public class EnemyProjectileConfig : ProjectileConfig<EnemyProjectile, EnemyProjectileConfig> { }
