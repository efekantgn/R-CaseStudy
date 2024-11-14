using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Event Channels")]
    [SerializeField] private SceneEventChannel sceneLoadEventChannel;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup transitionCanvasGroup;
    [SerializeField] private float fadeDuration = 1f; // Fade in/out süresi

    private void OnEnable()
    {
        sceneLoadEventChannel.OnSceneLoadRequested += LoadSceneWithFade;
        sceneLoadEventChannel.OnAppQiutRequested += QuitApp;
    }

    private void OnDisable()
    {
        sceneLoadEventChannel.OnSceneLoadRequested -= LoadSceneWithFade;
        sceneLoadEventChannel.OnAppQiutRequested -= QuitApp;
    }

    private void Start()
    {
        sceneLoadEventChannel.OnSceneLoadRequested.Invoke("MainMenu");
    }
    private void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }
    private void QuitApp() => Application.Quit();

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Fade Out
        yield return StartCoroutine(Fade(1));

        // Eski sahneleri temizle
        UnloadAllScenesExcept("Initialize");

        // Yeni sahneyi yükle
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            yield return null;
        }

        // Fade In
        yield return StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = transitionCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        transitionCanvasGroup.alpha = targetAlpha;
    }

    private void UnloadAllScenesExcept(string sceneToKeep)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != sceneToKeep)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
}
