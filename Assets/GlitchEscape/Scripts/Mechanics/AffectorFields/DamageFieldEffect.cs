using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAffectorField))]
public class DamageFieldEffect : MonoBehaviour {
    public enum DamageEffectType {
        None,
        OnEnter,
        ApplyContinuously,
    }

    public DamageEffectType damageType = DamageEffectType.ApplyContinuously;
    public float damage = 10f;

    private PlayerAffectorField _affectorField = null;
    private PlayerAffectorField affectorField => 
        _affectorField ?? Enforcements.GetComponent<PlayerAffectorField>(this);
    void OnEnable() {
        var affectorField = this.affectorField;
        
        affectorField.OnPlayerEnter += OnPlayerEnter;
        affectorField.OnPlayerTick += OnPlayerTick;
    }
    void OnDisable() {
        var affectorField = this.affectorField;
        affectorField.OnPlayerEnter -= OnPlayerEnter;
        affectorField.OnPlayerTick -= OnPlayerTick;
    }
    void OnPlayerEnter(Player player) {
        if (damageType == DamageEffectType.OnEnter) {
            player.TakeDamage(damage);
        }
    }
    void OnPlayerTick(Player player) {
        if (damageType == DamageEffectType.ApplyContinuously) {
            player.TakeDamage(damage * Time.deltaTime);
        }
    }
}
