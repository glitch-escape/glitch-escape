using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour, IEnemySearchForPlayerAction {
    private Enemy enemy;
    private EnemyController enemyController;
    private NavMeshAgent agent;
    private Player player;

    public Transform[] patrolPoints;
    public float ptLeniency;

    private int curDest;
    private bool isReturnTrip, isIdle;

    // Initialize component
    public void SetupControllerComponent(EnemyController controller) {
        // Get references
        enemyController = controller;
        enemy = controller.enemy;
        agent = enemy.navMeshAgent;
        player = enemyController.player;
    }

    public void StartAction() {
        if (patrolPoints.Length > 0) {
            agent.SetDestination(patrolPoints[curDest].position);
        }
        else {
            isIdle = true;
        }
    }
    public void EndAction() { }
    public void UpdateAction() {
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

    public bool ActionFinished(out EnemyBehaviorState nextAction) {
        if (isIdle) {
            nextAction = EnemyBehaviorState.Idle;
        }
        else {
            nextAction = EnemyBehaviorState.SearchingForPlayer;
        }
        return isIdle;
    }
    public bool CanActivate(Player player) {
        return true;
    }
}
