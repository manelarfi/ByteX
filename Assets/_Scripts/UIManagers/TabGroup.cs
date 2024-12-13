using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabGroup : MonoBehaviour
{
    public TabButton[] tabButtons;              // Array of TabButton objects
    public GameObject[] ObjectsToSwap;          // Array of objects to swap when tabs are selected
    public int currentTabIndex = 0;            // Index of the currently selected tab
    public Sprite tabIdle;
    public Sprite tabActive;
    public TabButton selectedTab;
    public float inputDelay = 0.25f;            // Delay to prevent rapid input

    private float nextInputTime = 0f;           // Time for the next allowed input

    private void Start()
    {
        // Initialize the first tab as selected if there are tabs available
        if (tabButtons.Length > 0)
        {
            OnTabSelected(tabButtons[0]);
        }
    }

    private void Update()
    {
        HandleControllerInput();
    }

    private void HandleControllerInput()
    {
        if (Time.time >= nextInputTime)
        {
            // Check if Right Bumper (RB) is pressed to navigate right
            if (Input.GetButtonDown("RB"))
            {
                NavigateRight();
            }
            // Check if Left Bumper (LB) is pressed to navigate left
            else if (Input.GetButtonDown("LB"))
            {
                NavigateLeft();
            }
        }
    }

    private void NavigateRight()
    {
        // Move to the next tab, looping back to the start if necessary
        currentTabIndex = (currentTabIndex + 1) % tabButtons.Length;
        OnTabSelected(tabButtons[currentTabIndex]);
        nextInputTime = Time.time + inputDelay;
    }

    private void NavigateLeft()
    {
        // Move to the previous tab, looping back to the end if necessary
        currentTabIndex = (currentTabIndex - 1 + tabButtons.Length) % tabButtons.Length;
        OnTabSelected(tabButtons[currentTabIndex]);
        nextInputTime = Time.time + inputDelay;
    }

    public void OnTabSelected(TabButton tabButton)
    {
        if (tabButton != null)
        {
            selectedTab = tabButton;
            ResetTabs();
            Debug.Log(tabActive.name);
            tabButton.swapSprite(tabActive);
            tabButton.buttonText.color = Color.yellow;

            // Set the selected tab button in the Event System
            EventSystem.current.SetSelectedGameObject(tabButton.gameObject);

            // Activate the corresponding object and deactivate others
            int index = System.Array.IndexOf(tabButtons, tabButton);
            for (int i = 0; i < ObjectsToSwap.Length; i++)
            {
                ObjectsToSwap[i].SetActive(i == index);
            }
        }
        
    }

    public void ResetTabs()
    {
        foreach (TabButton tab in tabButtons)
        {
            if (selectedTab != null && selectedTab == tab) continue;
            Debug.Log(tabIdle.name);
            tab.swapSprite(tabIdle);
            tab.buttonText.color = Color.white;
        }
    }
}
