using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;

public class SerialPortManager : PersistentSingleton<SerialPortManager>
{
    //this class handles all the methods/coroutines to communicate with the serial port

    //config data
    public ConfigSO config;

    
    public SerialPort SerialPort;
    private string portStatus;
    
    //events 
    public event Action OnOpenedSerialPort;    
    public event Action OnCloseSerialPort;

    // Open the serial port function
    public void OpenSerialPort()
    {
        try
        {
            // Initialize the serial port with AutoConnectionData parameters
            SerialPort = new SerialPort(config.portName,
                config.baudRate,
                config.parity,
                config.dataBits,
                config.stopBits)
            {
                ReadTimeout = config.readTimeout,
                WriteTimeout = config.writeTimeout
            };

            SerialPort.Open();
            portStatus = "Port is open";
            Debug.Log("Port opened");
            OnOpenedSerialPort?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.Log("Error opening port: " + ex.Message);
        }
    }

    //list available ports
    public List<string> ListPorts()
    {
        // Get available serial ports
        string[] ports = SerialPort.GetPortNames();

        // Create a list of port options and add "none" as the first option
        List<string> portOptions = new List<string> { "none" };


        // Add the available ports to the list of options
        portOptions.AddRange(ports);
        return portOptions;
    }

    // Close the serial port function
    public void CloseSerialPort()
    {
        Debug.Log("salam");
        if (SerialPort != null && SerialPort.IsOpen)
        {
            SerialPort.Close();
            portStatus = "Port is closed";
            Debug.Log("Port closed");
            OnCloseSerialPort?.Invoke();
        }
    }

    public IEnumerator SendMessageAndWaitForAnswer(string data, string expectedResponse, Action<bool> callback, float timeoutSeconds = 30f)
{
    if (SerialPort == null || !SerialPort.IsOpen)
    {
        Debug.Log("Serial port is not open.");
        callback?.Invoke(false);
        yield break;
    }
        // Reset the received data and start time for each attempt
        string receivedData = string.Empty;
        float startTime = 0f;

        // Wait for the expected response or until timeout
        while (startTime <= timeoutSeconds)
        {
            SendSerialDataAsLine(data);

                try {
                    Debug.Log("trying");
                receivedData = ReceiveSerialData();

                // Check if the received data contains the expected response
                if (receivedData.Contains(expectedResponse))
                {
                    Debug.Log("Expected response received.");
                    callback?.Invoke(true);  // Trigger callback with success
                    yield break;
                }
                } catch (TimeoutException)
                {

                }
                
            startTime++;
            yield return new WaitForSeconds(1f);
        }

    // All attempts failed, invoke callback with false
    Debug.Log($"Expected response '{expectedResponse}' not received");
    callback?.Invoke(false);  // Trigger callback with failure
}


    // Method to send data as a line (without waiting for a response)
    private void SendSerialDataAsLine(string data)
    {
        if (SerialPort != null && SerialPort.IsOpen)
        {
            SerialPort.WriteLine(data);
            portStatus = "Sent data: " + data;
            Debug.Log(portStatus);
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
        yield return new WaitForSeconds(3f); // Continue listening without a delay
    }
}
}


