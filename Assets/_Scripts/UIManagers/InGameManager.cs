using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InGameManager : Singleton<InGameManager>
{
    private int rawScore;
    public int currentScore;
    public ConfigSO configSO;
    public ScoreBoard scoreBoard;
    public LivesManager chancesManager;
    public float sensivity = 1.5f;

    private void Start()
    {
        // Validate critical references
        if (!AreReferencesValid())
        {
            Debug.LogError("One or more required references are missing in InGameManager.");
            return;
        }

        // Subscribe to the data reception event
        DataProcessor.Instance.delegateInfo += Round;

        // Initialize lives and scores
        chancesManager.InitializeScoreboard(configSO.chances);
        chancesManager.lives = configSO.chances;
        scoreBoard.bestScore = configSO.bestScore;
    }

    private bool AreReferencesValid()
    {
        return DataProcessor.Instance != null &&
               configSO != null &&
               chancesManager != null &&
               scoreBoard != null ;
    }

    public void Round(string data)
    {
        Debug.Log("Received data for processing.");

        // Convert received data into raw score
        rawScore = ConvertScrScore(data);

        if (rawScore >= 0)
        {
            // Apply difficulty scaling to calculate the current score
            currentScore = InverseLogScalingWithDifficulty(rawScore, configSO.level);
            Debug.Log($"Calculated Current Score: {currentScore}");
            StartCoroutine(ShowAndSaveScore());
        }
        else
        {
            Debug.LogWarning("Invalid score data received.");
        }
    }

    private IEnumerator ShowAndSaveScore()
    {
        // Display score and save the result
        yield return StartCoroutine(scoreBoard.UpdateScoreCoroutine(currentScore, 5f));

        // Update scoreboard and lives
        scoreBoard.playerScore = currentScore;
        Debug.Log("table chances " );
        Debug.Log(configSO.chances - chancesManager.lives + 1 );
        chancesManager.UpdateScore(configSO.chances - chancesManager.lives, currentScore);

        // Check for new best score
        CheckBestScore(currentScore);

        // Decrement lives and reset score
        chancesManager.lives--;
        yield return StartCoroutine(scoreBoard.ResetWithBlinkingEffect());
        ResetScore();

        if (chancesManager.lives <= 0)
        {
            yield return WaitForScoreUpdateThenLoadScene();
        }
    }

    private void ResetScore()
    {
        currentScore = 0;
        scoreBoard.playerScore = 0;
    }

    public int ConvertScrScore(string input)
    {
        foreach (string line in input.Split('\n'))
        {
            if (line.Contains("SCR|"))
            {
                string scoreString = line.Substring(line.IndexOf("SCR|") + 4);
                if (int.TryParse(scoreString, out int score))
                {
                    Debug.Log($"Parsed Score: {score}");
                    return score;
                }

                Debug.LogWarning("Invalid score format encountered.");
            }
        }

        return -1; // No valid score found
    }

    private int InverseLogScalingWithDifficulty(int rawScore, int difficulty, int minValue = 1, int maxValue = 1000000, int maxScore = 2000)
    {
        double logMax = Math.Log(maxValue - minValue + 1);
        double difficultyFactor = Math.Pow(sensivity, difficulty - 1);
        int score = (int)(maxScore - Math.Log(rawScore - minValue + 1) * maxScore / (logMax * difficultyFactor));
        return Mathf.Clamp(score, 0, maxScore);
    }

    private void CheckBestScore(int currentScore)
    {
        if (currentScore > configSO.bestScore)
        {
            configSO.bestScore = currentScore;
            scoreBoard.bestScore = currentScore;
            Debug.Log("New best score achieved!");
        }
    }

    private IEnumerator WaitForScoreUpdateThenLoadScene()
    {
        // Delay to allow score animations to finish
        yield return new WaitForSeconds(5f);
        LoadStartScene();
    }

    private void LoadStartScene()
    {
        Debug.Log("No chances left, loading START scene...");
        SceneManager.LoadScene("START");
    }

    private void OnDestroy()
    {
        if (DataProcessor.Instance != null)
        {
            DataProcessor.Instance.delegateInfo -= Round;
        }
    }
}
