using UnityEngine;
using System.Collections;

public class IrisFadeEffect : MonoBehaviour
{
    public Material irisMaterial; // Material using IrisMaskShader
    public float irisOpenSpeed = 2f; // Speed of iris opening

    private void Start()
    {
        StartCoroutine(PlayIrisFade());
    }

    private IEnumerator PlayIrisFade()
    {
        float elapsedTime = 0f;
        float irisSize = 0.01f; // Start with a tiny hole

        // ✅ Ensure the iris starts small and expands outward
        irisMaterial.SetFloat("_IrisSize", irisSize);

        while (elapsedTime < irisOpenSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / irisOpenSpeed;

            // 🔥 Expand the iris outward smoothly
            irisSize = Mathf.Lerp(0.01f, 1.0f, t);
            irisMaterial.SetFloat("_IrisSize", irisSize);

            yield return null;
        }
    }
}