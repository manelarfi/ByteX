using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsSO", menuName = "ScriptableObjects/SettingsSO", order = 2)]
public class SettingsSO : ScriptableObject 
{
    public int level;
    public int chances;
    public int bestScore;
}
