using System.Collections;
using System.Collections.Generic;
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
}
