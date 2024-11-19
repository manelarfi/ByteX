using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // public Slider difficultySlider;       // Slider for difficulty level
    // public TMP_Text difficultyText;       // Text for displaying difficulty description
    // public ConfigSO config;
    // public GameObject selectedButton;

    // private float inputDelay = 0.2f;      // Delay for input to prevent quick toggling
    // private float nextInputTime = 0f;     // Timer for input delay
    // private int maxDifficultyLevel = 5;
    // private string[] difficultyLevels = { "Super Easy", "Easy", "Intermediate", "Hard", "Legendary" };

    // private void Start()
    // {
    //     // Initialize slider and text
    //     difficultySlider.minValue = 1;
    //     difficultySlider.maxValue = maxDifficultyLevel;
    //     difficultySlider.value = config.level; // Starting at level 1
    //     UpdateDifficultyText();
    // }
    
    // private void Update()
    // {
    //     if (Time.time >= nextInputTime)  // Check if enough time has passed
    //     {
    //         // Only change difficulty if the selected object is the desired button
    //         if (EventSystem.current.currentSelectedGameObject == selectedButton)
    //         {
    //             HandleInput();
    //         }
    //     }
    // }

    // private void HandleInput()
    // {
    //     float horizontal = Input.GetAxis("Horizontal");

    //     if (horizontal > 0.8f || Input.GetKeyDown(KeyCode.RightArrow))
    //     {
    //         ChangeDifficulty(1); // Increase difficulty
    //     }
    //     else if (horizontal < -0.8f || Input.GetKeyDown(KeyCode.LeftArrow))
    //     {
    //         ChangeDifficulty(-1); // Decrease difficulty
    //     }
    // }

    // private void ChangeDifficulty(int direction)
    // {
    //     // Update slider value in a circular manner
    //     difficultySlider.value += direction;

    //     if (difficultySlider.value > maxDifficultyLevel)
    //     {
    //         difficultySlider.value = 1; // Wrap to the first level
    //     }
    //     else if (difficultySlider.value < 1)
    //     {
    //         difficultySlider.value = maxDifficultyLevel; // Wrap to the last level
    //     }

    //     nextInputTime = Time.time + inputDelay; // Set the next input time
    //     UpdateDifficultyText(); // Update difficulty text to match the new level
    // }

    // private void UpdateDifficultyText()
    // {
    //     // Get the current slider value and use it to set the text
    //     int levelIndex = (int)difficultySlider.value - 1;
    //     difficultyText.text = difficultyLevels[levelIndex];
    // }

    // public void UpdateConfig()
    // {
    //     config.level = (int)difficultySlider.value;
    // }
}
