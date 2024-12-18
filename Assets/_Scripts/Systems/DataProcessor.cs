using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class DataProcessor : StaticInstance<DataProcessor>
{
    public SerialPortManager _serialPortManager;
    public ConfigSO config;
    public GameObject ConnectionPanel;
    private FadeInFadeOutAnim _fadeInFadeOutAnim;
    private ConnectionPanelController _connectionPanelController;
    public bool isConnected;
    public bool canStart;
    private bool canConfigurate = false;
    private string latestReceivedData;
    private Coroutine readPortCoroutine;

    public event Action<string> delegateInfo;
    private void Start()
    {
        Debug.Log("ran start method");
        config.Load();
        _serialPortManager = GetComponent<SerialPortManager>();
        _connectionPanelController = ConnectionPanel.GetComponent<ConnectionPanelController>();
        _fadeInFadeOutAnim = ConnectionPanel.GetComponent<FadeInFadeOutAnim>();

        _serialPortManager.OnOpenedSerialPort += InitialiseApp;
        _serialPortManager.OnCloseSerialPort += HandlePortClosed;

        _serialPortManager.OpenSerialPort();
        // Start the coroutine when the game begins
        // StartCoroutine(CheckJoystickInput());
    }
    IEnumerator CheckJoystickInput()
    {
        while (true)
        {
            // Reset the canConfigurate flag before checking again
            canConfigurate = false;
            Debug.Log(canConfigurate);

            // Example for Left Joystick axis detection
            float horizontal = Input.GetAxis("Horizontal"); // Left Joystick X axis
            float vertical = Input.GetAxis("Vertical");     // Left Joystick Y axis

            // Check if the joystick has been moved
            if (horizontal != 0 || vertical != 0)
            {
                Debug.Log("Left Joystick moved: (" + horizontal + ", " + vertical + ")");
                canConfigurate = true;
                Debug.Log(canConfigurate);
            }

            // Wait for 0.2 seconds before checking again
            yield return new WaitForSeconds(1f);
        }
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
            Debug.Log(ConnectionPanel.activeSelf);
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

    //fix this and write BETTER CODE 
    public IEnumerator sendCommand(string data, string expectedAnswer, Action <bool> callback)
    {
        yield return _serialPortManager.SendMessageAndWaitForAnswer(data, expectedAnswer, callback);
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
                StartCoroutine(SendStartCommand());
            }
            else
            {
                Debug.LogWarning("Failed to set level. Retrying initialization.");
                _connectionPanelController.SetFailedActive();
                _fadeInFadeOutAnim.FadeOutAndDisable();
            }
        });
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
                // StartCoroutine(waitBeforeStart());
                _connectionPanelController.SetConnectedActive();
            }
            else
            {
                Debug.Log("Start command acknowledgment not received.");
                _connectionPanelController.SetFailedActive();
            }
        });

        _fadeInFadeOutAnim.FadeOutAndDisable();
    }

    public IEnumerator waitBeforeStart()
    {
        while (SceneManager.GetActiveScene().name != "START")
        {
            yield return new WaitForSeconds(15f);
            Debug.Log(SceneManager.GetActiveScene().name);
            if (SceneManager.GetActiveScene().name == "MAIN" && !canConfigurate)
            {
                Debug.Log(SceneManager.GetActiveScene().name);
                SceneManager.LoadScene("START");
            }
        }
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


    private void OnDestroy() {
        if (Instance == this)
    {
        Debug.Log("DataProcessor instance destroyed: " + gameObject.name);
    }
    }
}
