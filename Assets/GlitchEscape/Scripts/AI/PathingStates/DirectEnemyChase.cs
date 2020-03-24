using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DirectEnemyChase : MonoBehaviour, IEnemyPursuitAction {

    private Enemy _oldEnemy;
    private OldEnemyController _oldEnemyController;
    private NavMeshAgent agent;
    private Player player;

    // Initialize component
    public void SetupControllerComponent(OldEnemyController controller) {
        // Get references
        _oldEnemyController = controller;
        _oldEnemy = controller.oldEnemy;
        agent = _oldEnemy.navMeshAgent;
        player = controller.player;
    }

    public void StartAction() {
        // Update destination to current player position
        agent.SetDestination(player.transform.position);
    }
    public void EndAction() { }

    public void UpdateAction() {
        // Update destination to current player position
        agent.SetDestination(player.transform.position);

/*
        float playerDist = Vector3.Distance(player.transform.position, transform.position);
        if (curAtk != null && curAtk.distance >= playerDist) {
            SetupAttack();
        }
        */
    }

    // Ends when _oldEnemyController decides it's ok to attack
    public bool ActionFinished(out EnemyBehaviorState nextAction) {
        nextAction = EnemyBehaviorState.ChasingPlayer;
        return false;
    }
    public bool CanActivate(Player player) {
        return true;
    }
}
