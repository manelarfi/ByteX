using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneLoader : MonoBehaviour
{
    public bool canStart = false;
    public void LoadWaiting()
    {
        if (DataProcessor.Instance.isConnected)
        {
            SceneManager.LoadScene("START");
        }
        Debug.Log(DataProcessor.Instance.canStart);
    }

    public void LoadSettingsScene()
    {
        SceneManager.LoadScene("SETTINGS");
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void ShowConnectionErrorPopup()
    {
        // Implement your pop-up message here
        Debug.Log("No connection. Please connect to continue.");
    }
}
