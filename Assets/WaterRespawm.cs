using UnityEngine;

public class WaterRespawn : MonoBehaviour
{
    public Transform spawnPoint; // Assign this in the Inspector
    public ParticleSystem particles;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the Player has the "Player" tag
        {
            Debug.Log("Player touched water! Respawning...");

            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false; // Disable before teleporting
                other.transform.position = spawnPoint.position;
                particles.Clear();
                controller.enabled = true; // Re-enable after moving
            }
            else
            {
                other.transform.position = spawnPoint.position; // Fallback if no CharacterController
                
            }

            // Reset Velocity if Rigidbody exists
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; // Stop movement momentum
            }
        }
    }
}