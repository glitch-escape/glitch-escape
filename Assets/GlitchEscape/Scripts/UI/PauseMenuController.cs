using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    // input instance singleton
    // public Input input => m_input ?? (m_input = new Input());
    // private Input m_input;
    // private Vector2 uiInput => input.UI.Navigate.ReadValue<Vector2>();

    void Awake()
    {
        pauseMenu.SetActive(false);
        // input.Enable();
    }

    // void FixedUpdate()
    // {
        // if (uiInput.magnitude > 1e-6)
        // {
        //     Debug.Log(uiInput.x + uiInput.y);
        // }
    // }

    public void OnPauseMenu()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        pauseMenu.SetActive(true);
    }

    public void GameResume()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
    }
}
