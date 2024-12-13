using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.IO;

public class SerialPortManager : StaticInstance<SerialPortManager>
{
    public ConfigSO config;
    public SerialPort SerialPort;
    private string portStatus;
    private bool isPortOperationInProgress = false;
    private bool isListening = true;
    public event Action OnOpenedSerialPort;
    public event Action OnCloseSerialPort;
    public event Action<string> OnReceivedData;

    public void OpenSerialPort()
    {
        if (isPortOperationInProgress) return;
        
        isPortOperationInProgress = true;

        try
        {
            if (SerialPort != null && SerialPort.IsOpen)
            {
                Debug.LogWarning("Attempted to open the serial port, but it is already open.");
                return;
            }

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
        finally
        {
            isPortOperationInProgress = false;
        }
    }

    public List<string> ListPorts()
    {
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
            try
            {
                SerialPort.Close();
                portStatus = "Port is closed";
                Debug.Log("Port closed");
                OnCloseSerialPort?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error closing port: {ex.Message}");
            }
            finally
            {
                SerialPort = null;
            }
        }
        yield return null;
    }

    public IEnumerator SendMessageAndWaitForAnswer(string data, string expectedResponse, Action<bool> callback, float timeoutSeconds = 30f)
    {
        if (SerialPort == null || !SerialPort.IsOpen || !isListening)
        {
            Debug.LogWarning("Serial port is not available for sending messages.");
            callback?.Invoke(false);
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < timeoutSeconds)
        {
            SendSerialData(data);
            yield return new WaitForSeconds(1f);
            elapsed += 1f;

            try
            {
                string receivedData = ReceiveSerialData();
                if (receivedData.Contains(expectedResponse))
                {
                    callback?.Invoke(true);
                    yield break;
                }
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("Timeout while waiting for response.");
            }

            if (!isListening)
            {
                callback?.Invoke(false);
                yield break;
            }
        }

        Debug.LogWarning($"Expected response '{expectedResponse}' not received.");
        callback?.Invoke(false);
    }

    private void SendSerialData(string data)
    {
        if (SerialPort != null && SerialPort.IsOpen)
        {
            try
            {
                SerialPort.WriteLine(data);
                Debug.Log("Sent data: " + data);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending data: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Serial port is not open. Cannot send data.");
        }
    }

    private string ReceiveSerialData()
    {
        if (SerialPort != null && SerialPort.IsOpen)
        {
            try
            {
                string receivedData = SerialPort.ReadExisting();
                Debug.Log("Received data: " + receivedData);
                return receivedData;
            }
            catch (Exception ex)
            {
                Debug.Log($"receiving data: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Serial port is not open.");
        }
        return string.Empty;
    }

    public IEnumerator CoroutineReadPort(Action<string> onDataReceived)
    {
        while (SerialPort != null && SerialPort.IsOpen)
        {
            try
            {
                string data = ReceiveSerialData();
                if (!string.IsNullOrEmpty(data))
                {
                    onDataReceived?.Invoke(data);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while reading from port: {ex.Message}");
            }
            yield return new WaitForSeconds(3f);
        }
    }

    public void StopAllSerialPortCoroutines()
    {
        StopAllCoroutines();
        isListening = false;
        Debug.Log("Stopped all serial port coroutines.");
    }

    public IEnumerator RestartPortWithDelay()
    {
        StopAllSerialPortCoroutines();
        yield return ClosePortSafely();
        yield return new WaitForSeconds(0.1f);
        OpenSerialPort();
    }
}
