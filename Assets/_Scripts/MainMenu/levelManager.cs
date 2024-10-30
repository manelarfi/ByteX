using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class levelManager : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;
    public ConfigSO config;

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

        config.level = selectedIndex;

        StartCoroutine(DataProcessor.Instance.SendSETCommand());
    }
}
