using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [Tooltip("Name of the scene to load")]
    public string sceneToLoad = "SampleScene"; // Replace with your actual scene name
    public ScreenFade screenFade;                // Assign your fade script in the Inspector
    public float fadeDuration = 1f;

    private bool hasStarted = false;
    public TextMeshProUGUI[] menuTexts;  

    public void StartPlay()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(FadeAndLoad());
        }
    }

    private IEnumerator FadeAndLoad()
    {
        // Fade out text
        foreach (var text in menuTexts)
        {
            StartCoroutine(FadeTextOut(text));
        }

        // Fade screen to black
        if (screenFade != null)
        {
            yield return screenFade.FadeToBlack(fadeDuration);
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeTextOut(TextMeshProUGUI text)
    {
        float t = 0f;
        Color original = text.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            text.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }

        text.color = new Color(original.r, original.g, original.b, 0f);
    }
}