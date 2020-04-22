using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackAbility : EnemyAbility {

    private NavMeshAgent agent => enemy.navMeshAgent;
    private int shotsMade;

    // Sets the values of attack variables
    #region Attack Variables
    EnemyConfig.AttackData attackData => enemy.config.attacks[configID];

    // Attack variables
    private float strikeDist => attackData.strikeDistance;
    private float projectileRate => 1/attackData.shotsPerSecond;
    private float startup => attackData.startup;

    /// <summary>
    /// Stamina cost per projectile
    /// </summary>
    public override float resourceCost => attackData.staminaCost;

    /// <summary>
    /// Defines how long it takes for the ability to be used again
    /// </summary>
    public override float cooldownTime => attackData.cooldown;

    /// <summary>
    /// Defines how long the enemy is placed in the attack state
    /// </summary>
    protected override float abilityDuration => attackData.duration;
    protected override bool hasSetDuration => true;

    /// <summary>
    /// location (on enemy) to spawn the projectile at
    /// Should be a child transform on the player object w/ a FirePoint (empty marker script) on it
    /// </summary>
    [InjectComponent] public FirePoint projectileSpawnLocation;

    /// <summary>
    /// projectile config to use (pulls from <see cref="EnemyConfig"/> on the player object)
    /// </summary>
    private EnemyProjectileConfig projectileConfig => attackData.attackProjectile;
    #endregion

    public override void SetConfigID(int id) {
        if (id >= 0 && id < enemy.config.attacks.Length) {
            configID = id;
        }
        else {
            base.SetConfigID(id);
        }
        print(id);
    }

    public override bool AbilityFinished(out EnemyBehaviorState nextAction) {
        nextAction = EnemyBehaviorState.ChasingPlayer;
        return !isAbilityActive;
    }

    protected override void OnAbilityStart() {
        shotsMade = 0;

        // Make the enemy stand still
        agent.SetDestination(enemy.transform.position);
    }

    protected override void OnAbilityUpdate() {
        base.OnAbilityUpdate();

        if ((shotsMade * projectileRate) + startup < timeElapsedSinceAbilityStart) {
            shotsMade += 1;
            EnemyProjectile.Spawn(projectileConfig, projectileSpawnLocation);
        }
    }

    protected override void OnAbilityEnd() {
        base.OnAbilityEnd();
    }

    protected override void OnAbilityReset() {
        base.OnAbilityReset();
        OnAbilityEnd();
    }

    protected override bool CanStartAbility() {
        // Check if attack is in range
        Vector3 foePos = enemy.transform.position;
        Vector3 playPos = enemy.player.transform.position;
        if (Vector3.Distance(foePos, playPos) > strikeDist)
            return false;

        return true;
    }

}
