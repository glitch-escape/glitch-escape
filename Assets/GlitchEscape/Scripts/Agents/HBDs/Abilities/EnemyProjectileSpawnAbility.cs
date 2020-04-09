using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProjectileSpawnAbility : EnemyAbility {

    private NavMeshAgent agent => enemy.navMeshAgent;

    private float curAtkTime, curCooldwn;
    private int shotsMade;
    private bool isAttack =>
        curAtkTime < abilityDuration && curAtkTime > 0;

    #region Projectile Implementation
    // Attack variables
    private float strikeDist => enemy.config.shootDistance;
    private float projectileRate => enemy.config.projectileShotsPerSecond;
    private float startup => enemy.config.projectileStartup;

    /// <summary>
    /// Stamina cost per projectile
    /// </summary>
    public override float resourceCost => enemy.config.projectileStaminaCost;

    /// <summary>
    /// Defines how long it takes for the ability to be used again
    /// </summary>
    public override float cooldownTime => enemy.config.projectileCooldown;

    /// <summary>
    /// Defines how long the enemy is placed in the attack state
    /// </summary>
    protected override float abilityDuration => enemy.config.attackDuration;
    protected override bool hasSetDuration => true;

    /// <summary>
    /// location (on enemy) to spawn the projectile at
    /// Should be a child transform on the player object w/ a FirePoint (empty marker script) on it
    /// </summary>
    [InjectComponent] public FirePoint projectileSpawnLocation;

    /// <summary>
    /// projectile config to use (pulls from <see cref="EnemyConfig"/> on the player object)
    /// </summary>
    private EnemyProjectileConfig projectileConfig => enemy.config.attackProjectile;
    #endregion

    public override bool AbilityFinished(out EnemyBehaviorState nextAction) {
        nextAction = EnemyBehaviorState.ChasingPlayer;
        return isAbilityActive;
    }

    protected override void OnAbilityStart() {
        curAtkTime = 0;
        curCooldwn = 0;
        shotsMade = 0;

        // Make the enemy stand still
        agent.SetDestination(enemy.transform.position);
    }

    protected override void OnAbilityUpdate() {
        base.OnAbilityUpdate();

        curAtkTime += Time.deltaTime;
        if ((shotsMade * projectileRate) + startup < curAtkTime) {
            shotsMade += 1;
            EnemyProjectile.Spawn(projectileConfig, projectileSpawnLocation);
        }
    }

    protected override void OnAbilityEnd() {
        base.OnAbilityEnd();
        curCooldwn = Time.time;
        curAtkTime = 0;
    }

    protected override void OnAbilityReset() {
        base.OnAbilityReset();
        OnAbilityEnd();
    }

    protected override bool CanStartAbility() {
        return base.CanStartAbility();
    }

}
