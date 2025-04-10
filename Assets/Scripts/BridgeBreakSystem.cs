using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BridgeBreakSystem : MonoBehaviour
{
    public GameObject[] bridgeParts;
    public float dropCheckInterval = 0.2f;
    public float fallSpeed = 2f;
    public float fallDistance = 5f;
    public float dropBehindDistance = 1.5f;

    public GameObject firstRock;
    private bool hasPlayerEnteredOnce = false;
    public static bool HasBroken { get; private set; }

    private GameObject player;
    private HashSet<GameObject> droppedParts = new HashSet<GameObject>();
    public AudioSource horse;
    public HorseController horseController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;

            if (!hasPlayerEnteredOnce)
            {
                hasPlayerEnteredOnce = true;
                if (firstRock != null)
                {
                    Debug.Log("Player first entered bridge. Dropping one rock.");
                    StartCoroutine(FallDown(firstRock));
                    droppedParts.Add(firstRock);
                }
            }

            if (PreconditionTracker.hasEnteredPrecondition && !HasBroken)
            {
                HasBroken = true;
                Debug.Log("Precondition met. Breaking bridge behind player.");
                horseController.EnterBridge();
                horse.Play();
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
                if (cameraShake != null)
                {
                    //StartCoroutine(cameraShake.Shake(10f, 0.5f));
                    cameraShake.StartShake(15f, 0.5f);
                }

                StartCoroutine(DropRocksInFrontOfPlayer());
            }
        }
    }

    private IEnumerator DropRocksInFrontOfPlayer()
    {
        while (true)
        {
            GameObject nextRock = GetNextRockInFrontOfPlayer();
            if (nextRock != null)
            {
                StartCoroutine(FallDown(nextRock));
                droppedParts.Add(nextRock);
            }

            yield return new WaitForSeconds(dropCheckInterval);
        }
    }

    private GameObject GetNextRockInFrontOfPlayer()
    {
        Vector3 playerPos = player.transform.position;

        // Get undropped parts in FRONT of the player (relative to their forward direction)
        var candidates = bridgeParts
            .Where(part => part != null && !droppedParts.Contains(part))
            .OrderBy(part =>
            {
                Vector3 toPart = part.transform.position - playerPos;
                return Vector3.Dot(toPart, player.transform.forward); // Higher = more in front
            })
            .ToList();

        foreach (var part in candidates)
        {
            Vector3 toPart = part.transform.position - playerPos;
            float forwardDot = Vector3.Dot(toPart.normalized, player.transform.forward);

            if (forwardDot > 0.5f) // Only parts generally in front
            {
                return part;
            }
        }

        return null;
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
