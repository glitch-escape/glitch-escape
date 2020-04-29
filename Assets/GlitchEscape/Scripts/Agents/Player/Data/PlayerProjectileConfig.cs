using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Config data for a player projectile.
/// Implementation in <see cref="ProjectileConfig{TProjectile,TProjectileConfig}"/>
/// </summary>
[CreateAssetMenu(fileName = "PlayerProjectile", menuName = "Config/Player/PlayerProjectileConfig", order = 1)]
public class PlayerProjectileConfig : ProjectileConfig<PlayerProjectile, PlayerProjectileConfig> {}
