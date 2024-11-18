using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject panel;
    public ConfigSO settings;
    public ScoreBoard scoreBoard;
    public LivesManager livesManager;
    public GameObject scoreHH;
    public bool test;

    public int playerScore;

    private void Start() {
        if (scoreBoard != null) 
            scoreBoard.bestScore = settings.bestScore;

        if (livesManager != null) 
            livesManager.lives = settings.chances;

        UpdatePlayerScore(playerScore);
        StartCoroutine(yawedi());
        
    }

    public IEnumerator yawedi()
    {
        yield return new WaitForSeconds(10f);
        playerScore = 700;
        scoreHH.SetActive(true);
    }

    private void Update() {
        UpdatePlayerScore(playerScore);
        if (livesManager.lives == 0)
        {
            SceneManager.LoadScene("START");
        } else {
            Debug.Log(livesManager.lives);
        }
    }

    // Process the incoming data (message)
    private void processMessage(string message) {
        if (!string.IsNullOrEmpty(message)) {
            if (int.TryParse(message, out int parsedScore)) {  // Use 'int' instead of 'Int32'
                playerScore = ConvertValueToScaledRange(parsedScore);
                panel.SetActive(true);
                decreaseLife();
            } else {
                Debug.LogWarning("Failed to parse the message as a score.");
            }
        } else {
            Debug.LogWarning("Received null or empty message.");
        }

        UpdatePlayerScore(playerScore);
    }

    // Convert the integer value to a range of 0 to 1000
    int ConvertValueToScaledRange(int intValue)
    {
        // Clamp the value to the range of int32
        int minValue = int.MinValue;
        int maxValue = int.MaxValue;

        // Normalize the value to a 0-1 range
        float normalizedValue = (float)(intValue - minValue) / (maxValue - minValue);

        // Scale the normalized value to a 0-1000 range
        int scaledValue = Mathf.RoundToInt(normalizedValue * 1000);

        return scaledValue;
    }

    public void testPanel() {
        if (!panel.activeSelf) {
            panel.SetActive(true);
        }
    }

    public void decreaseLife() {
        if (livesManager.lives > 0) {
            livesManager.lives--;
            Debug.Log("Remaining Lives: " + livesManager.lives);
            CheckEndOfChances();
        }
    }

    private void CheckEndOfChances() {
        if (livesManager.lives == 0) {
            SceneManager.LoadScene(3);
        }
    }

    private void UpdatePlayerScore(int score) {
        scoreBoard.playerScore = score;
        if (settings.bestScore < score)
        {
            settings.bestScore = score;
        }
    }
}
