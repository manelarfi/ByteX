using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class DataProcessor : PersistentSingleton<DataProcessor>
{
    public SerialPortManager _serialPortManager;
    public ConfigSO config;
    public GameObject ConnectionPanel;

    private FadeInFadeOutAnim _FadeInFadeOutAnim;
    private ConnectionPanelController _connectionPanelController;    
    public bool isConnected ;
    public bool canStart;
    
    private string latestReceivedData;
    private Coroutine readPortCoroutine; // To manage the reading coroutine

    public event Action<string> delegateInfo;

    private void Start() 
    {
        config.Load();
        canStart = false;
        isConnected = false;
        _serialPortManager = GetComponent<SerialPortManager>();
        _connectionPanelController = ConnectionPanel.GetComponent<ConnectionPanelController>();
        _FadeInFadeOutAnim = ConnectionPanel.GetComponent<FadeInFadeOutAnim>();
        _serialPortManager.OnOpenedSerialPort += InitialiseApp;
        _serialPortManager.OpenSerialPort();

        StartCoroutine(checkConnection());
    }


    private void OnDisable()
    {
        // Stop listening and clean up
        StopReadingPort();
        _serialPortManager.OnOpenedSerialPort -= InitialiseApp;
    }

    private IEnumerator checkConnection()
{
    while (SceneManager.GetActiveScene().name == "MAIN" && !isConnected)
    {
        Debug.Log("Checking connection on returning to main scene...");
        InitialiseApp();

        // Wait for a few seconds before checking again
        yield return new WaitForSeconds(5f);
    }
}


    public void InitialiseApp()
    {
        string data = "CMD01";
        string expectedAnswer = "ACK01";
        
        if (isConnected)
        {
            RestartCommunication();
        } 
        else if (config.didConnect)
        {
            ConnectionPanel.SetActive(true);
            _connectionPanelController.SetConnexionActive();
            StartCoroutine(_serialPortManager.SendMessageAndWaitForAnswer(data, expectedAnswer, responseReceived =>
            {
                if (responseReceived)
                {
                    Debug.Log("Expected response received. Continuing with application initialization.");
                    StartCoroutine(SendSETCommand());
                }
                else
                {
                    Debug.LogWarning("Expected response not received within the timeout.");
                    _connectionPanelController.SetFailedActive();
                    _FadeInFadeOutAnim.FadeOutAndDisable();
                }
                
            }));
        }
    }


    public IEnumerator SendSETCommand()
    {
        string data = $"SET0{config.chances}";
        string expectedAnswer = "ACK02";

        Debug.Log(data);
        yield return _serialPortManager.SendMessageAndWaitForAnswer(data, expectedAnswer, responseReceived2 =>
        {
            if (responseReceived2)
            {
                isConnected = true;
                Debug.Log("Initialization successful. Starting game.");
                _connectionPanelController.SetConnectedActive();
                
            }
            else
            {
                Debug.LogWarning("Failed to set level. Retrying initialization.");
                _connectionPanelController.SetFailedActive();
            }
        });
        _FadeInFadeOutAnim.FadeOutAndDisable();
    }
    
    public IEnumerator SendStartCommand()
    {
        string data = "CMD02";
        string expectedAnswer = "ACK03";

        Debug.Log("command sent");
        yield return _serialPortManager.SendMessageAndWaitForAnswer(data, expectedAnswer, responseReceived2 =>
        {
            if (responseReceived2)
            {
                canStart = true;
                SceneManager.LoadScene("START");
            }
            else
            {
                Debug.Log("did not receive");
            }
        }); 
        
    }

    public void ListenToCOMPort()
    {
        StopReadingPort(); // Ensure previous reading coroutine is stopped
        readPortCoroutine = StartCoroutine(_serialPortManager.CoroutineReadPort(UpdateReceivedData));
    }

    private void StopReadingPort()
    {
        if (readPortCoroutine != null)
        {
            StopCoroutine(readPortCoroutine);
            readPortCoroutine = null; // Clear the reference
            Debug.Log("stopped reading from port");
        }
    }

    private void UpdateReceivedData(string data)
    {
        latestReceivedData = data;
        delegateInfo?.Invoke(data);
        Debug.Log("Updated latest received data: " + latestReceivedData);

    }

    private void OnApplicationQuit()
    {
        config.Save();
        Debug.Log("Application quitting. Closing serial port...");
        StopReadingPort(); // Stop reading before closing
        _serialPortManager.CloseSerialPort();
    }

    private void RestartCommunication()
    {
        Debug.Log("Restarting communication...");
        StopReadingPort(); // Ensure reading stops
        _serialPortManager.CloseSerialPort();
        _serialPortManager.OpenSerialPort();
    }
}