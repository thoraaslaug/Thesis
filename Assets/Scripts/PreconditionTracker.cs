using UnityEngine;

public class PreconditionTracker : MonoBehaviour
{
    public static bool hasEnteredPrecondition = false; // Shared across scripts
    public SnowstormTrigger snowstormTrigger; // Reference to the Snowstorm System

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEnteredPrecondition)
        {
            Debug.Log("✅ Player has entered the precondition area. Snowstorm begins!");
            hasEnteredPrecondition = true;

            // Start the snowstorm
            if (snowstormTrigger != null)
            {
                snowstormTrigger.StartSnowstorm();
            }
            else
            {
                Debug.LogWarning("⚠ No SnowstormTrigger assigned to PreconditionTracker.");
            }
        }
    }
}