using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolAction : EnemyAbility {
    protected override bool hasSetDuration => false;

    private NavMeshAgent agent => enemy.navMeshAgent;
    public Transform[] patrolPoints;
    public float ptLeniency;

    private int curDest;
    private bool isReturnTrip, isIdle;

    public override bool AbilityFinished(out EnemyBehaviorState nextAction) {
        nextAction = EnemyBehaviorState.Idle;
        return false;
    }

    #region Base Ability Implementation
    public override float resourceCost => 0;
    public override float cooldownTime => 0;
    protected override float abilityDuration => 1;

    protected override void OnAbilityStart() {
        if (patrolPoints.Length > 0) {
            agent.SetDestination(patrolPoints[curDest].position);
        }
        else {
            isIdle = true;
        }
    }

    protected override void OnAbilityUpdate() {
        base.OnAbilityUpdate();
        // Update destination point if needed
        if (Vector3.Distance(enemy.transform.position, agent.destination) <= ptLeniency) {
            if (isReturnTrip) {
                curDest -= 1;
                if (curDest < 0) {
                    isReturnTrip = false;
                    curDest += 1;
                }
            }
            else {
                curDest += 1;
                if (curDest >= patrolPoints.Length) {
                    isReturnTrip = true;
                    curDest -= 1;
                }
            }

            agent.SetDestination(patrolPoints[curDest].position);
        }
    }
    #endregion

}
