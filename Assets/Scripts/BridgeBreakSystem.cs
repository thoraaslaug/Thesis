using UnityEngine;
using System.Collections;

public class BridgeBreakSystem : MonoBehaviour
{
    public GameObject[] bridgeParts; // Assign bridge pieces in Inspector
    public float breakingDelay = 0.5f; // Delay before each plank disappears
    private int nextBridgePartIndex = 0; // Tracks which part to break next

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PreconditionTracker.hasEnteredPrecondition)
            {
                Debug.Log("Player has been in the precondition area before. Breaking bridge...");
                StartCoroutine(BreakBridge());
            }
            else
            {
                Debug.Log("Player has NOT entered the required area yet!");
            }
        }
    }

    private IEnumerator BreakBridge()
    {
        while (nextBridgePartIndex < bridgeParts.Length)
        {
            GameObject part = bridgeParts[nextBridgePartIndex];
            if (part != null)
            {
                Debug.Log("Breaking Bridge Part: " + part.name);
                part.SetActive(false);
            }

            nextBridgePartIndex++;
            yield return new WaitForSeconds(breakingDelay);
        }
    }
}