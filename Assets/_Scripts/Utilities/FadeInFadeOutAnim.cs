using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInFadeOutAnim : MonoBehaviour
{
    public CanvasGroup panelCanvasGroup; // Reference to the CanvasGroup of the panel
    public float fadeDuration = 0.5f;    // Duration of the fade animation

    private void OnEnable()
    {
        FadeIn(); // Trigger fade-in animation when the panel is activated
    }

    private void FadeIn()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0; // Start with panel fully transparent
            LeanTween.alphaCanvas(panelCanvasGroup, 1, fadeDuration); // Fade in to fully visible
        }
    }

    public void FadeOutAndDisable()
    {
        if (panelCanvasGroup != null)
        {
            LeanTween.alphaCanvas(panelCanvasGroup, 0, fadeDuration).setOnComplete(() =>
            {
                gameObject.SetActive(false); // Disable the panel after fade-out is complete
            });
        }
    }
}
