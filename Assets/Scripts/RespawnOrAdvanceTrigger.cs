using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using MalbersAnimations.Controller;

public class RespawnOrAdvanceTrigger : MonoBehaviour
{
    [Tooltip("Tag that identifies water")] public string waterTag = "Water";

    [Tooltip("Where the horse should respawn before precondition is met")]
    public Transform defaultRespawnPoint;

    [Tooltip("Where the horse should respawn after precondition is met")]
    public Transform alternateRespawnPoint;

    [Tooltip("Scene to load when bridge breaks and player falls")]
    public string nextSceneName = "Interior";

    [Tooltip("Optional delay before handling respawn/transition")]
    public float delay = 0f;

    private MAnimal animal;

    public ScreenFade ScreenFade;


    private void Start()
    {
        animal = GetComponent<MAnimal>();

        if (animal == null)
            Debug.LogError("❌ No MAnimal component found!");

        if (defaultRespawnPoint == null)
            Debug.LogError("❌ Default respawn point not assigned!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(waterTag))
        {
            if (delay > 0)
                Invoke(nameof(HandleFall), delay);
            else
                StartCoroutine(HandleFall());
        }
    }

    private IEnumerator HandleFall()
    {
        if (BridgeBreakSystem.HasBroken && BridgeBreakSystem.PlayerIsOnBridge)
        {
            yield return ScreenFade.FadeToBlack(2f);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Interior1");
            yield return new WaitForSeconds(5f);
            // ⛔ STOP here! We don't want to respawn when switching scenes
            //yield break;
        }

        // ✅ Otherwise: respawn locally
        Transform targetPoint = PreconditionTracker.hasEnteredPrecondition
            ? alternateRespawnPoint
            : defaultRespawnPoint;

        if (targetPoint != null && animal != null)
        {
            animal.Teleport(targetPoint.position);
            animal.transform.rotation = targetPoint.rotation;
            animal.ResetController();
            Debug.Log($"🔁 Respawned at {(PreconditionTracker.hasEnteredPrecondition ? "alternate" : "default")} point.");
        }
        else
        {
            Debug.LogWarning("⚠️ Missing animal or target respawn point.");
        }
    }

}
