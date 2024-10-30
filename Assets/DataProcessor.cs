using UnityEngine;
using System.Collections;
using System;

public class DataProcessor : PersistentSingleton<DataProcessor>
{
    // Manages the serial port/initialization/sending data.
    public SerialPortManager _serialPortManager;
    public ConfigSO config;
    
    public event Action OnStartGame;
    
    public bool isConnected { get; private set; }
    
    // Field to store the latest received data
    private string latestReceivedData;

    private void Start() 
    {
        isConnected = false;
        _serialPortManager = GetComponent<SerialPortManager>();
        _serialPortManager.OnOpenedSerialPort += InitialiseApp;
        OnStartGame += ListenToCOMPort;
        _serialPortManager.OpenSerialPort();
    }

    private void OnDisable()
    {
        // Unsubscribe from events to prevent memory leaks on disable/destroy
        _serialPortManager.OnOpenedSerialPort -= InitialiseApp;
        OnStartGame -= ListenToCOMPort;
    }

    public void InitialiseApp()
    {
        string data = "CMD01";
        string expectedAnswer = "ACK01";
        if (isConnected)
        {
            RestartCommunication();
        } 
        else
        {
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
                }
            }));
        }
    }

    public IEnumerator SendSETCommand()
    {
        string data = $"SET0{config.Index + 1}";
        string expectedAnswer = "ACK02";

        Debug.Log(data);
        yield return _serialPortManager.SendMessageAndWaitForAnswer(data, expectedAnswer, responseReceived2 =>
        {
            if (responseReceived2)
            {
                isConnected = true;
                Debug.Log("Initialization successful. Starting game.");
            }
            else
            {
                Debug.LogWarning("Failed to set level. Retrying initialization.");
            }
        });
    }

    private void ListenToCOMPort()
    {
        StartCoroutine(_serialPortManager.CoroutineReadPort(UpdateReceivedData));
    }
    
    private void UpdateReceivedData(string data)
    {
        latestReceivedData = data;
        Debug.Log("Updated latest received data: " + latestReceivedData);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application quitting. Closing serial port...");
        _serialPortManager.CloseSerialPort();
    }

    private void RestartCommunication()
    {
        Debug.Log("Restarting communication...");
        _serialPortManager.CloseSerialPort();
        _serialPortManager.OpenSerialPort();
    }
}
