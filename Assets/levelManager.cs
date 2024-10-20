using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class levelManager : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;
    public SettingsSO settings;

        private void Start()
    {
        // Clear current options and add custom levels
        difficultyDropdown.ClearOptions();
        difficultyDropdown.AddOptions(new List<string>
        {
            "Super Easy",
            "Easy",
            "Intermediate",
            "Hard",
            "Super Hard"
        });

    //     // Add listener for when the dropdown value changes
        difficultyDropdown.onValueChanged.AddListener(DropdownValueChanged);
    }

        private void DropdownValueChanged(int selectedIndex)
    {
        Debug.Log("Selected Difficulty Index: " + selectedIndex);

        string command = $"SET0{selectedIndex + 1}\n";

        StartCoroutine(PCDeviceConfiguration.Instance.DeviceCommunication(command, "ACK02", (success) =>
        {
            if (success)
            {
                settings.level = selectedIndex + 1;
                Debug.Log("Settings Level updated to: " + settings.level);
            }
            else
            {
                Debug.LogError("Failed to set difficulty level.");
                difficultyDropdown.value = settings.level; // Reset to previous value
            }
        }));
    }
}
