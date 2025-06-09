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


    
    public ScreenFade screenFade;

    public void StartNarrationWithAudioAndFades(string[] lines, AudioClip[] clips, float delayBetweenLines = 2f)
    {
        StartCoroutine(PlayNarrationWithFades(lines, clips, delayBetweenLines));
    }

    private IEnumerator PlayNarrationWithFades(string[] lines, AudioClip[] clips, float delayBetweenLines)
    {
        isNarrating = true;

        for (int i = 0; i < lines.Length; i++)
        {
            narrationText.text = lines[i];

            // ✨ Fade text in
            yield return StartCoroutine(FadeTextAlpha(0f, 1f, 1f));

            if (i < clips.Length && audioSource != null)
            {
                audioSource.clip = clips[i];
                audioSource.Play();
                yield return new WaitForSeconds(clips[i].length);
            }
            else
            {
                yield return new WaitForSeconds(delayBetweenLines);
            }

            // 🔚 Skip fade after last line
            if (i == lines.Length - 1) break;

            // 🌓 Fade text out and screen to black
            Coroutine fadeTextOut = StartCoroutine(FadeTextAlpha(1f, 0f, 1f));
            if (screenFade != null)
            {
                yield return screenFade.FadeToBlack(1f);
            }
            yield return fadeTextOut;

            yield return new WaitForSeconds(1f);

            // 🌄 Fade from black before next line
            if (screenFade != null)
            {
                yield return screenFade.FadeFromBlack(1f);
            }

            yield return new WaitForSeconds(0.3f);
        }

        // Final cleanup for last line (text fades out normally)
        yield return StartCoroutine(FadeTextAlpha(1f, 0f, 1f));
        narrationText.text = "";
        isNarrating = false;

        onNarrationComplete?.Invoke();
    }



    private IEnumerator FadeTextAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = narrationText.color;
        color.a = from;
        narrationText.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(from, to, elapsed / duration);
            narrationText.color = color;
            yield return null;
        }

        color.a = to;
        narrationText.color = color;
    }


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
            yield return new WaitForSecondsRealtime(startDelay); // ⏱️ Optional delay before narration starts

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            AudioClip clip = (audioClips != null && i < audioClips.Length) ? audioClips[i] : null;

            // Set text and fade in
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
            float waitTime = 2f; // default
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
                waitTime = clip.length;
            }

            // Wait while audio plays
            yield return new WaitForSecondsRealtime(waitTime);

            // Fade out AFTER audio ends
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                narrationText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }

            narrationText.alpha = 0f;
            narrationText.text = "";

            // Delay BETWEEN lines (while no audio or text is showing)
            if (delayBetweenLines > 0f)
                yield return new WaitForSecondsRealtime(delayBetweenLines);
        }

        onNarrationComplete?.Invoke();
    }
}