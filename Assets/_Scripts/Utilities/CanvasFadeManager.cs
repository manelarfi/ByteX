using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasFadeManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;         // Reference to the CanvasGroup component
    public float fadeDuration = 1f;         // Duration of the fade in/out animations

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Immediately set the CanvasGroup to transparent and fade out
        canvasGroup.alpha = 1;
        FadeOut();
    }

    private void OnEnable()
    {
        // Subscribe to the scene loaded event to trigger fade-in on scene change
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when this object is disabled
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void FadeOut()
    {
        // Use LeanTween to fade out the canvas group over `fadeDuration` seconds
        LeanTween.alphaCanvas(canvasGroup, 0, fadeDuration).setEase(LeanTweenType.easeInOutQuad);
    }

    private void FadeIn()
    {
        // Use LeanTween to fade in the canvas group over `fadeDuration` seconds
        LeanTween.alphaCanvas(canvasGroup, 1, fadeDuration).setEase(LeanTweenType.easeInOutQuad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Fade in the canvas group when a new scene is loaded
        FadeIn();
    }
}
