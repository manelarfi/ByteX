using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private GameObject[] leftSmallLights; // Small lights on the left
    [SerializeField] private GameObject[] rightSmallLights; // Small lights on the right

    [SerializeField] private Color blinkColor1 = Color.blue;  // First color for blinking
    [SerializeField] private Color blinkColor2 = Color.yellow; // Second color for blinking

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
            if (int.TryParse(playerScoreText.text, out int playerScore))
            {
                return playerScore;
            }
            return -1;
        }
        set => playerScoreText.text = value.ToString();
    }

    public IEnumerator ResetWithBlinkingEffect()
    {
        // Blinking effect for player score
        float blinkDuration = 1.5f; // Total blinking duration
        float blinkInterval = 0.15f; // Interval between color toggles
        float timeElapsed = 0f;

        bool useColor1 = true; // Start with the first color

        while (timeElapsed < blinkDuration)
        {
            timeElapsed += blinkInterval;

            // Toggle between the two colors
            playerScoreText.color = useColor1 ? blinkColor1 : blinkColor2;
            useColor1 = !useColor1;

            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure text color resets to a default value (optional)
        playerScoreText.color = Color.yellow;

        // Reset all small lights
        DeactivateAllLights();
        Debug.Log("Score and lights reset.");
    }

    private void DeactivateAllLights()
    {
        // Deactivate all left small lights
        foreach (var light in leftSmallLights)
        {
            light.SetActive(false);
        }

        // Deactivate all right small lights
        foreach (var light in rightSmallLights)
        {
            light.SetActive(false);
        }
    }

    public IEnumerator UpdateScoreCoroutine(int targetScore, float duration)
    {
        int currentScore = playerScore;
        float timeElapsed = 0f;

        // Deactivate all lights before starting
        DeactivateAllLights();

        // Smooth transition for score update
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            // Ease-In-Out for smooth transitions
            float t = timeElapsed / duration;
            float easedTime = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease-in effect

            // Update the score
            int newScore = Mathf.FloorToInt(Mathf.Lerp(currentScore, targetScore, easedTime));
            playerScore = newScore;

            // Calculate how many small lights should be activated based on the current score
            int lightsToActivate = Mathf.FloorToInt(newScore / 100f); // Activate 1 light for every 200 points


            int rightLightsToActivate = Mathf.FloorToInt(lightsToActivate * (rightSmallLights.Length / (float)(leftSmallLights.Length + rightSmallLights.Length)));
            for (int i = rightLightsToActivate; i > 0; i--)
            {
                StartCoroutine(ActivateSmallLightWithDelay(i));
            }

            yield return null;
        }

        // Ensure the final score is set
        playerScore = targetScore;

        // Start the blinking effect for the final score
        StartCoroutine(BlinkFinalScore());

        // Activate all lights after the score is fully updated
        // foreach (var light in leftSmallLights)
        // {
        //     light.SetActive(true);
        // }

        // foreach (var light in rightSmallLights)
        // {
        //     light.SetActive(true);
        // }
    }

    private IEnumerator ActivateSmallLightWithDelay(int lightIndex)
    {
        // Gradually activate each small light with a small delay between them
        yield return new WaitForSeconds(lightIndex * 0.2f); // Adjust the delay time to slow down the light activation

        if (lightIndex < leftSmallLights.Length && lightIndex < rightSmallLights.Length)
        {
            leftSmallLights[lightIndex].SetActive(true);
            rightSmallLights[lightIndex].SetActive(true);
        }
    }

    private IEnumerator BlinkFinalScore()
    {
        float blinkDuration = 1.5f;
        float blinkInterval = 0.15f;
        float timeElapsed = 0f;

        bool useColor1 = true; // Start with the first color

        while (timeElapsed < blinkDuration)
        {
            timeElapsed += blinkInterval;
            playerScoreText.color = useColor1 ? blinkColor1 : blinkColor2;
            useColor1 = !useColor1;

            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure text color resets to a default value (optional)
        playerScoreText.color = Color.yellow;
    }
}
