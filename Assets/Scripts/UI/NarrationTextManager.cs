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

    private Coroutine narrationCoroutine;
    
    public AudioSource audioSource; // Drag your AudioSource in the inspector


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
    
    public void StartNarrationWithAudio(string[] lines, AudioClip[] audioClips, float delayBetweenLines = 0f, float startDelay = 0f)
    {
        if (narrationCoroutine != null)
            StopCoroutine(narrationCoroutine);

        narrationCoroutine = StartCoroutine(PlayNarrationWithAudio(lines, audioClips, delayBetweenLines, startDelay));
    }



    private IEnumerator PlayNarrationWithAudio(string[] lines, AudioClip[] audioClips, float delayBetweenLines, float startDelay)
    {
        if (startDelay > 0f)
            yield return new WaitForSecondsRealtime(startDelay); // ⏱️ Delay before narration starts

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            AudioClip clip = (audioClips != null && i < audioClips.Length) ? audioClips[i] : null;

            // Fade in
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

            // Play audio
            float waitTime = 2f;
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
                waitTime = clip.length;
            }
            yield return new WaitForSecondsRealtime(waitTime);

            if (delayBetweenLines > 0f)
                yield return new WaitForSecondsRealtime(delayBetweenLines);

            // Fade out
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                narrationText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }

            narrationText.text = "";
        }

        onNarrationComplete?.Invoke();
    }



    
}