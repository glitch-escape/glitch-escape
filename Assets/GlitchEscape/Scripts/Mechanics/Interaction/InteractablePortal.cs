using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider))]
public class InteractablePortal : AInteractiveObject
{
    public bool disableOnStart = false;
    // singleton
    private static InteractablePortal _instance = null;
    public static InteractablePortal instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<InteractablePortal>();
                if (_instance == null)
                    Debug.LogError("No InteractablePortal instance in this scene!");
            }
            return _instance;
        }
    }

    public Transform floatTextArea;
    public string interactMessage = "[Step through the portal]";
    public PlayableDirector portalCutscene;
    [InjectComponent] public FloatingTextController floatingText;
    public InteractableTank interactableTank;
    public Loader.Scene levelToLoad = Loader.Scene.MainMenu;

    void Awake()
    {
        floatingText = FloatingTextController.instance;
        portalCutscene = GetComponent<PlayableDirector>();
        portalCutscene?.Play();
        portalCutscene?.Pause();
    }
    void Start()
    {
        if (disableOnStart) {
            this.transform.parent.gameObject.SetActive(false);
        }
    }

    public override void OnInteract(Player player) {
        if (levelToLoad != Loader.Scene.None) {
            Application.LoadLevel(levelToLoad.ToString());
        }
    }

    public override void OnFocusChanged(bool focused) {
        if (focused) {
            floatingText.EnableText(floatTextArea, interactMessage);
        } else {
            floatingText.DisableText(floatTextArea);
        }
    }
    public void OpenPortal()
    {
        portalCutscene?.Resume();
        this.gameObject.SetActive(true);
    }
}
