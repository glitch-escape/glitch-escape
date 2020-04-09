using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyIdle : MonoBehaviour, IEnemyIdleAction {
    private OldEnemy _oldEnemy;
    private OldEnemyController _oldEnemyController;
    private Player player;

    public bool doesPatrol;

    // Initialize component
    public void SetupControllerComponent(OldEnemyController controller) {
        // Get references
        _oldEnemyController = controller;
        _oldEnemy = controller.oldEnemy;
        player = controller.player;
    }

    #region IdleActionImplementation
    public void StartAction() {
        _oldEnemy.SetNavDest(_oldEnemy.transform.position);
    }

    public void EndAction() { }
    public void UpdateAction() {
        // Start chasing once player is detected
     //   if (_oldEnemyController.PlayerDetected())
     //       _oldEnemyController.OnPlayerDetected(player);
    }
    public bool ActionFinished(out OldEnemyBehaviorState nextAction) {
        nextAction = OldEnemyBehaviorState.SearchingForPlayer;
        return doesPatrol;
    }
    public bool CanActivate(Player player) {
        return true;
    }
    #endregion
}
