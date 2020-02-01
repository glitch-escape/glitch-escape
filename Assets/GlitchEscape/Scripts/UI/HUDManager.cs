using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class HUDManager : MonoBehaviour
{
    // Public Variables:
    [Header("Objects")]
    public GameObject system;
    public GameObject controlTips;
    public Text countdownText;
    public GameObject globalPostProcess;
    public PostProcessProfile globalProfile;
    public PostProcessProfile pinkMazeProfile;

    [Header("Variables")]
    public float timeLimit = 30f;

    // Functional Scripts:
    [System.NonSerialized] public SystemManager systemManager;
    [System.NonSerialized] public HUDTimer hudTimer;

    void Awake()
    {
        // Get Scripts
        systemManager = system.GetComponent<SystemManager>();
        hudTimer = GetComponent<HUDTimer>();
    }

    void Update()
    {
        hudTimer.Timer();
    }
}
