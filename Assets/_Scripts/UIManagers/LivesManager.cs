using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LivesManager : MonoBehaviour
{
    [SerializeField] private TMP_Text nbLives;
    [SerializeField] private GameObject scoreboardPrefab; // The prefab for each scoreboard element
    [SerializeField] private Transform scoreboardParent; // The parent object where scoreboard elements will be instantiated

    public List<GameObject> scoreboardElements = new List<GameObject>(); // List to store instantiated elements

    // Property to get/set the number of lives
    public int lives
    {
        get
        {
            int remChances;
            if (int.TryParse(nbLives.text, out remChances))
            {
                return remChances;
            }
            return -1;
        }

        set => nbLives.text = value.ToString();
    }

    // Function to initialize the scoreboard with the number of chances
    public void InitializeScoreboard(int numberOfChances)
    {
        Debug.Log("SALAAAM");
        // Clear any existing scoreboard elements
        foreach (var element in scoreboardElements)
        {
            Destroy(element);
        }
        scoreboardElements.Clear();

        // Instantiate scoreboard elements based on the number of chances
        for (int i = 0; i < numberOfChances; i++)
        {
            // Instantiate the scoreboard element
            GameObject newElement = Instantiate(scoreboardPrefab, scoreboardParent);
            
            // Get the TMP_Text components for displaying chance number and score
            TMP_Text chanceText = newElement.transform.Find("ChanceNumber").GetComponent<TMP_Text>();
            if (chanceText == null)
            {
                Debug.Log("cc");
            }
            TMP_Text scoreText = newElement.transform.Find("Score").GetComponent<TMP_Text>();
            if (chanceText == null)
            {
                Debug.Log("scoreText");
            }

            // Set the chance number text
            chanceText.text = (i + 1).ToString();
            scoreText.text = "0"; // Initial score can be set to 0 for each try

            // Add the element to the list
            scoreboardElements.Add(newElement);
        }
    }

    // Function to update the score for a specific chance
    public void UpdateScore(int chanceIndex, int newScore)
    {
        if (chanceIndex >= 0 && chanceIndex < scoreboardElements.Count)
        {
            Debug.Log("cc rani hna");
            TMP_Text scoreText = scoreboardElements[chanceIndex].transform.Find("Score").GetComponent<TMP_Text>();
            scoreText.text = newScore.ToString();
        }
    }
}
