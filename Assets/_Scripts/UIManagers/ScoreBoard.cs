using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI playerScoreText;

    public int bestScore
    {
        get
        {
            if (int.TryParse(bestScoreText.text, out int bestScore))
            {
                return bestScore;
            }
            return -1;
        }
        set => bestScoreText.text = value.ToString();
    }

    public int playerScore
    {
        get
        {
            int playerScore;
            if (int.TryParse(playerScoreText.text, out playerScore))
            {
                return playerScore;
            }
            return -1;
        }
        set => playerScoreText.text = value.ToString();
    }

    // Coroutine to update the player's score with easing
    public void UpdatePlayerScoreGradually(int targetScore, float duration)
    {
        StartCoroutine(UpdateScoreCoroutine(targetScore, duration));
    }

    private IEnumerator UpdateScoreCoroutine(int targetScore, float duration)
    {
        int currentScore = playerScore;
        float timeElapsed = 0f;

        // Gradually update the score with easing (fast at first, then slow)
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            // Ease In-Out function: (t /= 0.5f) { if (t < 1) return 0.5f * Mathf.Pow(t, 3); }
            float t = timeElapsed / duration;
            float easedTime = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease-in to slow down at the end

            int newScore = Mathf.FloorToInt(Mathf.Lerp(currentScore, targetScore, easedTime));
            playerScore = newScore; // Update player score on the screen
            yield return null;
        }

        // Ensure the final score is set to the target score
        playerScore = targetScore;
    }
}
