using System.Collections;
using UnityEngine;
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
            elapsed += Time.deltaTime;
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
            elapsed += Time.deltaTime;
            color.a = 1 - Mathf.Clamp01(elapsed / duration);
            fadePanel.color = color;
            yield return null;
        }

        fadePanel.gameObject.SetActive(false); 
    }
}