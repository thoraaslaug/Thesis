using UnityEngine;
using System.Collections;

public class IrisFadeEffect : MonoBehaviour
{
    public Material irisMaterial; // Material using IrisCutout Shader
    public float irisOpenSpeed = 2f; // Speed of iris opening
    public Transform player;  // assign this in Inspector

    private void Start()
    {
        StartCoroutine(PlayIrisFade());
    }

    private IEnumerator PlayIrisFade()
    {
        float elapsedTime = 0f;
        float irisSize = 0.002f;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(player.position);
        irisMaterial.SetVector("_IrisCenter", new Vector4(viewPos.x, viewPos.y, 0f, 0f));

        irisMaterial.SetFloat("_IrisSize", irisSize);

        while (elapsedTime < irisOpenSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / irisOpenSpeed;

            irisSize = Mathf.Lerp(0.002f, 2.0f, t);
            irisMaterial.SetFloat("_IrisSize", irisSize);

            yield return null;
        }
    }
}