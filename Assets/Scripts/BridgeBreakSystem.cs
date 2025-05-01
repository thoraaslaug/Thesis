using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;

public class BridgeBreakSystem : MonoBehaviour
{
    [Header("Bridge Settings")]
    public GameObject[] bridgeParts;
    public GameObject firstRock;
    public float dropCheckInterval = 0.1f;
    public float fallSpeed = 50f;               // ⬆️ Increased for quicker fall
    public float fallDistance = 10f;
    public float dropAheadDistance = 20f;       // ⬆️ Increased for faster triggering

    [Header("Audio & Horse")]
    public AudioSource horseSound;
    public HorseController horseController;

    [Header("Cinemachine Camera Shake")]
    public CinemachineImpulseSource impulseSource;

    private bool hasPlayerEnteredOnce = false;
    public static bool HasBroken { get; private set; }
    private GameObject player;
    private HashSet<GameObject> droppedParts = new HashSet<GameObject>();

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
                Debug.Log("Precondition met. Breaking bridge.");

                //horseController.EnterBridge();
                horseSound?.Play();
                impulseSource?.GenerateImpulse(1f);

                StartCoroutine(DropRocksAhead());
            }
        }
    }

    private IEnumerator DropRocksAhead()
    {
        while (true)
        {
            for (int i = 0; i < 3; i++) // ⬆️ Drop 3 rocks per frame for urgency
            {
                GameObject rock = GetNextRockAhead();
                if (rock != null)
                {
                    StartCoroutine(FallDown(rock));
                    droppedParts.Add(rock);
                }
            }
            yield return new WaitForSeconds(dropCheckInterval);
        }
    }

    private GameObject GetNextRockAhead()
    {
        Vector3 playerPos = player.transform.position;
        var candidates = bridgeParts
            .Where(part => part != null && !droppedParts.Contains(part))
            .OrderBy(part => Vector3.Distance(part.transform.position, playerPos))
            .ToList();

        foreach (var part in candidates)
        {
            Vector3 toPart = part.transform.position - playerPos;
            if (Vector3.Dot(toPart.normalized, player.transform.forward) > 0.5f &&
                Vector3.Distance(part.transform.position, playerPos) <= dropAheadDistance)
            {
                return part;
            }
        }
        return null;
    }

    private IEnumerator FallDown(GameObject part)
    {
        Vector3 start = part.transform.position;
        Vector3 end = start + Vector3.down * fallDistance;
        float duration = fallDistance / fallSpeed; // ✅ Faster fall duration

        float time = 0f;
        while (time < duration)
        {
            part.transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        part.transform.position = end;
    }
}
