using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileConfig<TProjectile, TProjectileConfig> : ScriptableObject 
    where TProjectile : Projectile<TProjectile, TProjectileConfig>
    where TProjectileConfig : ProjectileConfig<TProjectile, TProjectileConfig>
{
    public TProjectile prefab;
    public float damage = 10f;
    public float speed = 30f;
    public float lifetime = 10f;
    public bool doesLinger = false;
}
