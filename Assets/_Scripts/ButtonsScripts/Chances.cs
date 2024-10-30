using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChancesManager : MonoBehaviour
{
    [SerializeField] private ConfigSO config; // Reference to settings
    [SerializeField] private TMP_Text nbChances; // UI text to display the chances

    private void Start()
    {
        UpdateLivesDisplay(); // Update display at start
    }

    public void AddLives()
    {
        if (config != null) 
        {
            config.chances += 1; // Increase chances
            UpdateLivesDisplay(); // Update the UI text
        }
    }

    public void RemoveLives()
    {
        if (config != null && config.chances > 1) 
        {
            config.chances -= 1; // Decrease chances
            UpdateLivesDisplay(); // Update the UI text
        }
    }

    private void UpdateLivesDisplay()
    {
        if (nbChances != null && config != null) 
        {
            nbChances.text = config.chances.ToString(); // Display the current number of chances
        }
    }
}
