using UnityEngine;
using System.Collections;

public class BridgeBreakSystem : MonoBehaviour
{
    public GameObject[] bridgeParts; // Assign all bridge parts
    public float fallDelay = 0.1f;
    public float staggerDelay = 0.05f;
    public float fallSpeed = 2f;
    public float fallDistance = 5f;
    public GameObject firstRock; // Assign one rock to fall first

    private bool hasPlayerEnteredOnce = false;
    private bool hasBroken = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Trigger the first rock only once
            if (!hasPlayerEnteredOnce)
            {
                hasPlayerEnteredOnce = true;
                if (firstRock != null)
                {
                    Debug.Log("Player first entered bridge. Dropping one rock.");
                    StartCoroutine(FallDown(firstRock));
                }
            }

            // Trigger full bridge collapse if the condition is met
            if (PreconditionTracker.hasEnteredPrecondition && !hasBroken)
            {
                hasBroken = true;
                Debug.Log("Precondition met. Breaking full bridge.");
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
            yield return new WaitForSeconds(staggerDelay);
        }
    }

    private IEnumerator FallDown(GameObject part)
    {
        Vector3 startPosition = part.transform.position;
        Vector3 targetPosition = startPosition + Vector3.down * fallDistance;

        float elapsedTime = 0f;
        float duration = fallDistance / fallSpeed;

        while (elapsedTime < duration)
        {
            part.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        part.transform.position = targetPosition;
    }
}
