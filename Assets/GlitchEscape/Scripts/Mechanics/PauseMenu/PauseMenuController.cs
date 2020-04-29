using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject menus;
    public GameObject main;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider SFXSlider;
    public AudioMixer audioMixer;
    public Button mainResumeButton;

    void Awake()
    {
        ResetMenu();
        // PlayerPrefs.SetFloat("MasterVolume", 0f);
    }

    void Start()
    {
        LoadSettings();
    }

    public void OnPauseMenu()
    {
        if (!menus.activeInHierarchy)
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
            menus.SetActive(true);
            mainResumeButton.Select();
        }
        else
        {
            GameResume();
        }
    }

    // basic navigating for different Menus 
    public void MenuNav(Selectable seleted)
    {
        EventSystem.current.SetSelectedGameObject(null);
        // EventSystem.current.SetSelectedGameObject(seleted);
        // seletedButton.SetSelectedGameObject();
        seleted.Select();
    }

    public void MenuNavFrom(GameObject navFrom)
    {
        navFrom.SetActive(false);
    }

    public void MenuNavTo(GameObject navTo)
    {
        navTo.SetActive(true);
    }

    public void GameResume()
    {
        if (menus.activeInHierarchy)
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            menus.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            ResetMenu();
        }
    }

    public void GameRestart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        GameResume();
    }

    public void GameExit()
    {
        GameResume();
        Loader.Load(Loader.Scene.MainMenu);
    }

    void ResetMenu()
    {
        foreach (Transform child in menus.transform)
        {
            child.gameObject.SetActive(false);
        }
        main.SetActive(true);
        menus.SetActive(false);
    }

    public void LoadSettings()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        audioMixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        audioMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        audioMixer.SetFloat("Sound Effects", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume")) * 20);
    }

    public void ChangeMasterVol(float val)
    {
        PlayerPrefs.SetFloat("MasterVolume", val);
        audioMixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
    }

    public void ChangeMusicVol(float val)
    {
        PlayerPrefs.SetFloat("MusicVolume", val);
        audioMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
    }

    public void ChangeSFXVol(float val)
    {
        PlayerPrefs.SetFloat("SFXVolume", val);
        audioMixer.SetFloat("Sound Effects", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume")) * 20);
    }
}
