using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject menus;
    public GameObject main;

    public Button mainResumeButtom;

    void Awake()
    {
        ResetMenu();
    }

    public void OnPauseMenu()
    {
        if (!menus.activeInHierarchy)
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
            menus.SetActive(true);
            mainResumeButtom.Select();
        }
        else
        {
            GameResume();
        }
    }

    // basic navigating for different Menus 
    public void MenuNav(Button seletedButton)
    {
        EventSystem.current.SetSelectedGameObject(null);
        seletedButton.Select();
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
}
