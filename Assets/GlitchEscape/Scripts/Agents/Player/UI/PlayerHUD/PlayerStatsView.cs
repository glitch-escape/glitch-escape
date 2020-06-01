using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Renderer))]
public class PlayerStatsView : MonoBehaviour {
    public static PlayerStatsView instance = null;
    void Awake() { instance = this; }
    
    private Player _player = null;
    private Player player => _player ?? Enforcements.GetSingleComponentInScene<Player>(this);

    private const int HEALTH_BAR_MATEIRAL_INDEX = 1;
    private const int STAMINA_BAR_MATERIAL_INDEX = 0;
    //private const int SHARD_BAR_MATERIAL_INDEX = 2;
    private const string MATERIAL_FILL_VAR = "FillPercent_974DFA7B";
    private const string MATERIAL_FLASH_PERIOD = "FlashPeriod_5B68FE7D";
    private const string MATERIAL_FLASH_START_TIME = "FlashStartTime_A448C5BD";
    private const string MATERIAL_FLASH_COLOR = "ForegroundFlashColor_B2CD22BE";
    private const string MATERIAL_FLASHING = "FlashForeground_B02CF416";
    private const string MATERIAL_BG_FLASH_COLOR = "BackgroundFlashColor_6861E569";
    private const string MATERIAL_BG_FLASHING = "FlashBackground_68DCEC6F";
    
    struct FlashInfo {
        private bool isFlashing;
        private float startTime;
        private float duration;
        private Material material;
        public void SetMaterial(Material material) {
            this.material = material; 
        }
        public void SetFlashing(float duration, float period, Color color) {
            if (material != null) {
                material.SetFloat(MATERIAL_FLASH_START_TIME, Time.time);
                material.SetFloat(MATERIAL_FLASH_PERIOD, period);
                material.SetColor(MATERIAL_FLASH_COLOR, color);
                material.SetInt(MATERIAL_FLASHING, 1);
                this.startTime = Time.time;
                this.duration = duration;
                this.isFlashing = true;
            }
        }
        public void Update() {
            if (isFlashing && Time.time > startTime + duration) {
                isFlashing = false;
                material.SetInt(MATERIAL_FLASHING, 0);
            }   
        }
    }

    public Color lowStaminaFlashColor;
    public float flashStaminaTime = 1.5f;
    public float flashStaminaPeriod = 0.7f;
    private FlashInfo staminaFlash = new FlashInfo();
    public void FlashLowStamina() {
        staminaFlash.SetFlashing(flashStaminaTime, flashStaminaPeriod, lowStaminaFlashColor);
    }

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
    //private AttribBarSetter shardSetter = new AttribBarSetter(0.079f, 0.592f);

    public Image staminaWheel;
    public List<Image> healthSplashes;

    private void OnEnable() {
        var renderer = Enforcements.GetComponent<Renderer>(this);
        healthSetter.SetMaterial(renderer.materials[HEALTH_BAR_MATEIRAL_INDEX]);
        staminaSetter.SetMaterial(renderer.materials[STAMINA_BAR_MATERIAL_INDEX]);
        //shardSetter.SetMaterial(renderer.materials[SHARD_BAR_MATERIAL_INDEX]);
        staminaFlash.SetMaterial(renderer.materials[STAMINA_BAR_MATERIAL_INDEX]);
        player.OnFailedToUseAbilityDueToLowStamina += FlashLowStamina;
        foreach (Image i in healthSplashes) {
            i.gameObject.SetActive(false);
        }
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    private void OnDisable() {
        healthSetter.SetMaterial(null);
        staminaSetter.SetMaterial(null);
        //shardSetter.SetMaterial(null);
        staminaFlash.SetMaterial(null);
        player.OnFailedToUseAbilityDueToLowStamina -= FlashLowStamina;
    }

    private List<Material> astralPlatforms;
    private GameObject glitchMaze;
    private bool hasGlitchMaze = false;
    private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        GlitchPlatform[] temp = Resources.FindObjectsOfTypeAll<GlitchPlatform>();
        if (temp.Length >= 1)
        {
            glitchMaze = temp[0].gameObject;
            hasGlitchMaze = true;
            astralPlatforms = new List<Material>();
            foreach (Transform platform in glitchMaze.transform)
            {
                astralPlatforms.Add(platform.GetComponent<Renderer>().material);
            }
        }
    }
    
    void Update() {
        float health = player.health.value / player.health.maximum;
        if(hasGlitchMaze)
        {
            foreach (Material m in astralPlatforms)
            {
                m.SetFloat("Vector1_62D5110A", health);
            }
        }

        if(health < .45 && health > .30)
        {
            healthSplashes[0].gameObject.SetActive(true);
            healthSplashes[1].gameObject.SetActive(false);
            healthSplashes[2].gameObject.SetActive(false);
        }
        else if (health < .30 && health > .15)
        {
            healthSplashes[1].gameObject.SetActive(true);
            healthSplashes[2].gameObject.SetActive(false);
        }
        else if (health < .15)
        {
            healthSplashes[2].gameObject.SetActive(true);
        }
        else
        {
            healthSplashes[0].gameObject.SetActive(false);
            healthSplashes[1].gameObject.SetActive(false);
            healthSplashes[2].gameObject.SetActive(false);
        }
        float stamina = player.stamina.value / player.stamina.maximum;
        if(stamina < 1f)
        {
            if (!staminaWheel.gameObject.activeInHierarchy)
            {
                staminaWheel.gameObject.SetActive(true);
            }
            staminaWheel.fillAmount = stamina;
        }
        else
        {
            if (staminaWheel.gameObject.activeInHierarchy)
            {
                staminaWheel.gameObject.SetActive(false);
            }
        }
        healthSetter.Update(health);
        staminaSetter.Update(stamina);
        staminaFlash.Update();
    }
}
