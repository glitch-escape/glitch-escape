using UnityEngine;

public class SystemManager : MonoBehaviour
{
    // Public Variables:
    [Header("Objects")]
    public GameObject player;
    public GameObject ui;

    [System.NonSerialized] public PlayerManager playerControls;
    [System.NonSerialized] public HUDManager hudManager;

    void Awake()
    {
        // Get Scripts
        playerControls = player.GetComponent<PlayerManager>();
        hudManager = ui.GetComponent<HUDManager>();
    }
}
