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

        float minRemains = Mathf.Floor(timeRemains / 60f);
        float secRemains = Mathf.Floor(timeRemains % 60f);

        countdownText.text = "Time Left: " + minRemains.ToString("00") + ":" + secRemains.ToString("00");

    }

}


