using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectileAttackAbility<Agent, AgentConfig, AttackConfig> : BaseAttackAbility<Agent, AgentConfig, AttackConfig>
    where Agent : class, IConfigurable<AgentConfig>
    where AgentConfig : ScriptableObject
    where AttackConfig : ProjectileAttackConfig
{
    /// <summary>
    /// Point to spawn projectile at
    /// </summary>
    [InjectComponent] public FirePoint firePoint;
    protected abstract AgentType attackTarget { get; }
    
    protected override float duration => 0f;
    protected override float cost => attackConfig.staminaCost;
    protected override float cooldown => 1f / attackConfig.attacksPerSec;

    protected override void OnStartAbility() {
        var t = firePoint?.transform ?? transform;
        Stateful.Instantiate(
            attackConfig.projectile, t.position, t.rotation,
            (Projectile projectile) => {
                projectile.targetType = attackTarget;
                projectile.speed = attackConfig.speed;
                projectile.range = attackConfig.range;
                projectile.damage = attackConfig.damage;
            });
    }
}
