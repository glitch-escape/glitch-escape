using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base projectile class (see <see cref="PlayerProjectile"/>)
/// Uses <see cref="ProjectileConfig{TProjectile,TProjectileConfig}"/> for damage, speed, etc., variables
/// </summary>
[RequireComponent(typeof(Autowire))]
[RequireComponent(typeof(Collider))]
public abstract class Projectile<TDerivedProjectile, TProjectileConfig> : Attack 
    where TDerivedProjectile : Projectile<TDerivedProjectile, TProjectileConfig>
    where TProjectileConfig : ProjectileConfig<TDerivedProjectile, TProjectileConfig>
{
    [InjectComponent] public new Rigidbody rigidbody;
    protected TProjectileConfig config { get; private set; }
    protected float spawnTime { get; private set; }

    /// <summary>
    /// Spawns a projectile.
    /// </summary>
    /// <remark>You MUST use this method to create projectiles.</remark>
    /// <param name="point">Point to spawn the projectile at</param>
    /// <param name="config">Passed in projectile config</param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static TDerivedProjectile Spawn (
        TProjectileConfig config,
        FirePoint point,
        Transform parent = null) {
        var projectile = parent != null
            ? GameObject.Instantiate(config.prefab, point.transform.position, point.transform.rotation, parent)
            : GameObject.Instantiate(config.prefab, point.transform.position, point.transform.rotation);
        projectile.config = config;
        projectile.spawnTime = Time.time;
        projectile.rigidbody.velocity = point.transform.forward * config.speed;
        if (config.lifetime > 0f) Destroy(projectile.gameObject, config.lifetime);
        return projectile;
    }
    //private void OnCollisionEnter(Collision other) {
    private void OnTriggerEnter(Collider other)
    {
        var agent = other.gameObject.GetComponent<IAgent>();
        if (agent != null && agent.agentType == targetType)
        {
            agent.TakeDamage(config.damage);
            Destroy(gameObject);
        }
        if (agent == null && Time.time > spawnTime+0.1f)
        {
            Destroy(gameObject);
        }
    }
}
