using System.Collections;
using TMPro;
using UnityEngine;
using MalbersAnimations.Controller;
using MalbersAnimations.HAP;

public class NarrationRide : MonoBehaviour
{
    public TextMeshProUGUI narrationText;
    public float fadeDuration = 1f;
    public float visibleDuration = 2f;
    public string[] rideLines;
    public MRider rider;
    public MAnimal horse; // 🐎 Assign in inspector (this is the horse's MAnimal)
    public float movementThreshold = 0.1f; // Movement threshold to count as "riding"
    public float delayBetweenLines = 4f;   // Prevents rapid line playback

    private Coroutine currentNarration;
    private int currentLineIndex = 0;
    private bool narrationPlaying = false;
    private float timeSinceLastLine = 0f;
    
    public AudioSource audioSource; // Assign in Inspector
    public AudioClip[] rideAudio;   // Match 1:1 with rideLines

    private void Update()
    {
        if (horse == null || rideLines.Length == 0 || currentLineIndex >= rideLines.Length) return;

        if (rider != null && rider.IsRiding)
        {
            float speed = horse.MovementAxis.magnitude;

            if (speed > movementThreshold && !narrationPlaying && timeSinceLastLine >= delayBetweenLines)
            {
                ShowNextLine();
                timeSinceLastLine = 0f;
            }

            if (!narrationPlaying)
                timeSinceLastLine += Time.deltaTime;
        }
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

        // 🔊 Play corresponding audio
        if (currentLineIndex < rideAudio.Length && rideAudio[currentLineIndex] != null)
            audioSource.PlayOneShot(rideAudio[currentLineIndex]);

        // 📝 Show text
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

        // ⏳ Wait for the audio to finish or a fallback duration
        float waitTime = (currentLineIndex < rideAudio.Length && rideAudio[currentLineIndex] != null)
            ? rideAudio[currentLineIndex].length
            : visibleDuration;

        yield return new WaitForSecondsRealtime(waitTime);

        // 🔻 Fade out
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
