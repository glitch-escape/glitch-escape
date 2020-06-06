using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : PlayerComponent {
    public Image staminaWheel;
    public List<Image> healthSplashes;

    private void OnEnable() {
        foreach (Image i in healthSplashes) {
            i.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        float health = player.health.value / player.health.maximum;
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
    }
}
