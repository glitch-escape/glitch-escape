using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))] 
[RequireComponent(typeof(MeshRenderer))]
public class Attack : MonoBehaviour {
    
    public float duration, distance;
    public float damage;

    private float curAtkTime;
    private bool isActive;

    // Start is called before the first frame update
    void Start() {
        ResetAttack();
    }

    // Update is called once per frame
    void Update() {
        if(isActive){
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
    public bool GetActive() { 
        return isActive; 
    }

}
