using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ZoomToTrigger : MonoBehaviour
{

    public PlayableDirector zoomToCutscene;

    void OnEnable()
    {
        zoomToCutscene.stopped += OnPlayableDirectorStopped;
    }

    void Awake()
    {
        // zoomToCutscene = GetComponent<PlayableDirector>();
        zoomToCutscene.Play();
        zoomToCutscene.Pause();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter");
        zoomToCutscene.Resume();
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (zoomToCutscene == aDirector)
            this.transform.parent.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        zoomToCutscene.stopped -= OnPlayableDirectorStopped;
    }
}
