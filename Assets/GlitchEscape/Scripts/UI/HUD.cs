using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text countdownText;

    public float timeLimit = 120f;

    private float counter = 0f;
    private float timeRemains;

    [System.Obsolete]
    void Update()
    {
        counter += 1 * Time.deltaTime;
        timeRemains = timeLimit - counter;

        if (timeRemains <= 0)
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        float minRemains = timeRemains / 60;
        float secRemains = timeRemains % 60;

        countdownText.text = "Time Left: " + minRemains.ToString("0") + ":" + secRemains.ToString("0");

    }

}


