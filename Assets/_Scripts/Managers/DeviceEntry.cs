using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports; // Required to use SerialPort
using System.Collections.Generic;
using TMPro; // Required to use TMP_Dropdown

public class DeviceEntry : MonoBehaviour
{
    public AutoConnectionData autoConnectionData;
    public TMP_Dropdown dropdown; // Assign this in the Inspector

    public void TryPort() {
    Debug.Log("TryPort called");
    if (PCDeviceConfiguration.Instance == null) {
        Debug.LogError("PCDeviceConfiguration.Instance is null");
        return;
    }
    StartCoroutine(PCDeviceConfiguration.Instance.InitializeSerialPort());
    }

    void Start()
    {
        // Clear any pre-existing options in the dropdown
        dropdown.ClearOptions();

        // Get available serial ports
        string[] ports = SerialPort.GetPortNames();

        // Create a list of port options and add "none" as the first option
        List<string> portOptions = new List<string> { "none" };

        // Add the available ports to the list of options
        portOptions.AddRange(ports);

        // Populate the dropdown with the port options
        dropdown.AddOptions(portOptions);

        // Set the default selection to the previously stored index, or 0 ("none") if it is the first time
        dropdown.value = autoConnectionData.Index >= portOptions.Count ? 0 : autoConnectionData.Index;

        // Listen for changes in the dropdown selection
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Method called when dropdown value changes
    void OnDropdownValueChanged(int index)
    {
        // If index is 0, set portName to "none"
        if (index == 0)
        {
            Debug.Log("Selected port: none");
            autoConnectionData.portName = "none";
        }
        else
        {
            // Log the selected port
            Debug.Log("Selected port: " + dropdown.options[index].text);

            // Update the port name in AutoConnectionData based on selection
            autoConnectionData.portName = dropdown.options[index].text;
        }

        // Update the selected index for future use
        autoConnectionData.Index = index;
    }
}
