using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public Image fadePanel;
    
    public IEnumerator FadeToBlack(float duration)
    {
        fadePanel.gameObject.SetActive(true); 
        float elapsed = 0f;
        Color color = fadePanel.color;
        color.a = 0f;
        fadePanel.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            fadePanel.color = color;
            yield return null;
        }

        // Optional: keep active after fade or disable again depending on your scene needs
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        fadePanel.gameObject.SetActive(true); 
        float elapsed = 0f;
        Color color = fadePanel.color;
        color.a = 1f;
        fadePanel.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = 1 - Mathf.Clamp01(elapsed / duration);
            fadePanel.color = color;
            yield return null;
        }

        fadePanel.gameObject.SetActive(false); 
    }

    public void Fade()
    {
        StartCoroutine(FadeToBlack(1f));
    }
    public void ReturnFade()
    {
        StartCoroutine(FadeFromBlack(1f));
    }
    
    public void FadeAndReturnToMainMenu(string MainMenu, float delay = 2f)
    {
        StartCoroutine(FadeAndLoadScene(MainMenu, delay));
    }

    public IEnumerator FadeAndLoadScene(string sceneName, float delay)
    {
        // Optional: small delay before starting fade
        /*if (delay > 0)
            yield return new WaitForSeconds(delay);*/

        // Start fade
        yield return StartCoroutine(FadeToBlack(1f));

        // Wait while fully black
        yield return new WaitForSeconds(5f);  // <- THIS is your “stay black” time

        // Optional: ensure final black frame renders
        yield return new WaitForEndOfFrame();

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }

    
    public void SetBlackInstantly()
    {
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
        }
    }

    
}