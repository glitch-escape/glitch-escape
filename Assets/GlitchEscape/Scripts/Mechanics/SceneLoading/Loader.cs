﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonoBehavior : MonoBehaviour { }

    public enum Scene
    {
        MainMenu, Opening, Tutorial_01_Movement, Vertical_Main_Level, Vertical_Platforming_Level, Hub_Level,
        CorruptionVisualTest, Tutorial_02_Advanced_Movement, Tutorial_03_Maze_Switch, Third_Level, SGDAEndCard,
        Tutorial_04_Fragment, FragmentPersitance, FragmentPersitance2, None, End_Cutscene, FragmentParticleTest,
        Transcendence_Cutscene, Courage_Cutscene, Humanity_Cutscene, Credits,
    }

    private static Action onLoadCallback;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scene scene)
    {
        //-------------------
        // To enable loading screen, uncomment below
        // onLoadCallback = () =>
        // {
        // GameObject loadingGameObject = new GameObject("Loading Game Object");
        //     loadingGameObject.AddComponent<LoadingMonoBehavior>().StartCoroutine(LoadSceneAsync(scene));
        // };
        // SceneManager.LoadScene(Scene.Loading.ToString());
        //-------------------
        // and comment out below
        SceneManager.LoadScene(scene.ToString());
        //-------------------
    }

    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }

    public static void LoaderCallback()
    {
        if (onLoadCallback != null)
        {
            onLoadCallback();
            onLoadCallback = null;
        }
    }
}
