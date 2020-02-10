using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))] 
[RequireComponent(typeof(MeshRenderer))]
public class Attack : MonoBehaviour {

    public enum AttackType { STRIKE, CHARGE, BULLET }
    public AttackType atkType;

    // General variables
    public float duration, distance;
    public float damage;

    [Header("Charge + Retreat variables")]
    public float chargeDist;
    public float retreatDist;

    [Header("Bullet attack variables")]
    public Bullet bullPrefab;
    public int bulletAmt;
    public float bulletRate;
    private int shotsMade;

    private float curAtkTime;
    private bool isActive;

    // Start is called before the first frame update
    void Start() {
        ResetAttack();

        // Put this in a different script later when enemyController is done
        if (!bullPrefab && atkType == AttackType.BULLET){
            Debug.LogError("Bullet prefab missing!");
        }
    }

    // Update is called once per frame
    void Update() {
        if(isActive && atkType == AttackType.STRIKE){
            if(curAtkTime <= 0)    { ResetAttack(); }
            else                   { curAtkTime -= Time.deltaTime; }
        }
    }

    void OnTriggerEnter (Collider other) { 
        var player = other.GetComponent<Player>(); 
        if (player != null) { player.TakeDamage(damage); } 
    }

    private void ResetAttack() {
        curAtkTime = duration; 
        isActive = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    // Function to be called by other scripts
    public void StartAttack() { 
        isActive = true; 
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
    public bool GetActive() { return isActive; }

    // Functions for strike type
    public bool IsStrike() { return atkType == AttackType.STRIKE; }

    // Functions for charge type
    public bool IsCharge() { return atkType == AttackType.CHARGE; }
    public float GetChargeDist()  { return chargeDist; }
    public float GetRetreatDist() { return retreatDist; }

    // Functions for bullet type
    public bool IsBullet() { return atkType == AttackType.BULLET; }
    // Returns true when attack is over
    public bool ShootBullets(Vector3 direction, Vector3 origin) {
        curAtkTime += Time.deltaTime;
        if (shotsMade * bulletRate < curAtkTime) {
            shotsMade += 1;

            // Spawn bullet
            Bullet bullet = Instantiate(bullPrefab, origin, Quaternion.identity);
            bullet.gameObject.SetActive(true);
            bullet.SetDirection(direction);

        }
        if (shotsMade >= bulletAmt)
            return true;
        return false;
    }


}
