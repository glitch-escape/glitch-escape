using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(InteractionTrigger))]
public class HubPortal : MonoBehaviour, IPlayerInteractable {
    public bool disableOnStart = true;
    // singleton
    private static HubPortal _instance = null;
    public static HubPortal instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<HubPortal>();
                if (_instance == null)
                    Debug.LogError("No InteractablePortal instance in this scene!");
            }
            return _instance;
        }
    }

    public Transform floatTextArea;
    public string interactMessage = "[Step through the portal]";

    public PlayableDirector portalCutscene;
    private FloatingTextController floatingText;
    
    public Loader.Scene levelToLoad = Loader.Scene.MainMenu;

    void Awake()
    {
        floatingText = FloatingTextController.instance;
        // portalCutscene = GetComponent<PlayableDirector>();
        portalCutscene.Play();
        portalCutscene.Pause();
    }
    void Start()
    {
        if (disableOnStart) {
            this.transform.parent.gameObject.SetActive(false);
        }
        OpenPortal();
    }

    [Obsolete]
    public void OnInteract(Player player)
    {
        Application.LoadLevel(levelToLoad.ToString());
    }

    public void OnPlayerEnterInteractionRadius(Player player)
    {
        floatingText.EnableText(floatTextArea, interactMessage);
    }

    public void OnPlayerExitInteractionRadius(Player player)
    {
        floatingText.DisableText(floatTextArea);
    }

    public void OpenPortal()
    {
        portalCutscene.Resume();
        // this.transform.parent.gameObject.SetActive(true);
    }
}