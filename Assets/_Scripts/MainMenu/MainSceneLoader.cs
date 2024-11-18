using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneLoader : PersistentSingleton<MainSceneLoader>
{
    public CanvasGroup canvasGroup; // Reference to the CanvasGroup component
    public CanvasGroup SettingsGroup;
    
    public float fadeDuration = 1.5f; // Duration of the fade effect


    // Load Waiting Screen with Fade Transition
    public void LoadWaiting()
    {
        Debug.Log("1");
        if (DataProcessor.Instance.isConnected)
        {
            Debug.Log("2");
            StartCoroutine(DataProcessor.Instance.SendStartCommand());
        }
        else if (DataProcessor.Instance.canStart)
        {
            Debug.Log("3");
            SceneManager.LoadScene("START");
        }
        Debug.Log(DataProcessor.Instance.isConnected);
        Debug.Log(DataProcessor.Instance.canStart);
    }

    // Load Settings Scene with Fade Transition
    public void LoadSettingsScene()
    {
        // Fade out and then load the "SETTINGS" scene
        FadeOut(() => SceneManager.LoadScene("SETTINGS2"));
    }

    // Quit Application with Fade Out
    public void Quit()
    {
        // Fade out and then quit the application
        FadeOut(Application.Quit);
    }

    // Show a pop-up message if not connected
    private void ShowConnectionErrorPopup()
    {
        // Implement your pop-up message here
        Debug.Log("No connection. Please connect to continue.");
    }

    // Fade out the Canvas
    public void FadeOut(System.Action onComplete = null)
    {
        // Ensure the canvas group is initially fully visible
        SetAlpha(1f);

        // Fade the alpha to 0 (invisible) and invoke the onComplete action after fading out
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration).setOnComplete(onComplete);
    }

    // Fade in the Canvas
    public void FadeIn(System.Action onComplete = null)
    {
        // Ensure the canvas group is initially fully invisible
        SetAlpha(0f);

        // Fade the alpha to 1 (fully visible) and invoke the onComplete action after fading in
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration).setOnComplete(onComplete);
    }

    // Helper method to set the alpha of the canvas group directly
    private void SetAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
    }
}
