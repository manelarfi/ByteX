using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SettingsManager : Singleton<SettingsManager>
{
    public event Action OnUpdateConfig;
    public GameObject[] selectedGameObjects;

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            HandleSubmit();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            HandleCancel();
        }
    }

    private void HandleSubmit()
    {
        if (!ClearGameObjects())
        {
            Debug.Log("No special UI element selected. Updating config and returning to MAIN.");
            OnUpdateConfig?.Invoke();  // Trigger the config update event
            LoadMainScene();
        }
        else
        {
            Debug.Log("Submit pressed on a special button - no config update triggered.");
        }
    }

    private void HandleCancel()
    {
        Debug.Log("Cancel pressed - returning to MAIN without config update.");
        LoadMainScene();
    }

    private bool ClearGameObjects()
    {
        foreach (var button in selectedGameObjects)
        {
            if (EventSystem.current.currentSelectedGameObject == button)
            {
                return true;  // Return true if a specified button is currently selected
            }
        }
        return false;  // Return false if none of the specified buttons are selected
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("MAIN");  // Load the MAIN scene
    }
}
