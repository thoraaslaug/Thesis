using UnityEngine;
using System.Collections;

public class BridgeBreakSystem : MonoBehaviour
{
    public GameObject[] bridgeParts; // Assign the bridge parts in the Inspector
    public float fallDelay = 0.1f; // Delay before falling starts
    public float staggerDelay = 0.05f; // Delay between each part falling
    public float fallSpeed = 2f; // Speed at which parts fall
    public float fallDistance = 5f; // How far each piece falls down

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PreconditionTracker.hasEnteredPrecondition)
            {
                StartCoroutine(MakeBridgeFall());
            }
        }
    }

    private IEnumerator MakeBridgeFall()
    {
        yield return new WaitForSeconds(fallDelay);

        foreach (GameObject part in bridgeParts)
        {
            if (part != null)
            {
                StartCoroutine(FallDown(part));
            }
            yield return new WaitForSeconds(staggerDelay); // If you want them to fall one by one
        }
    }

    private IEnumerator FallDown(GameObject part)
    {
        Vector3 startPosition = part.transform.position;
        Vector3 targetPosition = startPosition + Vector3.down * fallDistance; // Move downward

        float elapsedTime = 0f;
        float duration = fallDistance / fallSpeed; // Controls how long it takes to fall

        while (elapsedTime < duration)
        {
            part.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it lands exactly at the final position
        part.transform.position = targetPosition;
    }
}