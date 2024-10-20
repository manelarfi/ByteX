using UnityEngine;
using System.IO.Ports;
using System.Collections;
using System.Text;

public class testtest : PersistentSingleton<testtest>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        Debug.Log("PCDeviceConfiguration script is loaded!");
    }

    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 9600;
    [SerializeField] private float connectionTimeout = 5f;
    [SerializeField] private int maxRetries = 3;

    private SerialPort serialPort;
    private bool isConnected = false;
    private StringBuilder incomingDataBuilder = new StringBuilder();

    void Awake()
    {
        Debug.Log("PCDeviceConfiguration Awake method called!");
    }

    void Start()
    {
        Debug.Log("PCDeviceConfiguration Start method called!");
        StartCoroutine(InitializeSerialPort());
    }

    private IEnumerator InitializeSerialPort()
    {
        Debug.Log("Attempting to initialize serial port...");
        while (!isConnected)
        {
            yield return StartCoroutine(TryConnect());
            if (!isConnected)
            {
                Debug.LogWarning("Connection attempt failed. Retrying in 1 second...");
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator TryConnect()
    {
        bool openedSuccessfully = false;

        try
        {
            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 100;
            serialPort.WriteTimeout = 100;
            serialPort.Open();
            openedSuccessfully = true;
            Debug.Log("Connected to SBC via " + portName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
            isConnected = false;
        }

        if (openedSuccessfully)
        {
            yield return StartCoroutine(VerifyDeviceConnection());
        }
    }

    private IEnumerator VerifyDeviceConnection()
    {
        float elapsedTime = 0f;
        bool sendSuccess = false;

        while (!isConnected && elapsedTime < connectionTimeout)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                sendSuccess = SendCommand("CMD001");
                if (sendSuccess)
                {
                    yield return StartCoroutine(WaitForResponse("ACK01", 5f));

                    if (incomingDataBuilder.ToString().Contains("ACK01"))
                    {
                        Debug.Log("Received ACK01: " + incomingDataBuilder.ToString());
                        isConnected = true;
                        yield return StartCoroutine(SendSettingsConfigurationWithRetry());
                    }
                    else
                    {
                        Debug.LogWarning("Did not receive ACK01. Received: " + incomingDataBuilder.ToString());
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.1f;
        }

        if (!isConnected)
        {
            Debug.LogError("Failed to verify device connection within timeout period.");
        }
    }

    private bool SendCommand(string command)
    {
        try
        {
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
            incomingDataBuilder.Clear();

            serialPort.WriteLine(command);
            Debug.Log("Sent command: " + command);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error sending command: " + e.Message);
            return false;
        }
    }

    private IEnumerator SendSettingsConfigurationWithRetry()
    {
        int retryCount = 0;

        while (retryCount < maxRetries)
        {
            yield return StartCoroutine(SendSettingsConfiguration());

            if (incomingDataBuilder.ToString().Contains("ACK02"))
            {
                Debug.Log("Settings configured successfully after " + (retryCount + 1) + " attempt(s).");
                yield break;
            }

            retryCount++;
            Debug.Log("Retrying SET command. Attempt " + (retryCount + 1) + " of " + maxRetries);
            yield return new WaitForSeconds(1f);
        }

        if (!incomingDataBuilder.ToString().Contains("ACK02"))
        {
            Debug.LogError("Failed to configure settings after " + maxRetries + " attempts.");
        }
    }

    private IEnumerator SendSettingsConfiguration()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            string playerChances = PlayerPrefs.GetString("PlayerChances", "5");
            string command = "SET05";

            switch (playerChances)
            {
                case "1": command = "SET01"; break;
                case "2": command = "SET02"; break;
                case "3": command = "SET03"; break;
                case "4": command = "SET04"; break;
                case "5": command = "SET05"; break;
                default: Debug.LogWarning("Invalid player chances, defaulting to SET05."); break;
            }

            if (SendCommand(command))
            {
                yield return StartCoroutine(WaitForResponse("ACK02", 8f));

                if (!incomingDataBuilder.ToString().Contains("ACK02"))
                {
                    Debug.LogWarning("Did not receive ACK02. Received: " + incomingDataBuilder.ToString());
                }
            }
        }
        else
        {
            Debug.LogError("Serial port not open or invalid.");
        }
    }

    private IEnumerator WaitForResponse(string expectedResponse, float timeout)
    {
        float elapsedTime = 0f;

        while (elapsedTime < timeout)
        {
            if (serialPort.BytesToRead > 0)
            {
                ReadFromSerialPort();
            }

            if (incomingDataBuilder.ToString().Contains(expectedResponse))
            {
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.1f;
        }

        Debug.LogWarning($"Timeout waiting for {expectedResponse}. Last received data: {incomingDataBuilder}");
    }

    private void ReadFromSerialPort()
    {
        try
        {
            string newData = serialPort.ReadExisting();
            incomingDataBuilder.Append(newData);
            Debug.Log("Received data: " + newData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading from serial port: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
