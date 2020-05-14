using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndCardButton : MonoBehaviour
{
    public Button defaultButton;
    public bool canAdvanceToMainMenu = false;

    public void Activate() {
        canAdvanceToMainMenu = true;
    }
    void Start()
    {
        defaultButton.Select();
    }

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

    public void PlayMainMenu() {
        if (canAdvanceToMainMenu) {
            Loader.Load(Loader.Scene.MainMenu);
        }
    }
}
