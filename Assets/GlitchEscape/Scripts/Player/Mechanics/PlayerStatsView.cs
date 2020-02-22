using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PlayerStatsView : MonoBehaviour {
    private Player _player = null;
    private Player player => _player ?? Enforcements.GetSingleComponentInScene<Player>(this);

    private const int HEALTH_BAR_MATEIRAL_INDEX = 1;
    private const int STAMINA_BAR_MATERIAL_INDEX = 0;
    private const string MATERIAL_FILL_VAR = "FillPercent_974DFA7B";

    private struct AttribBarSetter {
        private float minFill, maxFill;
        private Material material;
        public AttribBarSetter(float minFill, float maxFill) {
            this.minFill = minFill;
            this.maxFill = maxFill;
            this.material = null;
        }
        public void SetMaterial(Material material) {
            this.material = material; 
        }
        public void Update(float value) {
            if (material == null) return;
            value = Mathf.Clamp01(value);
            value = minFill + (maxFill - minFill) * value;
            // Debug.Log("setting "+material+" fill to "+ value);
            material.SetFloat(MATERIAL_FILL_VAR, value);   
        }
    }
    private AttribBarSetter healthSetter = new AttribBarSetter(0.081f, 0.82f);
    private AttribBarSetter staminaSetter = new AttribBarSetter(0.079f, 0.592f);

    private void OnEnable() {
        var renderer = Enforcements.GetComponent<Renderer>(this);
        healthSetter.SetMaterial(renderer.materials[HEALTH_BAR_MATEIRAL_INDEX]);
        staminaSetter.SetMaterial(renderer.materials[STAMINA_BAR_MATERIAL_INDEX]);
    }
    private void OnDisable() {
        healthSetter.SetMaterial(null);
        staminaSetter.SetMaterial(null);
    }
    
    void Update() {
        float health = player.health / player.maxHealth;
        float stamina = player.stamina / player.maxStamina;
        healthSetter.Update(health);
        staminaSetter.Update(stamina);
    }
}
