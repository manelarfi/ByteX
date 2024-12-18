using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamepadLayoutManager : MonoBehaviour
{
    // Control mappings
    public string upButton;
    public string downButton;
    public string leftButton;
    public string rightButton;

    public string submitButton;
    public string cancelButton;
    public string leftActionButton;
    public string rightActionButton;

    private bool isListeningForInput = false;
    private string controlToAssign;
    private void Update()
    {
        if (isListeningForInput)
        {
            DetectInput();
        }
    }

    public void StartListeningForInput(string controlName)
    {
        // Set the control that will be assigned the next input
        controlToAssign = controlName;
        isListeningForInput = true;
        Debug.Log($"Listening for input to assign to: {controlName}");
    }

    private void DetectInput()
    {
        // Check for joystick button presses
        for (int i = 0; i < 20; i++) // Adjust range if your controller has more buttons
        {
            if (Input.GetKeyDown($"joystick button {i}"))
            {
                AssignControl($"joystick button {i}");
                return;
            }
        }

        // Check for joystick axes (for up, down, left, right)
        if (Input.GetAxis("Horizontal") > 0.5f)
        {
            AssignControl("Horizontal");
        }
        else if (Input.GetAxis("Horizontal") < -0.5f)
        {
            AssignControl("Horizontal");
        }
        else if (Input.GetAxis("Vertical") > 0.5f)
        {
            AssignControl("Vertical");
        }
        else if (Input.GetAxis("Vertical") < -0.5f)
        {
            AssignControl("Vertical");
        }
    }

    private void AssignControl(string input)
    {
        isListeningForInput = false;

        switch (controlToAssign)
        {
            case "up":
                upButton = input;
                Debug.Log($"Assigned {input} to Up");
                break;
            case "down":
                downButton = input;
                Debug.Log($"Assigned {input} to Down");
                break;
            case "left":
                leftButton = input;
                Debug.Log($"Assigned {input} to Left");
                break;
            case "right":
                rightButton = input;
                Debug.Log($"Assigned {input} to Right");
                break;
            case "submit":
                submitButton = input;
                Debug.Log($"Assigned {input} to Submit");
                break;
            case "cancel":
                cancelButton = input;
                Debug.Log($"Assigned {input} to Cancel");
                break;
            case "leftAction":
                leftActionButton = input;
                Debug.Log($"Assigned {input} to Left Action");
                break;
            case "rightAction":
                rightActionButton = input;
                Debug.Log($"Assigned {input} to Right Action");
                break;
            default:
                Debug.LogWarning($"Unknown control: {controlToAssign}");
                break;
        }
    }

    public void PrintCurrentMappings()
    {
        Debug.Log("Current Control Mappings:");
        Debug.Log($"Up: {upButton}");
        Debug.Log($"Down: {downButton}");
        Debug.Log($"Left: {leftButton}");
        Debug.Log($"Right: {rightButton}");
        Debug.Log($"Submit: {submitButton}");
        Debug.Log($"Cancel: {cancelButton}");
        Debug.Log($"Left Action: {leftActionButton}");
        Debug.Log($"Right Action: {rightActionButton}");
    }

    public void ResetToDefaultLayout()
    {
        // Reset all mappings to defaults
        upButton = "Vertical";
        downButton = "Vertical";
        leftButton = "Horizontal";
        rightButton = "Horizontal";

        submitButton = "joystick button 0"; // A button
        cancelButton = "joystick button 1"; // B button
        leftActionButton = "joystick button 4"; // Left bumper
        rightActionButton = "joystick button 5"; // Right bumper

        Debug.Log("Default layout restored.");
    }
}