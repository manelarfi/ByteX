using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LivesManager : MonoBehaviour
{
    [SerializeField] private TMP_Text nbLives;
    
    public int lives
    {
        get
        {
            int remChances;
            if (int.TryParse(nbLives.text, out remChances))
            {
                return remChances;
            }
            return -1;
        }

        set => nbLives.text = value.ToString();
    }
}
