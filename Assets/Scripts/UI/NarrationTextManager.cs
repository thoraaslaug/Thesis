using System.Collections;
using UnityEngine;
using TMPro;

public class NarrationTextManager : MonoBehaviour
{
    public TextMeshProUGUI narrationText;
    public float textDuration = 3f;
    public float fadeDuration = 1f;
    public TextPopUpManager popupManager; // assign in inspector if needed
    public System.Action onNarrationComplete;
    [HideInInspector] public bool isNarrating = false;


    public void StartNarration(string[] lines)
    {
        if (popupManager != null)
        {
            popupManager.HideMessage(); // 👈 Clear any prompt text
        }

        StartCoroutine(PlayNarration(lines));
    }

    private IEnumerator PlayNarration(string[] lines)
    {
        isNarrating = true;

        foreach (string line in lines)
        {
            yield return StartCoroutine(ShowLine(line));
            yield return new WaitForSeconds(textDuration);
        }

        narrationText.text = "";
        narrationText.alpha = 0f;

        if (onNarrationComplete != null)
        {
            onNarrationComplete.Invoke();
        }
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