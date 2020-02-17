using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button resumeButtom;

    void Awake()
    {
        pauseMenu.SetActive(false);
    }

    public void OnPauseMenu()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
            pauseMenu.SetActive(true);
            resumeButtom.Select();
        }
    }

    public void GameResume()
    {
        if (pauseMenu.activeInHierarchy)
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            pauseMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
