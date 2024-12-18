using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class coinCounterReset : MonoBehaviour
{
        [SerializeField] private TMP_Text coins; // UI text to display the score
    [SerializeField] private ConfigSO config; // Reference to ConfigSO to track the score

    public Button Reset;              // The Reset button to trigger
    public GameObject selectedButton; // The button we want selected to trigger Reset
    public MenuNavigator menuNavigator; // Reference to MenuNavigator to control navigation

    private bool isNavigatingToReset = false; // Track if we are navigating to Reset button

    private void Start()
    {
        if (coins == null)
        {
            Debug.LogError("Score text is not assigned in the inspector.");
        }

        if (config == null)
        {
            Debug.LogError("ConfigSO is not assigned in the inspector.");
        }

        if (menuNavigator == null)
        {
            Debug.LogError("MenuNavigator is not assigned in the inspector.");
        }
        
        UpdateScoreDisplay(); // Update score display when the game starts
    }

    private void Update()
    {
        // Check if the current selected GameObject matches the specified button
        if (EventSystem.current.currentSelectedGameObject == selectedButton)
        {
            // Player presses the right joystick (we'll assume horizontal axis for right joystick)
            float horizontalInput = Input.GetAxis("Horizontal");
            if (horizontalInput > 0.5f) // Threshold for detecting right joystick movement to the right
            {
                // Switch navigation to the Reset button
                NavigateToReset();
            }
        }

        // If the current selected GameObject is the Reset button, check if player presses right joystick again to return to main button
        if (EventSystem.current.currentSelectedGameObject == Reset.gameObject && isNavigatingToReset)
        {
            // Player presses the right joystick again to return to the selected button
            float horizontalInput = Input.GetAxis("Horizontal");
            if (horizontalInput < -0.5f) // Threshold for detecting left joystick movement
            {
                // Return navigation to the selected button
                ReturnToSelectedButton();
            }
        }
    }

    // Method to navigate to the Reset button
    private void NavigateToReset()
    {
        isNavigatingToReset = true;
        menuNavigator.canNavigate = false; // Disable navigation in the rest of the menu
        EventSystem.current.SetSelectedGameObject(Reset.gameObject); // Set the Reset button as selected
    }

    // Method to return navigation to the selected button
    private void ReturnToSelectedButton()
    {
        isNavigatingToReset = false;
        menuNavigator.canNavigate = true; // Re-enable navigation in the rest of the menu
        EventSystem.current.SetSelectedGameObject(selectedButton); // Set the selected button as selected
    }

    // Method to reset the score
    public void ResetGameScore()
    {
        if (config != null) 
        {
            config.coinCounter = 0;      // Reset score in settings
            UpdateScoreDisplay();      // Update the score displayed in the UI
        }
        else
        {
            Debug.LogWarning("ConfigSO is missing, cannot reset score.");
        }
    }

    // Method to update the UI text with the current score
    private void UpdateScoreDisplay()
    {
        if (coins != null && config != null) 
        {
            coins.text = config.coinCounter.ToString(); // Display the score as a 3-digit number
        }
        else
        {
            Debug.LogWarning("Cannot update score display. Either score text or config is missing.");
        }
}
}