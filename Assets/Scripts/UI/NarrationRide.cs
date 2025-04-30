using System.Collections;
using TMPro;
using UnityEngine;

public class NarrationRide : MonoBehaviour
{
    public TextMeshProUGUI narrationText;
    public float fadeDuration = 1f;
    public float visibleDuration = 2f;
    public string[] rideLines; // ✅ All your ride narration lines

    [HideInInspector]
    public bool isRiding = false;

    private Coroutine currentNarration;
    private int currentLineIndex = 0;
    private bool narrationPlaying = false;

    private void Update()
    {
        if (isRiding && !narrationPlaying)
        {
            ShowNextLine();
        }
    }

    public void ShowNextLine()
    {
        if (rideLines.Length == 0 || currentLineIndex >= rideLines.Length)
            return;

        if (currentNarration != null)
            StopCoroutine(currentNarration);

        currentNarration = StartCoroutine(ShowLineRoutine(rideLines[currentLineIndex]));
        currentLineIndex++;
    }

    public IEnumerator ShowLineRoutine(string line)
    {
        narrationPlaying = true;

        narrationText.text = line;
        narrationText.alpha = 0f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            narrationText.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        narrationText.alpha = 1f;

        yield return new WaitForSecondsRealtime(visibleDuration);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            narrationText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        narrationText.text = "";
        narrationText.alpha = 0f;
        
        yield return new WaitForSecondsRealtime(2f);

        narrationPlaying = false;
    }

    public void ResetNarration()
    {
        if (currentNarration != null)
            StopCoroutine(currentNarration);

        currentLineIndex = 0;
        narrationPlaying = false;
        narrationText.text = "";
        narrationText.alpha = 0f;
    }
}
