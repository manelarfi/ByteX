using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class COMPortMenu : MonoBehaviour
{
    public ConfigSO Config;
    public TMP_Dropdown dropdown; // Assign this in the Inspector
    public TMP_InputField inputField; // Assign this in the Inspector

    public void TrySendSET() {
        Debug.Log("cc");
            DataProcessor.Instance.InitialiseApp();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // Clear any pre-existing options in the dropdown
        dropdown.ClearOptions();

        List<string> portOptions = SerialPortManager.Instance.ListPorts();
        // Populate the dropdown with the port options
        dropdown.AddOptions(portOptions);

        // Set the default selection to the previously stored index, or 0 ("none") if it is the first time
        dropdown.value = Config.Index >= portOptions.Count ? 0 : Config.Index;

        // Listen for changes in the dropdown selection
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int index)
    {
        // If index is 0, set portName to "none" and check the input field
        if (index == 0)
        {
            Debug.Log("Selected port: none");
            Config.portName = "none";

            // Check if the input field is not empty
            if (!string.IsNullOrEmpty(inputField.text))
            {
                // Optional: Clear the input field or log a warning
                Debug.LogWarning("Input field is not empty when 'none' is selected. Clearing it.");
                inputField.text = ""; // Clear the input field
            }
        }
        else
        {
            // Log the selected port
            Debug.Log("Selected port: " + dropdown.options[index].text);
            Config.portName = dropdown.options[index].text;
        }

        // Update the selected index for future use
        Config.Index = index;
    }
}
