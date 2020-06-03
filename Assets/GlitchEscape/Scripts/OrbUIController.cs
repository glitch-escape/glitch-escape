using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OrbUIController : PlayerComponent {
    [InjectComponent] public Animation orbGetAnim;
    private bool animPlayed = false;
    [InjectComponent] public TextMeshProUGUI returnToPortalPrompt;

    private void UpdateFragmentUI(Virtue activeVirtue, float fragmentCompletion) {
        int fragmentsPickedUp = (int) (fragmentCompletion * 7f);
        // TODO: init / update fragment UI here
    }
    private void OnEnable() {
        player.fragments.onActiveVirtueChanged += OnVirtueTypeChanged;
        player.fragments.onFragmentPickedUp += OnFragmentPickedUp;
        var activeVirtue = player.fragments.activeVirtueInThisScene;
        UpdateFragmentUI(activeVirtue, player.fragments.GetFragmentCompletion(activeVirtue));
        SceneManager.sceneLoaded += OnLevelLoaded;
    }
    private void OnDisable() {
        player.fragments.onActiveVirtueChanged -= OnVirtueTypeChanged;
        player.fragments.onFragmentPickedUp -= OnFragmentPickedUp;
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    void OnVirtueTypeChanged(Virtue virtue) {
        Debug.Log("Changed active virtue for fragment pickups to " + virtue);
        UpdateFragmentUI(virtue, player.fragments.GetFragmentCompletion(virtue));
    }
    void OnFragmentPickedUp(PlayerVirtueFragments.FragmentInfo fragment) {
        Debug.Log("Picked up fragment for " + fragment.virtue);
        UpdateFragmentUI(Virtue.Courage, player.fragments.GetFragmentCompletion(fragment.virtue));
        
        /// TODO: can play fragment pickup animation / etc here
    }
    void OnVirtueCompleted(Virtue virtue) {
        Debug.Log("Picked up all fragments for " + virtue + "!");
        
        /// TODO: can play virtue completion animation / etc here
        
        // play orb animation...?
        orbGetAnim?.Play();

        // activate portal, if portal not currently activated
        var portal = GameObject.FindObjectOfType<Portal>();
        if (portal != null && !portal.active) {
            portal.active = true;
        }
        returnToPortalPrompt.gameObject.SetActive(true);
    }
    void OnLevelLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        returnToPortalPrompt.gameObject.SetActive(false);
    }
}
