using MalbersAnimations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MalbersAnimations.Controller; // For MInput
using MalbersAnimations.InputSystem;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public MInput inputSource; // Assign the player's input component here
    public bool isPaused = false;

    private void Start()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu?.SetActive(true);
        isPaused = true;

        if (inputSource != null)
            inputSource.Enable(false); // Disable all input from Malbers Input
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu?.SetActive(false);
        isPaused = false;

        if (inputSource != null)
            inputSource.Enable(true); // Re-enable input
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}