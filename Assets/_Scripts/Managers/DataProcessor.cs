using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class DataProcessor : PersistentSingleton<DataProcessor>
{
    public SerialPortManager _serialPortManager;
    public ConfigSO config;
    public GameObject ConnectionPanel;

    private FadeInFadeOutAnim _fadeInFadeOutAnim;
    private ConnectionPanelController _connectionPanelController;
    public bool isConnected;
    public bool canStart;

    private string latestReceivedData;
    private Coroutine readPortCoroutine;

    public event Action<string> delegateInfo;

    private void Start()
    {
        config.Load();
        canStart = false;
        isConnected = false;

        _serialPortManager = GetComponent<SerialPortManager>();
        _connectionPanelController = ConnectionPanel.GetComponent<ConnectionPanelController>();
        _fadeInFadeOutAnim = ConnectionPanel.GetComponent<FadeInFadeOutAnim>();

        _serialPortManager.OnOpenedSerialPort += InitialiseApp;
        _serialPortManager.OnCloseSerialPort += HandlePortClosed;

        _serialPortManager.OpenSerialPort();
    }

    private void OnDisable()
    {
        StopReadingPort();
        _serialPortManager.OnOpenedSerialPort -= InitialiseApp;
        _serialPortManager.OnCloseSerialPort -= HandlePortClosed;
    }

    public void InitialiseApp()
    {
        Debug.Log("Initializing application...");
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
                    _fadeInFadeOutAnim.FadeOutAndDisable();
                }
            }));
        }
    }

    public IEnumerator SendSETCommand()
    {
        string data = $"SET0{config.chances}";
        string expectedAnswer = "ACK02";

        yield return _serialPortManager.SendMessageAndWaitForAnswer(data, expectedAnswer, responseReceived =>
        {
            if (responseReceived)
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

        _fadeInFadeOutAnim.FadeOutAndDisable();
    }

    public IEnumerator SendStartCommand()
    {
        string data = "CMD02";
        string expectedAnswer = "ACK03";

        yield return _serialPortManager.SendMessageAndWaitForAnswer(data, expectedAnswer, responseReceived =>
        {
            if (responseReceived)
            {
                canStart = true;
                SceneManager.LoadScene("START");
            }
            else
            {
                Debug.Log("Start command acknowledgment not received.");
            }
        });
    }

    public void ListenToCOMPort()
    {
        StopReadingPort();
        readPortCoroutine = StartCoroutine(_serialPortManager.CoroutineReadPort(UpdateReceivedData));
    }

    private void StopReadingPort()
    {
        if (readPortCoroutine != null)
        {
            StopCoroutine(readPortCoroutine);
            readPortCoroutine = null;
            Debug.Log("Stopped reading from port.");
        }
    }

    private void UpdateReceivedData(string data)
    {
        latestReceivedData = data;
        delegateInfo?.Invoke(data);
        Debug.Log("Updated latest received data: " + latestReceivedData);
    }

    public void RestartCommunication()
    {
        Debug.Log("Restarting communication...");
        StopReadingPort();
        StartCoroutine(_serialPortManager.RestartPortWithDelay());
    }

    private void HandlePortClosed()
    {
        Debug.Log("Serial port was closed. Resetting connection state.");
        isConnected = false;
    }

    private void OnApplicationQuit()
    {
        config.Save();
        Debug.Log("Application quitting. Closing serial port...");
        StopReadingPort();
        _serialPortManager.CloseSerialPort();
    }
}
