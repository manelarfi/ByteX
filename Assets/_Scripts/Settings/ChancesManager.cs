using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ChancesManager : MonoBehaviour
{
    public TMP_Text numberText;         // TextMeshPro text to display the number
    public ConfigSO config;             // ScriptableObject to store config values
    public GameObject selectedButton;   // Button to select for changes

    private int currentNumber = 1;      // Starting number displayed
    private const int maxChances = 5;   // Maximum number of chances
    private const int minChances = 1;   // Minimum number of chances
    
    private float inputDelay = 0.2f;    // Delay for input to prevent quick toggling
    private float nextInputTime = 0f;   // Timer for input delay

    private void Start()
    {
        currentNumber = config.chances;
        // Initialize the displayed number and update text
        UpdateNumberText();
        SettingsManager.Instance.OnUpdateConfig += UpdateConfig;
    }

    private void Update()
    {
        if (Time.time >= nextInputTime) // Check if enough time has passed
        {
            // Only change if the selected object is the desired button
            if (EventSystem.current.currentSelectedGameObject == selectedButton)
            {
                HandleInput();
            }
        }
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if (horizontal > 0.8f || Input.GetKeyDown(KeyCode.RightArrow))
        {
            IncreaseChance();
        }
        else if (horizontal < -0.8f || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DecreaseChance();
        }
    }

    public void IncreaseChance()
    {
        if (currentNumber < maxChances)
        {
            currentNumber++; // Increment the current number
            UpdateNumberText();
            nextInputTime = Time.time + inputDelay; // Reset input delay timer
        }
    }

    public void DecreaseChance()
    {
        if (currentNumber > minChances)
        {
            currentNumber--; // Decrement the current number
            UpdateNumberText();
            nextInputTime = Time.time + inputDelay; // Reset input delay timer
        }
    }

    private void UpdateNumberText()
    {
        numberText.text = currentNumber.ToString(); // Update the TMP text
    }

    public void UpdateConfig()
    {
        config.chances = currentNumber; // Update the config with the current number
    }
}
