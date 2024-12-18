using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowScorePanel : MonoBehaviour
{
    public LivesManager chancesManager;
    public GameObject[] lights; // Array to store lights for easier management
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private Color color1 = Color.white; // Default color
    [SerializeField] private Color color2 = Color.yellow; // Wink color
    [SerializeField] private float delayAfterShowingScore = 2f; // Delay before hiding the panel after score is fully shown

    private void OnEnable()
    {
        // Start the score animation coroutine each time the panel is enabled
        //StartCoroutine(SetScore(InGameManager.Instance.currentScore));
    }

    private IEnumerator SetScore(int targetScore)
    {
        int currentScore = 0;
        float delay;
        bool isWinkColor = false;

        // Reset all lights to inactive at the start of each animation
        foreach (var light in lights)
        {
            light.SetActive(false);
        }

        // Animation loop to increment score with a "winking" color effect
        while (currentScore < targetScore)
        {
            // Determine step size and delay based on proximity to the target score
            int step = currentScore < targetScore * 0.7f ? 25 : 5;
            delay = currentScore < targetScore * 0.7f ? 0.03f : 0.07f;
            
            // Increment score
            currentScore += step;
            if (currentScore > targetScore) currentScore = targetScore;

            // Update the score text and color
            ScoreText.text = currentScore.ToString();
            ScoreText.color = isWinkColor ? color1 : color2;
            isWinkColor = !isWinkColor;

            // Activate a light every 100 points
            int lightIndex = currentScore / 100;
            if (lightIndex < lights.Length && !lights[lightIndex].activeSelf)
            {
                lights[lightIndex].SetActive(true);
            }

            yield return new WaitForSeconds(delay);
        }

        // Delay after showing the score before hiding the panel
        yield return new WaitForSeconds(delayAfterShowingScore);

        // Check if lives have been exhausted
        if (chancesManager.lives <= 0)
        {
            Debug.Log("No more lives. Returning to start screen.");
            SceneManager.LoadScene("START");
        } else 
        {
            // Now hide the panel
            gameObject.SetActive(false);
        }

        
    }
}
