using System.Collections;
using TMPro;
using UnityEngine;

public class PoemDisplayManager : MonoBehaviour
{
    [Header("Text Components")]
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI bottomText;

    [Header("Poem Lines")]
    [TextArea] public string leftPoem;
    [TextArea] public string rightPoem;
    [TextArea] public string bottomPoem;
    [TextArea] public string translationPoem;


    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;  
    public float delayBetweenSections = 2.0f;  // ← longer pause before next part
    public NarrationRide narrationManager; // Assign this in the inspector
    [TextArea] public string[] postPoemLines;

    private Coroutine poemCoroutine;
    
    public AudioSource poemAudioSource;


    public void StartPoem(System.Action onComplete = null)
    {
        if (poemCoroutine != null)
            StopCoroutine(poemCoroutine);

        leftText.gameObject.SetActive(true);
        rightText.gameObject.SetActive(true);
        bottomText.gameObject.SetActive(true);

        //StartCoroutine(TypeText(translationPoem, translation));
        poemCoroutine = StartCoroutine(TypePoemSequence(onComplete));
    }


    private IEnumerator TypePoemSequence(System.Action onComplete)
    {
        leftText.text = "";
        rightText.text = "";
        bottomText.text = "";
        
        yield return StartCoroutine(TypeText(leftPoem, leftText));
        yield return new WaitForSecondsRealtime(delayBetweenSections);
        yield return StartCoroutine(TypeText(rightPoem, rightText));
        yield return new WaitForSecondsRealtime(delayBetweenSections);
        yield return StartCoroutine(TypeText(bottomPoem, bottomText));

        float audioDuration = 5f;
        if (poemAudioSource != null && poemAudioSource.clip != null)
            audioDuration = poemAudioSource.clip.length;

        yield return new WaitForSecondsRealtime(audioDuration);

        leftText.gameObject.SetActive(false);
        rightText.gameObject.SetActive(false);
        bottomText.gameObject.SetActive(false);

        if (onComplete != null)
            onComplete.Invoke();

        StartCoroutine(PlayPostPoemNarration());
    }

    
    private IEnumerator PlayPostPoemNarration()
    {
        yield return new WaitForSecondsRealtime(5f); // Wait after poem fade-out

        foreach (string line in postPoemLines)
        {
            if (narrationManager != null)
                yield return narrationManager.ShowLineRoutine(line); // Play each line
        }
    }

    private IEnumerator TypeText(string fullText, TextMeshProUGUI textTarget)
    {
        textTarget.text = "";

        foreach (char letter in fullText)
        {
            textTarget.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }
}