using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAffectorField))]
public class DamageFieldEffect : MonoBehaviour, IFieldEffect {
    public enum DamageEffectType {
        None, OnEnter, ApplyContinuously,
    }
    public DamageEffectType damageType;
    public float damage = 10f;
    public void SetupField(PlayerAffectorField effectorField) {}
    public void OnPlayerEnter(Player player) {
        if (damageType == DamageEffectType.OnEnter) {
            player.TakeDamage(damage);
        }
    }
    public void OnPlayerExit(Player player) {}
    public void OnPlayerTick(Player player) {
        if (damageType == DamageEffectType.ApplyContinuously) {
            player.TakeDamage(damage * Time.deltaTime);
        }
    }
}
