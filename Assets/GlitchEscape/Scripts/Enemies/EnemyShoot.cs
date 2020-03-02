using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour, IEnemyAttackAction {
    private Enemy enemy;
    private EnemyController enemyController;
    private Player player;

    // Attack variables
    public float duration;
    public float cooldown, strikeDist;
    public float projYShift;
    public Projectile bullPrefab;
    public int bulletAmt;
    public float bulletRate;

    // Other variables
    private float curAtkTime, curCooldwn;
    private int shotsMade;
    private bool isAttack =>
        curAtkTime < duration && curAtkTime > 0;

    // Initialize component
    public void SetupControllerComponent(EnemyController controller) {
        // Get references
        enemyController = controller;
        enemy = enemyController.enemy;
        player = enemyController.player;

        if (!bullPrefab) { Debug.LogError("Bullet prefab missing!"); }
    }

    #region AttackActionImplementation
    // Initialize variables of the attack
    public void StartAction() {
        curAtkTime = 0;
        curCooldwn = 0;
        shotsMade = 0;

        // Make the enemy stand still
        enemy.navMeshAgent.SetDestination(enemy.transform.position);
    }
    // Reset variables of the attack
    public void EndAction() {
        curCooldwn = Time.time;
        curAtkTime = 0;
    }
    // Update variables of the attack
    public void UpdateAction() {
        Vector3 dir = player.transform.position - enemy.transform.position;
        curAtkTime += Time.deltaTime;
        if (shotsMade < bulletAmt) {
            ShootBullet(dir, enemy.transform.position);
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
        Vector3 foePos = enemy.transform.position;
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
            origin.y += projYShift;
            Quaternion quat = Quaternion.LookRotation(direction, Vector3.up);
            Projectile bullet = Instantiate(bullPrefab, origin, quat);
            bullet.gameObject.SetActive(true);
            bullet.SetPlayerPos(player.transform);
        }
    }
    #endregion
}
