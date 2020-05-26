using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{
    public Button defaultButtom;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sFXSlider;
    public Slider cameraSensitiveSlider;
    public Slider fOVSlider;
    public Toggle invertYToggle;
    public AudioMixer audioMixer;
    private int isInvert = 0;

    void Start()
    {
        defaultButtom.Select();
    }

    // basic navigating for different Menus 
    public void MenuNav(Selectable seleted)
    {
        EventSystem.current.SetSelectedGameObject(null);
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
    public void PlayTutorial() { Loader.Load(Loader.Scene.Tutorial_01_Movement); }
    public void PlayPlatformingLevel() { Loader.Load(Loader.Scene.Vertical_Platforming_Level); }
    public void PlayMainLevel() { Loader.Load(Loader.Scene.Opening); }
    public void PlayEndCutscene() { Loader.Load(Loader.Scene.End_Cutscene); }

    public void GameExit()
    {
        Application.Quit();
    }

    public void LoadSettings()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        audioMixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        audioMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
        sFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        audioMixer.SetFloat("Sound Effects", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume")) * 20);
        cameraSensitiveSlider.value = PlayerPrefs.GetFloat("CameraSensitive", 1.0f);
        // playerConfig.cameraTurnSpeed = Mathf.Lerp(5f, 360f, cameraSensitiveSlider.value);
        isInvert = PlayerPrefs.GetInt("InvertY", 0);
        if (isInvert == 1)
        {
            invertYToggle.isOn = true;
        }
        else
        {
            invertYToggle.isOn = false;
        }
        fOVSlider.value = PlayerPrefs.GetFloat("FOV", 1f);
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

    public void ChangeCameraSensitive(float val)
    {
        PlayerPrefs.SetFloat("CameraSensitive", val);
        // playerConfig.cameraTurnSpeed = Mathf.Lerp(5f, 360f, val);
        // Debug.Log(playerConfig.cameraTurnSpeed);
    }

    public void ChangeInvertY(bool isInvert)
    {
        if (isInvert)
        {
            PlayerPrefs.SetInt("InvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("InvertY", 0);
        }
    }

    public void ChangeFOV(float val)
    {
        PlayerPrefs.SetFloat("FOV", val);
    }
}
