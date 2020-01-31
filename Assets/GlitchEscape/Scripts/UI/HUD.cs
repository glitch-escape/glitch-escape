using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class HUD : MonoBehaviour
{
    public PlayerManager playerControls;
    public Text countdownText;
    public GameObject globalPostProcess;
    public PostProcessProfile globalProfile;
    public PostProcessProfile pinkMazeProfile;

    public float timeLimit = 30f;

    private float counter = 0f;
    private float timeRemains;
    private bool timerOn;

    void Awake()
    {
        timerOn = false;
    }

    void Update()
    {
        Timer();
    }

    public void TimerReset() => counter = 0;

    public void Timer()
    {
        if (timerOn)
        {
            counter += 1 * Time.deltaTime;
            timeRemains = timeLimit - counter;

            if (timeRemains <= 0) {
                TimerReset();
                playerControls.Respawn();
            }

            float minRemains = Mathf.Floor(timeRemains / 60f);
            float secRemains = Mathf.Floor(timeRemains % 60f);

            countdownText.text = "Time Left: " + minRemains.ToString("00") + ":" + secRemains.ToString("00");
        }
        else
        {
            countdownText.text = "";

        }
    }

    public void ClearTimer() {
        timerOn = false; 
        TimerReset();
    }

    public void switchTimer()
    {
        timerOn = !timerOn;
        TimerReset();
        if (timerOn)
        {
            globalPostProcess.GetComponent<PostProcessVolume>().profile = pinkMazeProfile;
        }
        else
        {
            globalPostProcess.GetComponent<PostProcessVolume>().profile = globalProfile;
        }
    }
}


