using TMPro;
using UnityEngine;
using System.Collections;

public class TextPopUpManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float fadeDuration = 1f;
    public float visibleDuration = 2f;

    public void ShowMessage(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        messageText.text = message;
        messageText.alpha = 1f;

        yield return new WaitForSeconds(visibleDuration);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            messageText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        messageText.text = "";
    }
    
    public void HideMessage()
    {
        StopAllCoroutines();
        messageText.text = "";
        messageText.alpha = 0f;
    }
}