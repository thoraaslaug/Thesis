using System.Collections;
using UnityEngine;
using TMPro;

public class NarrationTextManager : MonoBehaviour
{
    public TextMeshProUGUI narrationText;
    public float textDuration = 3f;
    public float fadeDuration = 1f;

    public void StartNarration(string[] lines)
    {
        StartCoroutine(PlayNarration(lines));
    }

    private IEnumerator PlayNarration(string[] lines)
    {
        foreach (string line in lines)
        {
            yield return StartCoroutine(ShowLine(line));
            yield return new WaitForSeconds(textDuration);
        }

        narrationText.text = "";
        narrationText.alpha = 0f;
    }

    private IEnumerator ShowLine(string line)
    {
        narrationText.text = line;

        // Fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            narrationText.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        narrationText.alpha = 1f;

        // Hold full visibility for duration
        yield return new WaitForSeconds(textDuration);

        // Fade out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            narrationText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        narrationText.alpha = 0f;
    }
    
}