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

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;  
    public float delayBetweenSections = 2.0f;  // ← longer pause before next part
    public NarrationRide narrationManager; // Assign this in the inspector
    [TextArea] public string[] postPoemLines;

    private Coroutine poemCoroutine;

    public void StartPoem()
    {
        if (poemCoroutine != null)
            StopCoroutine(poemCoroutine);

        poemCoroutine = StartCoroutine(TypePoemSequence());
    }

    private IEnumerator TypePoemSequence()
    {
        leftText.text = "";
        rightText.text = "";
        bottomText.text = "";

        // Type LEFT
        yield return StartCoroutine(TypeText(leftPoem, leftText));

        // Pause
        yield return new WaitForSecondsRealtime(delayBetweenSections);

        // Type RIGHT
        yield return StartCoroutine(TypeText(rightPoem, rightText));

        // Pause
        yield return new WaitForSecondsRealtime(delayBetweenSections);

        // Type BOTTOM
        yield return StartCoroutine(TypeText(bottomPoem, bottomText));
        
        yield return new WaitForSecondsRealtime(3.0f);

        // 🚫 Set texts inactive after poem is fully shown
        leftText.gameObject.SetActive(false);
        rightText.gameObject.SetActive(false);
        bottomText.gameObject.SetActive(false);
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