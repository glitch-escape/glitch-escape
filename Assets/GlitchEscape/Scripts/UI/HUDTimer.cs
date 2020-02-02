using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HUDTimer : MonoBehaviour
{
    private HUDManager hudManager;
    private float counter = 0f;
    private float timeRemains;
    private bool timerOn = false;

    void Awake()
    {
        hudManager = GetComponent<HUDManager>();
    }

    public void Timer()
    {
        if (timerOn)
        {
            counter += 1 * Time.deltaTime;
            timeRemains = hudManager.timeLimit - counter;

            if (timeRemains <= 0)
            {
                TimerReset();
            }

            float minRemains = Mathf.Floor(timeRemains / 60f);
            float secRemains = Mathf.Floor(timeRemains % 60f);

            hudManager.countdownText.text = "Time Left: " + minRemains.ToString("00") + ":" + secRemains.ToString("00");
        }
        else
        {
            hudManager.countdownText.text = "";
        }
    }

    public void TimerReset() => counter = 0;
    // public void ClearTimer()
    // {
    //     timerOn = false;
    //     TimerReset();
    // }

    public void switchTimer()
    {
        timerOn = !timerOn;
        TimerReset();
        if (timerOn)
        {
            hudManager.globalPostProcess.GetComponent<PostProcessVolume>().profile = hudManager.pinkMazeProfile;
        }
        else
        {
            hudManager.globalPostProcess.GetComponent<PostProcessVolume>().profile = hudManager.globalProfile;
        }
    }
}
