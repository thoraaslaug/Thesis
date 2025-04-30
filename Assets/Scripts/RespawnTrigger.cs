using UnityEngine;
using MalbersAnimations.Controller;

public class RespawnTrigger : MonoBehaviour
{
    [Tooltip("Tag that identifies water")]
    public string waterTag = "Water";

    [Tooltip("Where the horse should respawn")]
    public Transform respawnPoint;

    [Tooltip("Optional delay before teleporting (in seconds)")]
    public float delay = 0f;

    private MAnimal animal;

    private void Start()
    {
        animal = GetComponent<MAnimal>();

        if (animal == null)
        {
            Debug.LogError("No MAnimal component found on this GameObject!");
        }

        if (respawnPoint == null)
        {
            Debug.LogError("Respawn point is not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(waterTag))
        {
            if (delay > 0)
                Invoke(nameof(DoRespawn), delay);
            else
                DoRespawn();
        }
    }

    private void DoRespawn()
    {
        if (animal != null && respawnPoint != null)
        {
            animal.Teleport(respawnPoint.position); // This is the public method
            animal.transform.rotation = respawnPoint.rotation;
            animal.ResetController();
        }
    }
}