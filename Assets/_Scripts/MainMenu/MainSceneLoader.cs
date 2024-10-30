using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneLoader : Singleton<MainSceneLoader>
{
    public GameObject COMPORT_Menu;
    public GameObject Settings;
    public GameObject MainMenu;
    public void LoadPortMenu()
    {
        if (COMPORT_Menu != null)
        {
            COMPORT_Menu.SetActive(true);
            MainMenu.SetActive(false);
        }
    }

    public void ExitPortManu()
    {
        if (Application.isPlaying)
        {
            COMPORT_Menu.SetActive(false);
            MainMenu.SetActive(true);
        }
    }

    public void LoadSetting()
    {
        if (Settings != null)
        {
            Settings.SetActive(true);
            MainMenu.SetActive(false);
        }
    }

    public void ExitSettings ()
    {
        if (Application.isPlaying)
        {
            Settings.SetActive(false);
            MainMenu.SetActive(true);
        }
    }

    public void LoadWaiting()
    {
        if (Application.isPlaying && DataProcessor.Instance.isConnected)
        {
            
            SceneManager.LoadScene("WaitingScreen"); // This is Unity's SceneManager
        }
    }
}

