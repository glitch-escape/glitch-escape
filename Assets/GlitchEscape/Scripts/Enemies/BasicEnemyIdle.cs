using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyIdle : MonoBehaviour, IEnemyIdleAction {
    private Enemy enemy;
    private EnemyController enemyController;
    private Player player;

    // Initialize component
    public void SetupControllerComponent(EnemyController controller) {
        // Get references
        enemyController = controller;
        enemy = controller.enemy;
        player = controller.player;
    }

    #region IdleActionImplementation
    public void StartAction() {
        enemy.SetNavDest(enemy.transform.position);
    }

    public void EndAction() { }
    public void UpdateAction() {
        // Start chasing once player is detected
     //   if (enemyController.PlayerDetected())
     //       enemyController.OnPlayerDetected(player);
    }
    public bool ActionFinished(out EnemyBehaviorState nextAction) {
        nextAction = EnemyBehaviorState.Idle;
        return false;
    }
    public bool CanActivate(Player player) {
        return true;
    }
    #endregion
}
