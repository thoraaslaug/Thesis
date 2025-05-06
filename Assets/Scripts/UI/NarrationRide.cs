using System.Collections;
using TMPro;
using UnityEngine;
using MalbersAnimations.Controller;

public class NarrationRide : MonoBehaviour
{
    public TextMeshProUGUI narrationText;
    public float fadeDuration = 1f;
    public float visibleDuration = 2f;
    public string[] rideLines;

    public MAnimal horse; // 🐎 Assign in inspector (this is the horse's MAnimal)

    public float movementThreshold = 0.1f; // Movement threshold to count as "riding"
    public float delayBetweenLines = 4f;   // Prevents rapid line playback

    private Coroutine currentNarration;
    private int currentLineIndex = 0;
    private bool narrationPlaying = false;
    private float timeSinceLastLine = 0f;

    private void Update()
    {
        if (horse == null || rideLines.Length == 0 || currentLineIndex >= rideLines.Length) return;

        float speed = horse.MovementAxis.magnitude;

        if (speed > movementThreshold && !narrationPlaying && timeSinceLastLine >= delayBetweenLines)
        {
            ShowNextLine();
            timeSinceLastLine = 0f;
        }

        if (!narrationPlaying)
            timeSinceLastLine += Time.deltaTime;
    }

    public void ShowNextLine()
    {
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
