using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAffectorField))]
public class DamageFieldEffect : MonoBehaviour {
    public enum DamageEffectType {
        None, OnEnter, ApplyContinuously,
    }
    public DamageEffectType damageType;
    public float damage = 10f;

    void Awake() {
        var affectorField = Enforcements.GetComponent<PlayerAffectorField>(this);
        affectorField.OnPlayerEnter += player => {
            if (damageType == DamageEffectType.OnEnter) {
                player.TakeDamage(damage);
            }
        };
        affectorField.OnPlayerTick += player => {
            if (damageType == DamageEffectType.OnEnter) {
                player.TakeDamage(damage);
            }
        };
    }
}
