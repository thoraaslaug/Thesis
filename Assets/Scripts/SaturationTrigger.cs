using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SaturationTrigger : MonoBehaviour
{
    public Volume volume; // Assign your camera Volume here in the inspector
    public float targetSaturation = 20f; // how saturated it should end up
    public float saturationDuration = 2f; // how long the effect takes

    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        if (volume != null)
        {
            volume.profile.TryGet(out colorAdjustments);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && colorAdjustments != null)
        {
            StartCoroutine(IncreaseSaturationGradually());
        }
    }

    private IEnumerator IncreaseSaturationGradually()
    {
        float elapsed = 0f;
        float initialSaturation = colorAdjustments.saturation.value;

        while (elapsed < saturationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / saturationDuration;
            colorAdjustments.saturation.value = Mathf.Lerp(initialSaturation, targetSaturation, t);
            yield return null;
        }

        colorAdjustments.saturation.value = targetSaturation;
    }
}