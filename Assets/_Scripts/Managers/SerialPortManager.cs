using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;

using System.IO;

public class SerialPortManager : PersistentSingleton<SerialPortManager>
{
    public ConfigSO config;
    public SerialPort SerialPort;
    private string portStatus;

    public event Action OnOpenedSerialPort;    
    public event Action OnCloseSerialPort;
    public event Action<string> OnReceivedData;

    
    public void OpenSerialPort()
    {
        try
        {
            // Check if the port is already open
            if (SerialPort != null && SerialPort.IsOpen)
            {
                Debug.LogWarning("Attempted to open the serial port, but it is already open.");
                return; // Early exit if the port is already open
            }

            // Initialize the serial port with config parameters
            SerialPort = new SerialPort(config.portName, config.baudRate, config.parity, config.dataBits, config.stopBits)
            {
                ReadTimeout = config.readTimeout,
                WriteTimeout = config.writeTimeout
            };

            SerialPort.Open();
            portStatus = "Port is open";
            Debug.Log("Port opened");
            OnOpenedSerialPort?.Invoke();
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.LogError($"Access denied to the port: {ex.Message}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"I/O error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error opening port: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public List<string> ListPorts()
    {
        // Get available serial ports
        string[] ports = SerialPort.GetPortNames();
        List<string> portOptions = new List<string> { "none" };
        portOptions.AddRange(ports);
        return portOptions;
    }

    public void CloseSerialPort()
    {
        StartCoroutine(ClosePortSafely());
    }

    private IEnumerator ClosePortSafely()
    {
        if (SerialPort != null && SerialPort.IsOpen)
        {
            SerialPort.Close();
            portStatus = "Port is closed";
            Debug.Log("Port closed");
            OnCloseSerialPort?.Invoke();
        }
        yield return null; // Yield to ensure this method can be used in a coroutine
    }

    public IEnumerator SendMessageAndWaitForAnswer(string data, string expectedResponse, Action<bool> callback, float timeoutSeconds = 30f)
    {
        if (SerialPort == null || !SerialPort.IsOpen)
        {
            Debug.Log("Serial port is not open.");
            callback?.Invoke(false);
            yield break;
        }

        string receivedData = string.Empty;
        float startTime = 0f;

        while (startTime <= timeoutSeconds)
        {
            SendSerialData(data);
            yield return new WaitForSeconds(1f);
            startTime += 1f;

            try
            {
                receivedData = ReceiveSerialData();
                if (receivedData.Contains(expectedResponse))
                {
                    Debug.Log("Expected response received.");
                    callback?.Invoke(true);
                    yield break;
                }
            }
            catch (TimeoutException)
            {
                // Handle timeout silently or log if needed
            }
        }

        Debug.Log($"Expected response '{expectedResponse}' not received");
        callback?.Invoke(false);
    }

    private void SendSerialData(string data)
    {
        if (SerialPort != null && SerialPort.IsOpen)
        {
            SerialPort.WriteLine(data);
            Debug.Log("Sent data: " + data);
        }
        else
        {
            Debug.Log("Serial port is not open.");
        }
    }

    private string ReceiveSerialData()
    {
        if (SerialPort != null && SerialPort.IsOpen)
        {
            string receivedData = SerialPort.ReadExisting();
            Debug.Log("Received data: " + receivedData);
            return receivedData;
        }
        else
        {
            Debug.Log("Serial port is not open.");
            return string.Empty;
        }
    }

    public IEnumerator CoroutineReadPort(Action<string> onDataReceived)
    {
        while (SerialPort != null && SerialPort.IsOpen)
        {
            string data = ReceiveSerialData();
            if (!string.IsNullOrEmpty(data))
            {
                onDataReceived?.Invoke(data);
            }
            yield return new WaitForSeconds(3f); // Continue listening with a slight delay
        }
    }
}
