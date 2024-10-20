using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResetBestScore : MonoBehaviour
{
    [SerializeField] private TMP_Text score; // UI text to display the score
    [SerializeField] private SettingsSO settings; // Reference to SettingsSO to track the score

    void Start()
    {
        if (score == null)
        {
            Debug.LogError("Score text is not assigned in the inspector.");
        }

        if (settings == null)
        {
            Debug.LogError("SettingsSO is not assigned in the inspector.");
        }

        UpdateScoreDisplay(); // Update score display when the game starts
    }

    // Method to reset the score
    public void ResetGameScore()
    {
        if (settings != null) 
        {
            settings.bestScore = 0; // Reset score in settings
            UpdateScoreDisplay(); // Update the score displayed in the UI
        }
        else
        {
            Debug.LogWarning("SettingsSO is missing, cannot reset score.");
        }
    }

    // Method to update the UI text with the current score
    private void UpdateScoreDisplay()
    {
        if (score != null && settings != null) 
        {
            score.text = settings.bestScore.ToString(); // Display the current score
        }
        else
        {
            Debug.LogWarning("Cannot update score display. Either score text or settings is missing.");
        }
    }
}
