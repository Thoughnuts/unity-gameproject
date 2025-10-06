using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));

        // Load new scene
        SceneManager.LoadScene(sceneName);

        // Fade back in
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            fadeCanvasGroup.alpha = currentAlpha;
            yield return null;
        }

        fadeCanvasGroup.alpha = endAlpha;
    }
}