using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseAction : EnemyAbility {
    protected override bool hasSetDuration => false;

    private NavMeshAgent agent => enemy.navMeshAgent;
    private Player player => enemy.player;

    public override bool AbilityFinished(out EnemyBehaviorState nextAction) {
        nextAction = EnemyBehaviorState.ChasingPlayer;
        return false;
    }

    #region Base Ability Implementation
    public override float resourceCost => 0;
    public override float cooldownTime => 0;
    protected override float abilityDuration => 1;

    protected override void OnAbilityStart() {
        // Update destination to current player position
        agent.SetDestination(player.transform.position);
    }

    protected override void OnAbilityUpdate() {
        base.OnAbilityUpdate();
        // Update destination to current player position
        agent.SetDestination(player.transform.position);
    }
    #endregion
}
