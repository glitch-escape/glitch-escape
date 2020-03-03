using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OpeningController : MonoBehaviour
{
    public Button continueButtom;

    private AudioSource sceneAudioSource;

    public List<Image> images;

    private int counter = 1;
    
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
    }

    IEnumerator FadeOutCoroutine(Image image)
    {
        EventSystem.current.SetSelectedGameObject(null);
        continueButtom.gameObject.SetActive(false);
        for (float f = 1; f >= -FADE_INTERVAL; f -= FADE_INTERVAL)
        {
            Color c = image.color;
            c.a = f;
            image.color = c;
            yield return new WaitForSeconds(FADE_DELAY);
        }
        StartCoroutine(FadeInCoroutine(images[counter]));
    }

    IEnumerator FadeInCoroutine(Image image)
    {
        for (float f = 0; f <= 1; f += FADE_INTERVAL)
        {
            Color c = image.color;
            c.a = f;
            image.color = c;
            yield return new WaitForSeconds(FADE_DELAY);
        }
        counter++;
        continueButtom.gameObject.SetActive(true);
        continueButtom.Select();
    }

    public void OnContinue()
    {
        if (counter >= images.Count)
        {
            GameStart();
        }
        else
        {
            StartCoroutine(FadeOutCoroutine(images[counter - 1]));
        }

        if (counter == 1)
        {
            sceneAudioSource.Play();
        }

    }

    public void GameStart()
    {
        Loader.Load(Loader.Scene.Vertical_Main_Level);
    }
}
