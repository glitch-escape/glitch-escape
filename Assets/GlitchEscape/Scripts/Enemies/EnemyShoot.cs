using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnemyShoot : MonoBehaviour, IEnemyAttackAction {
    private Enemy enemy;
    private EnemyController enemyController;
    private Player player;

    // Attack variables
    public float cooldown, strikeDist;
    public float yShift;
    public Projectile bullPrefab;
    public int bulletAmt;
    public float bulletRate;

    // Other variables
    private float curAtkTime, curCooldwn;
    private int shotsMade;

    // Initialize component
    public void SetupControllerComponent(EnemyController controller) {
        // Get references
        enemyController = controller;
        enemy = controller.enemy;
        player = controller.player;

        if (!bullPrefab) { Debug.LogError("Bullet prefab missing!"); }
    }

    #region AttackActionImplementation
    // Initialize variables of the attack
    public void StartAction() {
        curAtkTime = 0;
        curCooldwn = 0;
        shotsMade = 0;
    }
    // Reset variables of the attack
    public void EndAction() {
        curCooldwn = Time.time;
        curAtkTime = 0;
        
    }
    // Update variables of the attack
    public void UpdateAction() {
        Vector3 dir = player.transform.position - enemy.transform.position;
        ShootBullet(dir, enemy.transform.position);
    }

    // Informs if the attack has completed
    public bool ActionFinished(out EnemyBehaviorState nextAction) {
        if (shotsMade >= bulletAmt) {
            Debug.Log("ended");
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

        Debug.Log("asdsadas");
        return true;
    }
    #endregion

    #region OtherFunctions
    // Update the cooldown tracker
    public void UpdateCooldown() { curCooldwn -= Time.deltaTime; }

    // Returns true when attack is over
    public void ShootBullet(Vector3 direction, Vector3 origin) {
        curAtkTime += Time.deltaTime;
        if (shotsMade * bulletRate < curAtkTime) {
            shotsMade += 1;

            // Spawn bullet
            origin.y += yShift;
            Projectile bullet = Instantiate(bullPrefab, origin, Quaternion.identity);
            bullet.gameObject.SetActive(true);
            bullet.SetDirection(direction);

        }
    }
    #endregion
}
