using System.Collections;
using TMPro;
using UnityEngine;

public class ShowScorePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text ScoreText;

    public int Score
    {
        get
        {
            if (int.TryParse(ScoreText.text, out int intScore))
            {
                return intScore;
            }
            return -1; // Returns -1 if parsing fails
        }
        set => StartCoroutine(SetScore(value)); // Use Coroutine to set the score
    }

    private IEnumerator SetScore(int value)
    {
        for (int i = 0; i <= value; i++)
        {
            ScoreText.text = i.ToString();
            yield return new WaitForSeconds(0.02f); // Wait for half a second before updating the score again
        }

        gameObject.SetActive(false);
    }

    private void Start() {
        Score = GameManager.Instance.playerScore;
    }
}
