using UnityEngine;

public abstract class BaseAbility<Agent, Config> : MonoBehaviourBorrowingConfigFrom<Agent, Config> 
    where Agent : class, IAgent, IConfigurable<Config>
    where Config : ScriptableObject 
{
    [InjectComponent] public Agent agent;
    protected abstract bool canUseAbility { get; }
    protected abstract float baseCost { get; }
    protected abstract bool hasVaryingCost { get; }
    protected abstract float maxVaryingCost { get; }    
}

// public abstract class PlayerAbility<Config> : BaseAbility<Player, Config>
//     where Config : ScriptableObject 
// {
//     protected override bool canUseAbility => agent.stamina.value >= baseCost;
// }
//
// public class FirePoint : MonoBehaviour {}
//
//
// public class PlayerRangedAttackAbility : BaseProjectileAttackAbility<Player, PlayerConfig, ProjectileAttackConfig> {
//     
//     [InjectComponent] public PlayerControls controls;
//     private void OnEnable() { controls.shoot.onPressed += OnPressFire; }
//     private void OnDisable() { controls.shoot.onPressed -= OnPressFire; }
//     private void OnPressFire() { TryStartAbility(); }
//     
//     protected override AgentType attackTarget => AgentType.Player;
//     protected override ProjectileAttackConfig attackConfig => config.rangedAttack1;
//     protected override AgentType attackTarget { get; }
//     protected override bool canUseAbility { get; }
//     protected override float baseCost { get; }
//     protected override bool hasVaryingCost { get; }
//     protected override float maxVaryingCost { get; }
// }












