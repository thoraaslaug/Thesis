using System.Collections;
using UnityEngine;

public class SceneFadeIn : MonoBehaviour
{
    public ScreenFade screenFade;
    public float fadeDuration = 1.5f;

    private void Start()
    {
        if (screenFade != null)
        {
            StartCoroutine(screenFade.FadeFromBlack(fadeDuration));
        }
    }
}