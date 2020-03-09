using UnityEngine;

public abstract class BaseAbility<Agent, Config> : MonoBehaviourBorrowingConfigFrom<Agent, Config> 
    where Agent : class, IConfigurable<Config>
    where Config : ScriptableObject 
{
    [InjectComponent] public Agent agent;
    protected abstract bool canUseAbility { get; }
    protected abstract float baseCost { get; }
    protected abstract bool hasVaryingCost { get; }
    protected abstract float maxVaryingCost { get; }
}

public abstract class PlayerAbility<Config> : BaseAbility<Player, Config>
    where Config : ScriptableObject 
{
    protected override bool canUseAbility => agent.stamina.value >= baseCost;
}

public class FirePoint : MonoBehaviour {}

public class BaseAttackConfig : ScriptableObject {
    public float damage = 10f;
    public float speed = 30f;
    public float staminaCost = 5f;
    public float attacksPerSec = 2f;
}
public class RangedAttackConfig : BaseAttackConfig {
    public Projectile projectile;
    public float range = 30f;
}
public enum AgentType {
    None,
    Player,
    Friendly,
    Enemy
}

public abstract class RangedAttackAbility<Agent, Config> : BaseAbility<Agent, Config> 
    where Agent : class, IConfigurable<Config>
    where Config : ScriptableObject
{
    /// <summary>
    /// Point to spawn projectile at
    /// </summary>
    [InjectComponent] public FirePoint firePoint;
    protected abstract RangedAttackConfig attackConfig { get; }
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

public class PlayerRangedAttackAbility : RangedAttackAbility<Player, PlayerConfig> {
    
    [InjectComponent] public PlayerControls controls;
    private void OnEnable() { controls.shoot.onPressed += OnPressFire; }
    private void OnDisable() { controls.shoot.onPressed -= OnPressFire; }
    private void OnPressFire() { TryStartAbility(); }
    
    protected override AgentType attackTarget => AgentType.Player;
    protected override RangedAttackConfig attackConfig => config.rangedAttack1;
}












