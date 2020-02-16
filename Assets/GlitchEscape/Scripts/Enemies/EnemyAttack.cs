using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnemyAttack : MonoBehaviour, IEnemyAttackAction {
    private Enemy enemy;
    private EnemyController enemyController;
    private BoxCollider hitbox;

    // Attack variables
    public float duration, cooldown;
    public float damage, strikeDist;

    // Other variables
    private float curAtkTime, curCooldwn;

    // Initialize component
    public void SetupControllerComponent(EnemyController controller) {
        // Get references
        enemyController = controller;
        enemy = controller.enemy;
        hitbox = GetComponent<BoxCollider>()
                   ?? gameObject.AddComponent<BoxCollider>();
        hitbox.isTrigger = true;
    }

    #region AttackActionImplementation
    // Initialize variables of the attack
    public void StartAction() {
        curAtkTime = duration;
        curCooldwn = 0;
    }
    // Reset variables of the attack
    public void EndAction() { StartAction(); }
    // Update variables of the attack
    public void UpdateAction() { curAtkTime -= Time.deltaTime; }

    // Informs if the attack has completed
    public bool ActionFinished(out EnemyBehaviorState nextAction) {
        if (curAtkTime <= 0) {
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
        if (curCooldwn > 0)
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
    // Update the cooldown tracker
    public void UpdateCooldown() { curCooldwn -= Time.deltaTime; }

    // See if the player was hit
    void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<Player>();
        if (player != null) { player.TakeDamage(damage); }
    }
    #endregion
}
