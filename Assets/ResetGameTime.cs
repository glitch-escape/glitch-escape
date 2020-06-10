﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        foreach (PlayerController thePlayer in FindObjectsOfType<PlayerController>())
        {
            Destroy(thePlayer.gameObject);
        }
    }
}
