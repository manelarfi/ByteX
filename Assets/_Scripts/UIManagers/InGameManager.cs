using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : Singleton<InGameManager>
{
    private int rawScore;
    public int currentScore;
    public ConfigSO configSO;
    public ScoreBoard scoreBoard;
    public LivesManager chancesManager;
    public GameObject ScorePanel;

    private void Start()
    {
        // Check for any missing references to prevent null errors
        if (DataProcessor.Instance == null || configSO == null || chancesManager == null || scoreBoard == null || ScorePanel == null)
        {
            Debug.LogError("One or more required references are missing in InGameManager.");
            return;
        }

        // Subscribe to the data reception event
        DataProcessor.Instance.delegateInfo += Round;

        // Initialize lives and best score from the config
        chancesManager.lives = configSO.chances;
        scoreBoard.bestScore = configSO.bestScore;
    }

    public void Round(string data)
    {
        Debug.Log("Received Data");

        // Convert the received data into a raw score
        rawScore = ConvertScrScore(data);

        if (rawScore >= 0) // Check for a valid raw score
        {
            currentScore = InverseLogScalingWithDifficulty(rawScore, configSO.level);
            Debug.Log("Current Score: " + currentScore);

            // Display score panel and update best score
            //ScorePanel.SetActive(true);
            scoreBoard.UpdatePlayerScoreGradually(currentScore, 5f);
            scoreBoard.playerScore = currentScore;
            CheckBestScore(currentScore);
            chancesManager.lives--;

        }
        else
        {
            Debug.LogWarning("Invalid score data received.");
        }
    }

    public int ConvertScrScore(string input)
    {
        // Split the input by newline and search for a line containing "SCR|"
        string[] lines = input.Split('\n');
        foreach (string line in lines)
        {
            if (line.Contains("SCR|"))
            {
                string scoreString = line.Substring(line.IndexOf("SCR|") + 4);

                if (int.TryParse(scoreString, out int score))
                {
                    Debug.Log("Parsed Score: " + score);
                    return score;
                }
                else
                {
                    Debug.LogWarning("Invalid score format.");
                }
            }
        }

        return -1; // Indicates no valid score was found
    }

    int SimpleInverseScoreWithDifficulty(int rawScore, int difficulty, int maxRawScore = 1000000, int maxScore = 1000)
    {
        // Clamp the raw score to avoid values beyond expected range
        rawScore = Mathf.Clamp(rawScore, 0, maxRawScore);

        // Calculate difficulty adjustment factor
        float difficultyFactor = 1f + (difficulty - 1) * 0.2f;
        int adjustedMaxScore = (int)(maxScore / difficultyFactor);

        // Calculate and clamp inverse score
        int calculatedScore = adjustedMaxScore - (int)((float)rawScore / maxRawScore * adjustedMaxScore);
        return Mathf.Clamp(calculatedScore, 0, maxScore);
    }

    int InverseLogScalingWithDifficulty(int x, int difficulty, int min_value = 1, int max_value = 1000000, int max_score = 1000)
{
    double logMax = Math.Log(max_value - min_value + 1);
    double difficultyFactor = Math.Pow(1.5, difficulty - 1);
    int score = (int)(max_score - Math.Log(x - min_value + 1) * max_score / (logMax * difficultyFactor));
    return Math.Max(0, Math.Min(max_score, score));
}

    private void CheckBestScore(int currentScore)
    {
        // Update and display best score if the current score exceeds the best
        if (currentScore > configSO.bestScore)
        {
            configSO.bestScore = currentScore;
            scoreBoard.bestScore = currentScore;
            Debug.Log("New best score achieved!");
            // Additional logic to display a "New Best Score" panel can be added here
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (DataProcessor.Instance != null)
        {
            DataProcessor.Instance.delegateInfo -= Round;
        }
    }
}
