using UnityEngine;
using UnityEngine.VFX;

public class SnowstormTrigger : MonoBehaviour
{
    public VisualEffect snowVFX;  // Assign the VFX Graph object
    public float normalRate = 100f; // Default particle spawn rate
    public float stormRate = 1000f; // Increased spawn rate during storm
    public float normalSpeed = 2f; // Normal falling speed
    public float stormSpeed = 8f; // Faster falling speed in storm
    public float transitionTime = 2f; // Time to blend into storm

    private bool isStormActive = false;
    private float currentRate;
    private float currentSpeed;

    void Start()
    {
        if (snowVFX == null)
        {
            return;
        }

        // Set initial values
        currentRate = normalRate;
        currentSpeed = normalSpeed;
        snowVFX.SetFloat("SpawnRate", currentRate);
        snowVFX.SetFloat("SnowSpeed", currentSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isStormActive)
        {
            isStormActive = true;
            StopAllCoroutines();
            StartCoroutine(ChangeSnowstorm(stormRate, stormSpeed));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isStormActive)
        {
           
            isStormActive = false;
            StopAllCoroutines();
            StartCoroutine(ChangeSnowstorm(normalRate, normalSpeed));
        }
    }


    private System.Collections.IEnumerator ChangeSnowstorm(float targetRate, float targetSpeed)
    {
        float elapsedTime = 0f;
        float startRate = currentRate;
        float startSpeed = currentSpeed;

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;
            currentRate = Mathf.Lerp(startRate, targetRate, t);
            currentSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);

            snowVFX.SetFloat("SpawnRate", currentRate);
            snowVFX.SetFloat("SnowSpeed", currentSpeed);

            yield return null;
        }
    }
}
