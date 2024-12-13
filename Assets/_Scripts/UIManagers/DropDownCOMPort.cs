using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropDownCOMPort : MonoBehaviour
{
    public ConfigSO Config;
    public TMP_Dropdown dropdown; // Assign this in the Inspector
    public TMP_InputField inputField; // Assign this in the Inspector
    public GameObject selectedButton; // The button that triggers dropdown opening
    public MenuNavigator menuNavigator;
    public GameObject validationPanel;

    public bool isDropdownOpen = false;
    private string previousSelectedPort;
    private float nextInputTime = 0f;

    void Start()
    {
        // Clear any pre-existing options in the dropdown
        dropdown.ClearOptions();

        // Get the list of available ports from the SerialPortManager
        List<string> portOptions = SerialPortManager.Instance.ListPorts();

        // Populate the dropdown with the port options
        dropdown.AddOptions(portOptions);

        // Set the dropdown selection based on Config.portName
        SetDropdownValueFromPortName();

        // Store the initial port as the previously selected port
        previousSelectedPort = Config.portName;

        // Listen for changes in the dropdown selection
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        SettingsManager.Instance.OnUpdateConfig += UpdateConfig;
    }

    private void Update()
    {
        // Check if the selected button is focused and right joystick is pressed to open dropdown
        if (EventSystem.current.currentSelectedGameObject == selectedButton && Input.GetAxis("Horizontal") > 0.5f)
        {
            OpenDropdown();
        }

        if (isDropdownOpen && Time.time >= nextInputTime)
        {
            HandleGamepadNavigation();
        }
    }

    private void OpenDropdown()
    {
        isDropdownOpen = true;
        menuNavigator.canNavigate = false; // Disable menu navigation
        dropdown.Show(); // Display the dropdown options

        // Set dropdown as the currently selected object for navigation
        EventSystem.current.SetSelectedGameObject(dropdown.gameObject);
    }

    private void HandleGamepadNavigation()
    {
        float vertical = Input.GetAxis("Vertical");
        bool submit = Input.GetButtonDown("Submit");
        bool leftJoystickPress = Input.GetAxis("Horizontal") < -0.5f;

        // Select the current option and close dropdown if submit is pressed
        if (submit)
        {
            validationPanel.SetActive(true);
        }
    }

    public void changeValue()
    {
        dropdown.onValueChanged.Invoke(dropdown.value);
    }
    
    public void CloseDropdown()
    {
        isDropdownOpen = false;
        //menuNavigator.canNavigate = true; // Re-enable menu navigation
        
        // Reset focus to the button that triggered the dropdown
        EventSystem.current.SetSelectedGameObject(selectedButton);
    }

    void OnDropdownValueChanged(int index)
    {
        // Get the newly selected port name
        string selectedPort = dropdown.options[index].text;

        // If a different port is selected, restart communication
        if (!string.IsNullOrEmpty(previousSelectedPort) && selectedPort != previousSelectedPort)
        {
            Debug.Log($"Port changed from {previousSelectedPort} to {selectedPort}. Restarting communication.");
            DataProcessor.Instance.RestartCommunication();
        }

        // Update the previously selected port
        previousSelectedPort = selectedPort;

        // If index is 0, clear the input field if not empty
        if (index == 0 && !string.IsNullOrEmpty(inputField.text))
        {
            inputField.text = ""; // Clear the input field
        }
    }
    public void UpdateConfig()
    {
        // Update Config.portName to the currently selected dropdown option
        Config.portName = dropdown.options[dropdown.value].text;

        // Update the stored index in Config for future use
        Config.Index = dropdown.value;
    }

    private void SetDropdownValueFromPortName()
    {
        int index = dropdown.options.FindIndex(option => option.text == Config.portName);
        dropdown.value = index != -1 ? index : 0; // Default to 0 if Config.portName is not found
    }
}
