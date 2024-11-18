using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject hammer;
    public TMP_Text label;

    public void SelectButton()
    {
        
    }

    public void DeselectButton()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        label.color = Color.yellow;
        LeanTween.rotateZ(hammer, -20, 0.2f).setOnComplete(() => {
            LeanTween.rotateZ(hammer, 45, 0.5f);
        });
    }

    public void OnDeselect(BaseEventData eventData)
    {
        label.color = Color.white;
    }
}
