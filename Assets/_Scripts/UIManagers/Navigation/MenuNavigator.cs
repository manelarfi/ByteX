using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuNavigator : MonoBehaviour
{
    public Button[] menuButtons;           // Array of menu buttons
    public bool canNavigate = true;        // Flag to allow or restrict navigation input

    private int selectedIndex = 0;         // Currently selected button index
    private float inputDelay = 1f;       // Delay for input to prevent quick toggling
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
        float vertical = Input.GetAxis("Vertical");

        // Move up
        if (vertical > 0.9f || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeSelection(-1);             // Move selection up
        }
        // Move down
        else if (vertical < -0.9f || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeSelection(1);              // Move selection down
        }

        // Horizontal input detected, keep the current selection highlighted
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0)
        {
            SetSelectedButton(); // Ensure the current button remains highlighted
        }

        // Submit action on selected button
        if (Input.GetButtonDown("Submit"))
        {
            // audioManagerr.Instance.PlaySFX(0);
            Debug.Log("submit");
            menuButtons[selectedIndex].onClick.Invoke(); // Trigger button click event
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
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
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
