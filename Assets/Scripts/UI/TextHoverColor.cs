using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text buttonText; // assign your text component
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }

    void Start()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();
        buttonText.color = normalColor;
    }
}