using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class TextHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text buttonText;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public float fadeDuration = 0.3f;

    private Coroutine fadeCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartFade(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartFade(normalColor);
    }

    private void StartFade(Color targetColor)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToColor(targetColor));
    }

    private IEnumerator FadeToColor(Color targetColor)
    {
        Color startColor = buttonText.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            buttonText.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
            yield return null;
        }

        buttonText.color = targetColor;
    }
}