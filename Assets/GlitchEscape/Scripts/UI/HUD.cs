using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public PlayerControls playerControls;
    public Text countdownText;

    public float timeLimit = 120f;

    private float counter = 0f;
    private float timeRemains;

    void Update()
    {
        counter += 1 * Time.deltaTime;
        timeRemains = timeLimit - counter;

        if (timeRemains <= 0)
        {
            // Application.LoadLevel(Application.loadedLevel);
            playerControls.Respawn();
            counter = 0;
        }

        float minRemains = Mathf.Floor(timeRemains / 60f);
        float secRemains = Mathf.Floor(timeRemains % 60f);

        countdownText.text = "Time Left: " + minRemains.ToString("00") + ":" + secRemains.ToString("00");

    }

}


