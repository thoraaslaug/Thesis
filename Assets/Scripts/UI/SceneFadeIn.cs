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
            StartCoroutine(fadeFromBlack(2));
            StartCoroutine(screenFade.FadeFromBlack(fadeDuration));
        }
    }

    public IEnumerator fadeFromBlack(float duration)
    {
        yield return new WaitForSeconds(2);
    }
}