using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OpeningController : MonoBehaviour
{
    public Scene scene;

    public Button defaultButtom;

    public List<GameObject> ops;

    private int counter = 1;

    void Start()
    {
        foreach (GameObject op in ops)
        {
            op.SetActive(false);
        }
        defaultButtom.Select();
        ops[0].SetActive(true);
    }

    // basic navigating for different Menus 
    public void OnContinue()
    {
        if (counter >= ops.Count)
        {
            GameStart();
        }
        else
        {
            ops[counter - 1].SetActive(false);
            ops[counter].SetActive(true);
            counter++;
        }

    }

    public void GameStart()
    {
        Loader.Load(Loader.Scene.Level1);
    }
}
