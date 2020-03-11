using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttackAbility<Agent, AgentConfig, AttackConfig> : BaseAbility<Agent, AgentConfig>
    where Agent : BaseAgent<AgentConfig>
    where AgentConfig : ScriptableObject
    where AttackConfig : BaseAttackConfig
{
    protected abstract AttackConfig attackConfig { get; }
    protected abstract AgentType attackTarget { get; }
    protected override float duration => 0f;
    protected override float cost => attackConfig.staminaCost;
    protected override float cooldown => 1f / attackConfig.attacksPerSec;
    protected override float canUseAbility => agent.
    
}
