using UnityEngine;
using System.IO;
using System.IO.Ports;

[CreateAssetMenu(fileName = "ConfigSO", menuName = "ScriptableObjects/ConfigSO", order = 1)]
public class ConfigSO : ScriptableObject
{
    [Header("Serial Port Settings")]
    public string portName = "COM3";
    public int Index = 0;
    public int baudRate = 115200;
    public Parity parity = Parity.None;
    public StopBits stopBits = StopBits.One;
    public int dataBits = 8;
    public bool dtrEnable;
    public bool rtsEnable;
    public bool didConnect;
    public int index;
    public int readTimeout = 100;
    public int writeTimeout = 100;

    [Header("Game Settings")]
    public int level;
    public int chances;
    public int bestScore;

    private string filePath;

    private void OnEnable()
    {
        filePath = Path.Combine(Application.persistentDataPath, $"{name}.json");
    }

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(this, true); // true for pretty print
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Config data saved to: " + filePath);
    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(jsonData, this);
            Debug.Log("Config data loaded from: " + filePath);
        }
        else
        {
            Debug.LogWarning("Config file not found. Using default values.");
        }
    }
}
