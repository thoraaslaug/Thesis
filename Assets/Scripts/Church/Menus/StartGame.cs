using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [Tooltip("Name of the scene to load")]
    public string sceneToLoad = "SampleScene";
    public ScreenFade screenFade;
    public float fadeDuration = 1f;
    private bool hasStarted = false;

    public TextMeshProUGUI[] menuTexts;

    private void Start()
    {
        foreach (var text in menuTexts)
        {
            Color c = text.color;
            text.color = new Color(c.r, c.g, c.b, 0f); // Start fully transparent
        }

        StartCoroutine(FadeInMenuTexts());
    }

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
        foreach (var text in menuTexts)
        {
            StartCoroutine(FadeTextOut(text));
        }

        if (screenFade != null)
        {
            yield return screenFade.FadeToBlack(fadeDuration);
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeInMenuTexts()
    {
        yield return new WaitForSeconds(2f); // Wait before fade-in

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);

            foreach (var text in menuTexts)
            {
                Color c = text.color;
                text.color = new Color(c.r, c.g, c.b, alpha);
            }

            yield return null;
        }

        // Just to ensure it's fully visible
        foreach (var text in menuTexts)
        {
            Color c = text.color;
            text.color = new Color(c.r, c.g, c.b, 1f);
        }
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
