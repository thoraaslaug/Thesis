using UnityEngine;
using System.Collections;

public class IrisFadeEffect : MonoBehaviour
{
    public Material irisMaterial; // Material using IrisCutout Shader
    public float irisOpenSpeed = 2f; // Speed of iris opening

    private void Start()
    {
        StartCoroutine(PlayIrisFade());
    }

    private IEnumerator PlayIrisFade()
    {
        float elapsedTime = 0f;
        float irisSize = 0.002f; // Start with an even smaller transparent hole

        // ✅ Ensure the iris starts small and expands outward
        irisMaterial.SetFloat("_IrisSize", irisSize);

        while (elapsedTime < irisOpenSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / irisOpenSpeed;

            // 🔥 Expand the iris outward smoothly
            irisSize = Mathf.Lerp(0.002f, 1.0f, t); // Smaller start value
            irisMaterial.SetFloat("_IrisSize", irisSize);

            yield return null;
        }
    }
}