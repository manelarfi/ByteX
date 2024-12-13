using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinReceiver : MonoBehaviour
{
    private bool isSceneLoading = false;

    private void Start() {
        DataProcessor.Instance.ListenToCOMPort();
        DataProcessor.Instance.delegateInfo += checkCommand;
    }

    private void checkCommand(string data)
    {
        Debug.Log(data + "hhhh");
        if (data != null && data.Contains("CMD03"))
        {
            Debug.Log("received CMD03");
            SceneManager.LoadScene("INGAME");
        }
    }

    private void Update() {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("MAIN");
        }
    }

    private void OnDestroy() {
        // Unsubscribe from the event to prevent memory leaks
        DataProcessor.Instance.delegateInfo -= checkCommand;
    }
}
