using UnityEngine;
using UnityEngine.SceneManagement;

public class InactivityManager : MonoBehaviour
{
    public float inactivityTimeout = 20f;
    private float inactivityTimer = 0f;
    private bool isPaused = false;

    private static InactivityManager instance;

    private void Awake()
    {
        // Singleton setup (optional but useful)
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (isPaused) return;

        if (Input.anyKey || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            inactivityTimer = 0f;
        }
        else
        {
            inactivityTimer += Time.deltaTime;

            if (inactivityTimer >= inactivityTimeout)
            {
                GoToMainMenu();
            }
        }
    }

    public void PauseInactivity() => isPaused = true;
    public void ResumeInactivity() => isPaused = false;
    public void ResetTimer() => inactivityTimer = 0f;

    private void GoToMainMenu()
    {
        Destroy(gameObject);  // Prevent conflicts or duplication
        SceneManager.LoadScene("MainMenu");  // Your actual menu scene name
    }
}
