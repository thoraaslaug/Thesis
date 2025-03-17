using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BridgeBreakSystem : MonoBehaviour
{
    public GameObject[] bridgeParts; // Assign bridge planks/pieces in the Inspector
    public Transform preconditionArea; // Assign a transform representing the required area
    public float breakingDelay = 0.5f; // Delay before each plank disappears
    private bool hasEnteredPrecondition = false; // Tracks if player has been in the area

    private int nextBridgePartIndex = 0; // Keeps track of which part should break next

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!hasEnteredPrecondition)
            {
                // Check if the player is near the precondition area
                float distance = Vector3.Distance(other.transform.position, preconditionArea.position);
                if (distance < 5f) // Adjust range as needed
                {
                    Debug.Log(" Player has entered the precondition area.");
                    hasEnteredPrecondition = true;
                }
            }
            else
            {
                // If the player has already been to the precondition area, start breaking the bridge
                StartCoroutine(BreakBridge());
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
                Debug.Log(" Breaking Bridge Part: " + part.name);
                part.SetActive(false);
            }

            nextBridgePartIndex++;
            yield return new WaitForSeconds(breakingDelay); // Wait before breaking the next part
        }
    }
}