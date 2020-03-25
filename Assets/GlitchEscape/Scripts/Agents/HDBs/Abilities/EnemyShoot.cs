using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour, IEnemyAttackAction {
    private OldEnemy _oldEnemy;
    private OldEnemyController _oldEnemyController;
    private Player player;

    // Attack variables
    public float duration;
    public float cooldown, strikeDist;
    public OldEnemyProjectile bullPrefab;
    public int bulletAmt;
    public float bulletRate;

    // Other variables
    private float curAtkTime, curCooldwn;
    private int shotsMade;
    private bool isAttack =>
        curAtkTime < duration && curAtkTime > 0;

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
        curCooldwn = 0;
        shotsMade = 0;

        // Make the enemy stand still
        _oldEnemy.navMeshAgent.SetDestination(_oldEnemy.transform.position);
        _oldEnemy.animator.SetBool("isAttacking", true);
    }
    // Reset variables of the attack
    public void EndAction() {
        curCooldwn = Time.time;
        curAtkTime = 0;

        _oldEnemy.animator.SetBool("isAttacking", false);
    }
    // Update variables of the attack
    public void UpdateAction() {
        Vector3 dir = player.transform.position - _oldEnemy.transform.position;
        curAtkTime += Time.deltaTime;
        if (shotsMade < bulletAmt) {
            ShootBullet(dir, _oldEnemy.transform.position);
        }
    }

    // Informs if the attack has completed
    public bool ActionFinished(out EnemyBehaviorState nextAction) {
        if (!isAttack && shotsMade >= bulletAmt) {
            nextAction = EnemyBehaviorState.ChasingPlayer;
            EndAction();
            return true;
        }
        nextAction = EnemyBehaviorState.AttackingPlayer;
        return false;
    }

    // Determine if the attack can be preformed
    public bool CanActivate(Player player) {
        // Check if attack is in cooldown
        if (Time.time - curCooldwn < cooldown)
            return false;
        // Check if attack is in range
        Vector3 foePos = _oldEnemy.transform.position;
        Vector3 playPos = player.transform.position;
        if (Vector3.Distance(foePos, playPos) > strikeDist)
            return false;

        return true;
    }
    #endregion

    #region OtherFunctions
    // Remove cooldown on enable
    void OnEnable() {
        curCooldwn = -cooldown;
    }

    // Update the cooldown tracker
    public void UpdateCooldown() { curCooldwn -= Time.deltaTime; }

    // Returns true when attack is over
    public void ShootBullet(Vector3 direction, Vector3 origin) {
        if (shotsMade * bulletRate < curAtkTime) {
            shotsMade += 1;

            // Spawn bullet
            Quaternion rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            OldEnemyProjectile bullet = Instantiate(bullPrefab, origin, rotation);
            bullet.gameObject.SetActive(true);
            bullet.SetPlayerPos(player.transform);
        }
    }
    #endregion
}
