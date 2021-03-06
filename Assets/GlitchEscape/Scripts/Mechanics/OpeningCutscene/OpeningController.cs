﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OpeningController : MonoBehaviour
{
    public Button continueButtom;
    private int autoPlayDelay = 1;
    public float autoplayDelaySecs = 1f;
    private AudioSource sceneAudioSource;

    public List<Image> images;

    private int imageCounter = 0;
    private int autoPlayCounter = 0;
    private bool isFading = false;

    private static float FADE_INTERVAL = 0.05f;
    private static float FADE_DELAY = 0.01f;

    void Start()
    {
        foreach (Image image in images)
        {
            Color c = image.color;
            c.a = 0f;
            image.color = c;
        }
        continueButtom.Select();
        Color cc = images[0].color;
        cc.a = 1f;
        images[0].color = cc;
        sceneAudioSource = GetComponent<AudioSource>();
        sceneAudioSource.Stop();
        StartCoroutine(AutoPlayCoroutine(images[imageCounter]));
    }

    IEnumerator FadeOutCoroutine(Image image)
    {
        image.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        continueButtom.gameObject.SetActive(false);
        for (float f = 1; f >= -FADE_INTERVAL; f -= FADE_INTERVAL)
        {
            Color c = image.color;
            c.a = f;
            image.color = c;
            yield return new WaitForSeconds(FADE_DELAY);
        }
        image.gameObject.SetActive(false);
        imageCounter++;
        StartCoroutine(FadeInCoroutine(images[imageCounter]));
    }

    IEnumerator FadeInCoroutine(Image image)
    {
        image.gameObject.SetActive(true);;
        for (float f = 0; f <= 1; f += FADE_INTERVAL)
        {
            Color c = image.color;
            c.a = f;
            image.color = c;
            yield return new WaitForSeconds(FADE_DELAY);
        }
        autoPlayCounter = 0;
        isFading = false;
    }

    IEnumerator AutoPlayCoroutine(Image image)
    {
        yield return new WaitForSeconds(autoplayDelaySecs);
        autoPlayCounter++;
        if (autoPlayCounter >= autoPlayDelay && !isFading)
        {
            PlayNext();
        }
        StartCoroutine(AutoPlayCoroutine(images[imageCounter]));
    }

    public void OnContinue()
    {
        PlayNext();
    }

    private void PlayNext()
    {
        if (imageCounter >= images.Count - 1)
        {
            if (!continueButtom.gameObject.activeInHierarchy) {
                continueButtom.gameObject.SetActive(true);
                continueButtom.Select();
                }
            GameStart();
        }
        else
        {
            isFading = true;
            StartCoroutine(FadeOutCoroutine(images[imageCounter]));
        }

        if (imageCounter == 0)
        {
            sceneAudioSource.Play();
        }
    }

    public void GameStart()
    {
        Loader.Load(Loader.Scene.Vertical_Platforming_Level);
    }
}
