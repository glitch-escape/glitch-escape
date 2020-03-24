using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryCanonAttack : MonoBehaviour, IEnemyAttackAction {
    private Enemy _oldEnemy;
    private OldEnemyController _oldEnemyController;
    private Player player;

    // Attack variables
    public float projYShift;
    public OldEnemyProjectile bullPrefab;
    public float bulletRate;

    // Other variables
    private float curAtkTime;

    // Initialize component
    public void SetupControllerComponent(OldEnemyController controller) {
        // Get references
        _oldEnemyController = controller;
        _oldEnemy = _oldEnemyController.oldEnemy;
        player = _oldEnemyController.player;

        if (!bullPrefab) { Debug.LogError("Bullet prefab missing!"); }
    }

    #region AttackActionImplementation
    // Initialize variables of the attack
    public void StartAction() {
        curAtkTime = 0;

        // Make the enemy stand still
        _oldEnemy.navMeshAgent.SetDestination(_oldEnemy.transform.position);
    }
    // Reset variables of the attack
    public void EndAction() { }
    // Update variables of the attack
    public void UpdateAction() {
        curAtkTime += Time.deltaTime;
        ShootBullet(_oldEnemy.transform.rotation, _oldEnemy.transform.position);
    }

    // Informs if the attack has completed
    public bool ActionFinished(out EnemyBehaviorState nextAction) {
        nextAction = EnemyBehaviorState.AttackingPlayer;
        return false;
    }

    // Determine if the attack can be preformed
    public bool CanActivate(Player player) { return true; }
    #endregion

    #region OtherFunctions
    // Returns true when attack is over
    public void ShootBullet(Quaternion forward, Vector3 origin) {
        if (bulletRate < curAtkTime) {
            curAtkTime = 0;

            // Spawn bullet
            origin.y += projYShift;
            OldEnemyProjectile bullet = Instantiate(bullPrefab, origin, forward);
            bullet.gameObject.SetActive(true);
            bullet.SetPlayerPos(player.transform);
        }
    }
    #endregion
}
