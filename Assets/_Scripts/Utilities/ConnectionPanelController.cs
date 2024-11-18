using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPanelController : MonoBehaviour
{
    public GameObject ConnexionPanel;
    public GameObject ConnectedPanel;
    public GameObject FailedConnectPanel;
    
    public void DisableAllPanels()
    {
        ConnexionPanel.SetActive(false);
        ConnectedPanel.SetActive(false);
        FailedConnectPanel.SetActive(false);
    }

    public void SetConnexionActive()
    {
        DisableAllPanels();
        ConnexionPanel.SetActive(true);
    }

    public void SetConnectedActive()
    {
        DisableAllPanels();
        ConnectedPanel.SetActive(true);
    }

    public void SetFailedActive()
    {
        DisableAllPanels();
        FailedConnectPanel.SetActive(true); // Corrected to activate FailedConnectPanel
    }
}
