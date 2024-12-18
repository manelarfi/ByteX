using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HorizontalMenuNavigation : MonoBehaviour
{
    public Button[] menuButtons;           // Array of menu buttons
    public bool canNavigate = true;        // Flag to allow or restrict navigation input
    public GameObject selectedButton;      // Reference to the dropdown's selected button

    private int selectedIndex = 0;         // Currently selected button index
    private float inputDelay = 1f;         // Delay for input to prevent quick toggling
    private float nextInputTime = 0f;      // Timer for input delay

    private void Start()
    {
        SetSelectedButton();
    }

    private void Update()
    {
        // Only handle input if navigation is enabled
        if (canNavigate && Time.time >= nextInputTime)
        {
            HandleInput();                   // Handle user input for navigation
        }
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");

        // Navigate left
        if (horizontal > 0.5f || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeSelection(1);             // Move selection right
        }
        // Navigate right
        else if (horizontal < -0.5f || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeSelection(-1);            // Move selection left
        }

        // Submit action on the selected button
        if (Input.GetButtonDown("Submit"))
        {
            menuButtons[selectedIndex].onClick.Invoke(); // Trigger button click event
            ReturnToDropdown();                          // Return selection to dropdown's button
        }

        // Cancel action
        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Cancel action triggered.");
            ReturnToDropdown();                          // Return selection to dropdown's button
        }
    }

    private void ChangeSelection(int direction)
    {
        // Adjust selectedIndex in a circular manner
        selectedIndex = (selectedIndex + direction + menuButtons.Length) % menuButtons.Length;

        nextInputTime = Time.time + inputDelay; // Set the next input time
        SetSelectedButton();                    // Update the currently selected button
    }

    private void SetSelectedButton()
    {
        EventSystem.current.SetSelectedGameObject(menuButtons[selectedIndex].gameObject); // Highlight the button in the Event System
        Debug.Log("Current Selected Button: " + EventSystem.current.currentSelectedGameObject.name);
    }

    private void ReturnToDropdown()
    {
        if (selectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(selectedButton); // Highlight the dropdown's selected button
            Debug.Log("Returned to Dropdown Selected Button: " + selectedButton.name);
        }
    }

    // Call this method to disable navigation when dropdown opens
    public void DisableNavigation()
    {
        canNavigate = false;
    }

    // Call this method to enable navigation when dropdown closes
    public void EnableNavigation()
    {
        canNavigate = true;
    }
}
