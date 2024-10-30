using UnityEngine;
using System.IO.Ports;

[CreateAssetMenu(fileName = "ConfigSO", menuName = "ScriptableObjects/ConfigSO", order = 1)]
public class ConfigSO : ScriptableObject
{
    [Header("Serial Port Settings")]
    [Tooltip("Current COM port for connection.")]
    public string portName = "COM5";
    [Tooltip("Current COM port index in dropdown")]
    public int Index = 0 ;

    [Tooltip("Baud rate for serial communication. Options: 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200.")]
    public int baudRate = 115200;

    [Tooltip("Parity-checking protocol.")]
    public Parity parity = Parity.None;

    [Tooltip("Number of stop bits per byte.")]
    public StopBits stopBits = StopBits.One;

    [Tooltip("Number of data bits per byte.")]
    public int dataBits = 8;

    [Tooltip("Data Terminal Ready (DTR) signal state.")]
    public bool dtrEnable;

    [Tooltip("Request to Send (RTS) signal state.")]
    public bool rtsEnable;

    [Tooltip("Flag indicating successful connection.")]
    public bool didConnect;

    [Tooltip("Connection index.")]
    public int index;

    [Tooltip("Read timeout duration in milliseconds.")]
    public int readTimeout = 100;

    [Tooltip("Write timeout duration in milliseconds.")]
    public int writeTimeout = 100;

    [Header("Game Settings")]
    [Tooltip("Current game level.")]
    public int level;

    [Tooltip("Remaining chances for the player.")]
    public int chances;

    [Tooltip("Best score achieved by the player.")]
    public int bestScore;
}
