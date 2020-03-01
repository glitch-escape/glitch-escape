using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button defaultButtom;

    void Start()
    {
        defaultButtom.Select();
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

    public void GameStart()
    {
        Loader.Load(Loader.Scene.Opening);
    }

    public void GameExit()
    {
        Application.Quit();
    }


}
