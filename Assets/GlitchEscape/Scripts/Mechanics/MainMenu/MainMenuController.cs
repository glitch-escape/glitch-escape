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
    public void PlayTutorial () { Loader.Load(Loader.Scene.Tutorial_01_Movement); }
    public void PlayPlatformingLevel () { Loader.Load(Loader.Scene.Vertical_Platforming_Level); }
    public void PlayMainLevel () { Loader.Load(Loader.Scene.Opening); }
    public void PlayEndCutscene () { Loader.Load(Loader.Scene.End_Cutscene); }

    public void GameExit()
    {
        Application.Quit();
    }
}
