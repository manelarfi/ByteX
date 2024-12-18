using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : StaticInstance<AudioManager> {
    public AudioSource musicSource; // For background music
    public AudioSource sfxSource;   // For sound effects
    public AudioClip[] audioClips;  // Array to hold 5 audio clips

    /// <summary>
    /// Plays a sound effect from the audioClips array.
    /// </summary>
    /// <param name="clipIndex">Index of the clip to play.</param>
    public void PlaySFX(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < audioClips.Length && audioClips[clipIndex] != null)
        {
            sfxSource.PlayOneShot(audioClips[clipIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid SFX clip index or clip is null.");
        }
    }

    /// <summary>
    /// Plays a music clip from the audioClips array.
    /// </summary>
    /// <param name="clipIndex">Index of the clip to play as music.</param>
    public void PlayMusic(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < audioClips.Length && audioClips[clipIndex] != null)
        {
            musicSource.clip = audioClips[clipIndex];
            musicSource.loop = true; // Ensures the music loops
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Invalid music clip index or clip is null.");
        }
    }

    /// <summary>
    /// Stops the currently playing music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}
