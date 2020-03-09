using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DirectEnemyChase : MonoBehaviour, IEnemyPursuitAction {

    private Enemy enemy;
    private EnemyController enemyController;
    private NavMeshAgent agent;
    private Player player;

    // Initialize component
    public void SetupControllerComponent(EnemyController controller) {
        // Get references
        enemyController = controller;
        enemy = controller.enemy;
        agent = enemy.navMeshAgent;
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

    // Ends when enemyController decides it's ok to attack
    public bool ActionFinished(out EnemyBehaviorState nextAction) {
        nextAction = EnemyBehaviorState.ChasingPlayer;
        return false;
    }
    public bool CanActivate(Player player) {
        return true;
    }
}
