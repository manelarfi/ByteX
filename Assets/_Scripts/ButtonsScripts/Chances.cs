using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chances : MonoBehaviour
{
    [SerializeField] private SettingsSO settingsSO; // Reference to settings
    [SerializeField] private TMP_Text nbChances; // UI text to display the chances

    private void Start()
    {
        UpdateLivesDisplay(); // Update display at start
    }

    public void AddLives()
    {
        if (settingsSO != null) 
        {
            settingsSO.chances += 1; // Increase chances
            UpdateLivesDisplay(); // Update the UI text
        }
    }

    public void RemoveLives()
    {
        if (settingsSO != null && settingsSO.chances > 1) 
        {
            settingsSO.chances -= 1; // Decrease chances
            UpdateLivesDisplay(); // Update the UI text
        }
    }

    private void UpdateLivesDisplay()
    {
        if (nbChances != null && settingsSO != null) 
        {
            nbChances.text = settingsSO.chances.ToString(); // Display the current number of chances
        }
    }
}
