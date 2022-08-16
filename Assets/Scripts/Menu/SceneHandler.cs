using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public enum SceneMap
{
    MENU_SCENE = 0,
    GAME_SCENE,
}

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }

    private List<AsyncOperation> sceneAsyncOperations = new List<AsyncOperation>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void LoadScene(SceneMap sceneMap, Action callback, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        sceneAsyncOperations.Add(SceneManager.LoadSceneAsync((int)sceneMap, loadSceneMode));
        StartCoroutine(SceneProgressCoroutine(sceneMap, callback));
    }

    public void UnloadScene(SceneMap sceneMap)
    {
        sceneAsyncOperations.Add(SceneManager.UnloadSceneAsync((int)sceneMap));
    }

    public IEnumerator SceneProgressCoroutine(SceneMap sceneMap, Action callback)
    {
        for (int i = 0; i < sceneAsyncOperations.Count; i++)
        {
            while (!sceneAsyncOperations[i].isDone)
            {
                yield return null;
            }
        }

        callback?.Invoke();
    }
}
