using UnityEngine;

public class PreconditionTracker : MonoBehaviour
{
    public static bool hasEnteredPrecondition = false; // ✅ Shared across scripts

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the precondition area.");
            hasEnteredPrecondition = true;
        }
    }
}