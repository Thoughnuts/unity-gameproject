using System.Collections;
<<<<<<< Updated upstream
=======
using System.Collections.Generic;
>>>>>>> Stashed changes
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
<<<<<<< Updated upstream
        StartCoroutine(FadeAndLoadScene(1));
=======
        SceneManager.LoadSceneAsync(1);
>>>>>>> Stashed changes
    }

    public void Credits()
    {
<<<<<<< Updated upstream
        StartCoroutine(FadeAndLoadScene(2));
=======
        SceneManager.LoadSceneAsync(2);
>>>>>>> Stashed changes
    }

    public void Exit()
    {
        Application.Quit();
    }

<<<<<<< Updated upstream
    IEnumerator FadeAndLoadScene(int sceneIndex)
    {
        // Create fade object
        GameObject fadeObject = new GameObject("FadeOverlay");
        Canvas canvas = fadeObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        UnityEngine.UI.Image fadeImage = fadeObject.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        // Fade to black
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Load scene and WAIT for it to fully load
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false; // Don't switch immediately

        while (!asyncLoad.isDone)
        {
            // When loading is 90% complete, activate the scene
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
=======

}
>>>>>>> Stashed changes
