using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinReceiver : MonoBehaviour
{
    private void Start() {
        PCDeviceConfiguration.Instance.OnReceivedData += processMessage;
    }

    private void processMessage (string message)
    {
        if (message.Contains("CMD003")) {
            Debug.Log("coin has been logged");
            SceneManager.LoadScene(4);
        }
    }
}
