using UnityEngine;
using System.IO.Ports;
using System.Collections;
using System.Threading;
using System.Text;
using System.IO;
using System;

public class PCDeviceConfiguration : PersistentSingleton<PCDeviceConfiguration>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        Debug.Log("PCDeviceConfiguration script is loaded!");
    }

    public event Action<string> OnReceivedData;
    public SettingsSO settingSO;

    public AutoConnectionData autoConnectionData;
    [SerializeField] private string portName;
    [SerializeField] private int baudRate = 9600;
    [SerializeField] private float connectionTimeout = 10f;
    [SerializeField] private int maxRetries = 3;

    private SerialPort serialPort;
    private Thread serialThread;
    private StringBuilder incomingDataBuilder = new StringBuilder();
    private bool isListening = false;
    public bool isConnected = false;

    void Start()
    {
        Debug.Log("PCDeviceConfiguration Start method called!");
        if (autoConnectionData.didConnect) 
        {
            StartCoroutine(InitializeSerialPort());
        }
    }

    public IEnumerator InitializeSerialPort()
    {
        portName = autoConnectionData.portName;

            yield return StartCoroutine(TryConnect());
            if (!isConnected)
            {
                Debug.LogWarning("Connection attempt failed. Retrying in 1 second...");
                yield return new WaitForSeconds(1f);
            }
        
    }

    private IEnumerator TryConnect()
{
    bool openedSuccessfully = false;
    int retries = 0;

    while (retries < maxRetries && !openedSuccessfully)
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 5000;
            serialPort.WriteTimeout = 500;
            serialPort.Open();
            openedSuccessfully = true;
            Debug.Log("Connected to SBC via " + portName);
        }
        catch (UnauthorizedAccessException)
        {
            Debug.LogError("Access denied. Ensure no other application is using the serial port and run as administrator.");
        }
        catch (IOException e)
        {
            Debug.LogError("IO error while trying to open serial port: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }

        if (!openedSuccessfully)
        {
            retries++;
            Debug.LogWarning($"Retrying connection... attempt {retries}");
            yield return new WaitForSeconds(1f); // Wait before retrying
        }
    }

    if (openedSuccessfully)
    {
        // Proceed to device communication after successfully opening the port
        yield return StartCoroutine(DeviceCommunication("CMD01", "ACK01", success =>
        {
            Debug.Log("2");
            isConnected = success;
            if (isConnected)
            {
                Debug.Log("4");
                StartCoroutine(DeviceCommunication("SET04","ACK02", s =>
                {
                    if (s)
                    {
                        StartListening();
                    }
                }));
                
            }
        }));
    }
    else
    {
        Debug.LogError("Failed to connect after maximum retries.");
    }
}


    public IEnumerator DeviceCommunication(string command, string expectedAnswer, Action<bool> callback)
    {
        int retries = 0;
        float elapsedTime = 0f;
        while (retries < maxRetries)
        {
            if (serialPort?.IsOpen == true && SendCommand(command))
            {
                yield return StartCoroutine(WaitForResponse(expectedAnswer, 10f));
                if (incomingDataBuilder.ToString().Contains(expectedAnswer)) {
                    callback(true);
                    yield break;
                }
            }
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.1f;
            retries ++;
        }

        Debug.LogError("Failed to verify device connection within timeout period.");
        callback(false);
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
        catch (Exception e)
        {
            Debug.LogError("Error sending command: " + e.Message);
            return false;
        }
    }

    private IEnumerator WaitForResponse(string expectedResponse, float timeout)
    {
        Debug.Log("waitforresponse");
        float elapsedTime = 0f;

        while (elapsedTime < timeout)
        {
            if (serialPort.BytesToRead > 0)
            {
                ReadFromSerialPort();
            }

            Debug.Log(incomingDataBuilder.ToString().Contains(expectedResponse));
            if (incomingDataBuilder.ToString().Contains(expectedResponse))
            {
                Debug.Log("1");
                yield break;
            }

            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        Debug.LogWarning($"Timeout waiting for {expectedResponse}. Last received data: {incomingDataBuilder}");
    }

    private void ReadFromSerialPort()
    {
        try
        {
            string newData = serialPort.ReadExisting().Trim();
            incomingDataBuilder.Append(newData);
            Debug.Log("Received data: " + newData);
            OnReceivedData?.Invoke(newData);
        }
        catch (Exception e)
        {
            Debug.LogError("Error reading from serial port: " + e.Message);
        }
    }

    private void StartListening()
    {
        isListening = true;
        serialThread = new Thread(ListenForData);
        serialThread.Start();
    }

    private void ListenForData()
    {
        while (isListening && serialPort.IsOpen)
        {
            try
            {
                string message = serialPort.ReadLine();
                Debug.Log("Received message: " + message);
                OnReceivedData?.Invoke(message);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error reading from serial port: " + ex.Message);
            }
        }
    }

    private void OnApplicationQuit()
    {
        isListening = false;
        if (serialPort?.IsOpen == true)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }
}
