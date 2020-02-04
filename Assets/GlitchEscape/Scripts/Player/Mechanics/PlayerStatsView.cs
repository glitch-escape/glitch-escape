using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerStatsView : MonoBehaviour, IPlayerControllerComponent {
    private Player player;
    private Camera mainCamera;
    public GameObject healthBar;
    public GameObject staminaBar;
    private GameObject foregroundHealthBar;
    private GameObject foregroundStaminaBar;
    private Material activeHealthBarMaterial;
    private Material activeStaminaBarMaterial;
    private Color baseHealthBarColor;
    private Color baseStaminaBarColor;
    public Color healthFlashColor = Color.red;
    public Color staminaFlashColor = Color.white;

    public bool hitPlayer = false;
    public bool useAbility = false;

    public float currentHealth = 0f;

    public void SetupControllerComponent(PlayerController controller) {
        player = controller.player;
        mainCamera = controller.camera;
        foregroundHealthBar = Instantiate(healthBar, healthBar.transform.parent);
        foregroundStaminaBar = GameObject.Instantiate(staminaBar, staminaBar.transform.parent);
        healthBar.SetActive(false);
        staminaBar.SetActive(false);

        baseHealthBarColor = healthBar.GetComponent<Renderer>().material.color;
        baseStaminaBarColor = staminaBar.GetComponent<Renderer>().material.color;
        activeHealthBarMaterial = foregroundHealthBar.GetComponent<Renderer>().material;
        activeStaminaBarMaterial = foregroundStaminaBar.GetComponent<Renderer>().material;
    }
    private void UpdateHealthBar(GameObject target, GameObject initial, float value) {
        var scale = target.transform.localScale;
        var s0 = initial.transform.localScale;
        scale.Set(s0.x * value, s0.y, s0.z);
        target.transform.localScale = scale;

        var pos = target.transform.localPosition;
        pos.x = initial.transform.localPosition.x - scale.x / 2 + s0.x / 2;
        target.transform.localPosition = pos;
    }
    void Update() {
        float health = player.health / player.maxHealth;
        float stamina = player.stamina / player.maxStamina;

        currentHealth = health;
        
        UpdateHealthBar(foregroundHealthBar, healthBar, health);
        UpdateHealthBar(foregroundStaminaBar, staminaBar, stamina);

        activeHealthBarMaterial.color = Color.Lerp(baseHealthBarColor, healthFlashColor, player.isHealthFlashing ? 1f : 0f);
        activeStaminaBarMaterial.color =
            Color.Lerp(baseStaminaBarColor, staminaFlashColor, player.isStaminaFlashing ? 1f : 0f);
        
        if (hitPlayer) {
            hitPlayer = false; 
            player.TakeDamage(15f);
        }
        if (useAbility) {
            useAbility = false;
            player.TryUseAbility(25f);
        }
    }
}
