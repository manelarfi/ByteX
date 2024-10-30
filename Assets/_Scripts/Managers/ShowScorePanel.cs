using System.Collections;
using TMPro;
using UnityEngine;

public class ShowScorePanel : MonoBehaviour
{
    // [SerializeField] private TMP_Text ScoreText;
    // [SerializeField] private float delayAfterShowingScore = 2f;  // Delay before hiding the panel after score is fully shown

    // public int Score
    // {
    //     get
    //     {
    //         if (int.TryParse(ScoreText.text, out int intScore))
    //         {
    //             return intScore;
    //         }
    //         return -1; // Returns -1 if parsing fails
    //     }
    //     set => StartCoroutine(SetScore(value)); // Use Coroutine to set the score
    // }

    // private IEnumerator SetScore(int value)
    // {
    //     // Incrementally display the score
    //     for (int i = 0; i <= value; i++)
    //     {
    //         ScoreText.text = i.ToString();
    //         yield return new WaitForSeconds(0.02f); // Wait for 0.02 seconds before updating the score again
    //     }

    //     // Delay after showing the score before hiding the panel
    //     yield return new WaitForSeconds(delayAfterShowingScore);

    //     // Now hide the panel
    //     gameObject.SetActive(false);
    // }

    // private void Start()
    // {
    //     Score = GameManager.Instance.playerScore; // Assuming GameManager is handling the playerScore
    // }
}
