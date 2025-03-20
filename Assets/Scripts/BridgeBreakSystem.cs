using System.Collections;
using UnityEngine;
   

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
                BreakBridge();
            }
            else
            {
                Debug.Log("Player has NOT entered the required area yet!");
            }
        }
    }
    
    private void BreakBridge()
    {
        foreach (GameObject part in bridgeParts)
        {
            if (part != null)
            {
                Debug.Log("Breaking Bridge Part: " + part.name);
                part.SetActive(false);
            }
        }
    }

    /*private IEnumerator BreakBridge()
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
    }*/
}