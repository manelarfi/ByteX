using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class AnimateHammer : MonoBehaviour
{
    public static event Action OnAnimateHammer; // Event for hammer animation
    public VideoPlayer videoPlayer; // Assign in Inspector

    public bool isPlaying = true;

    private void Start()
    {
        StartCoroutine(AnimateCoroutine());
    }

    private IEnumerator AnimateCoroutine()
    {
        while (isPlaying)
        {
            // Fire the animation event
            OnAnimateHammer?.Invoke();

            yield return new WaitForSeconds(0.5f);
            // Play the video
            videoPlayer.Play();

            

            // Wait for video length or 5 seconds (whichever is shorter)
            yield return new WaitForSeconds(5f);

            // Pause the video if it's still playing
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }
    }
}
