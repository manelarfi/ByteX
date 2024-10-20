using UnityEngine;

[CreateAssetMenu(fileName = "AutoConnectionData", menuName = "ScriptableObjects/AutoConnectionData", order = 1)]
public class AutoConnectionData : ScriptableObject
{
    public bool didConnect;
    public string portName;
    public int Index;
}
