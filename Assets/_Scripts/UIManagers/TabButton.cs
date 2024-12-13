using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public TabGroup tabGroup;
    public TMP_Text buttonText;
    public Image background;

    private void Start() {
        background = GetComponent<Image>();
    }

    public void swapSprite (Sprite sprite)
    {
        if (sprite == null)
        {
            background.sprite  = sprite;
        }
    }


}
