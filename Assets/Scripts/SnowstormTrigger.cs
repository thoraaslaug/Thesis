using StarterAssets; 
using UnityEngine;
using UnityEngine.VFX;

public class SnowstormTrigger : MonoBehaviour
{
    public VisualEffect snowVFX;  
    public float normalRate = 100f; 
    public float stormRate = 1000f;
    public float normalSpeed = 2f;
    public float stormSpeed = 8f;
    public float transitionTime = 2f;

    public ThirdPersonController playerController; 
    public HorseController horseController; // ✅ Reference to HorseController

    public float normalMoveSpeed = 2.0f;
    public float normalSprintSpeed = 5.335f;
    public float stormMoveSpeed = 0.8f;
    public float stormSprintSpeed = 2.0f;

    private bool isStormActive = false;
    private float currentRate;
    private float currentSpeed;

    void Start()
    {
        if (snowVFX == null || playerController == null || horseController == null)
        {
            Debug.LogError("❌ Missing Visual Effect, PlayerController, or HorseController reference!");
            return;
        }

        currentRate = normalRate;
        currentSpeed = normalSpeed;
        snowVFX.SetFloat("SpawnRate", currentRate);
        snowVFX.SetFloat("SnowSpeed", currentSpeed);
    }

    public void StartSnowstorm()
    {
        if (!isStormActive)
        {
            Debug.Log("❄ Snowstorm begins! Player and horse struggle.");
            isStormActive = true;
            StopAllCoroutines();
            StartCoroutine(ChangeSnowstorm(stormRate, stormSpeed));

            // Reduce player movement speed
            playerController.MoveSpeed = stormMoveSpeed;
            playerController.SprintSpeed = stormSprintSpeed;

            // ✅ Make the horse harder to control
            horseController.EnterSnowstorm();
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