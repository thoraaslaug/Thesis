using System;
using UnityEngine;
using System.Collections;

public class CharacterFader : MonoBehaviour
{
    public SkinnedMeshRenderer ghostRenderer;
    public float fadeDuration = 2f;

    private Material ghostMaterial;

   private void Start()
    {
        ghostRenderer = GetComponent<SkinnedMeshRenderer>();
        Color c = ghostRenderer.material.color;
        c.a = 0f;
        ghostRenderer.material.color = c;
    }

    public void FadeIn()
    {
        StartCoroutine(FadeRoutine());
        Debug.Log("Fade in");
    }

    public void FadeOut()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        for (float f = 0.05f; f < 1f; f += 0.05f)
        {
            Color c = ghostRenderer.material.color;
            c.a = f;
            ghostRenderer.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }
}