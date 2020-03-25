using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class EnemyAttack : MonoBehaviour, IEnemyAttackAction {
    private OldEnemy _oldEnemy;
    private OldEnemyController _oldEnemyController;
    private BoxCollider hitbox;
    private MeshRenderer mesh;

    // Attack variables 
    public float duration, cooldown;
    public float firstActive, activeLen; // Determine interval attack is active
    public float damage, strikeDist;
    public float angleRange;

    // Other variables
    private float atkStart, curCooldwn;
    private bool hasHit;
    private bool isAttack =>
        Time.time - atkStart <= duration &&
        atkStart > 0;
    private bool isActive =>
        isAttack && 
        Time.time - atkStart >= firstActive &&
        Time.time - atkStart <= firstActive + activeLen;

    // Initialize component
    public void SetupControllerComponent(OldEnemyController controller) {
        // Get references
        _oldEnemyController = controller;
        _oldEnemy = controller.oldEnemy;
        hitbox = GetComponent<BoxCollider>()
                   ?? gameObject.AddComponent<BoxCollider>();
        hitbox.isTrigger = true;
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
    }

    #region AttackActionImplementation
    // Initialize variables of the attack
    public void StartAction() {
        atkStart = Time.time;
        curCooldwn = 0;
    }
    // Reset variables of the attack
    public void EndAction() {
        atkStart = 0;
        curCooldwn = Time.time;
        hasHit = false;
        mesh.enabled = false;
    }
    // Update variables of the attack
    public void UpdateAction() {
        if (isActive)   { mesh.enabled = true; }
        else            { mesh.enabled = false; }
    }

    // Informs if the attack has completed
    public bool ActionFinished(out EnemyBehaviorState nextAction) {
        if (!isAttack) {
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

        // Make sure enemy is facing player
        Vector3 playerDir = playPos - foePos;
        if (Vector3.Angle(playerDir, _oldEnemy.transform.forward) > angleRange) {
            Debug.Log(Vector3.Angle(playerDir, _oldEnemy.transform.forward));
            return false;
        }
           

        return true;
    }
    #endregion

    #region OtherFunctions
    // Remove cooldown on enable
    void OnEnable() {
        curCooldwn = -cooldown;
    }

    // See if the player was hit
    void OnTriggerStay(Collider other) {
        if (isActive && !hasHit) {
            var player = other.GetComponent<Player>();
            if (player != null) {
                player.TakeDamage(damage);
                hasHit = true;
            }
        }
    }
    #endregion
}
