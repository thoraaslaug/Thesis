using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;


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
    public static bool PlayerIsOnBridge { get; private set; } = false;

    private void Awake()
    {
        PlayerIsOnBridge = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            PlayerIsOnBridge = true;

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
                //Debug.Log("player is on bridge. interior scene next");
                HasBroken = true;
                //Debug.Log("Precondition met. Breaking bridge.");

                //horseController.EnterBridge();
                horseSound?.Play();
                impulseSource?.GenerateImpulse(1f);

                StartCoroutine(DropRocksAhead());
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerIsOnBridge = false;
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

  /*  private GameObject GetNextRockAhead()
    {
        Vector3 playerPos = player.transform.position;
        var candidates = bridgeParts
            .Where(part => part != null && !droppedParts.Contains(part))
            .OrderBy(part => Vector3.Distance(part.transform.position, playerPos))
            .ToList();

        foreach (var part in candidates)
        {
            Vector3 toPart = part.transform.position - playerPos;
            if (Vector3.Dot(toPart.normalized, player.transform.forward) > 0.1f &&
                Vector3.Distance(part.transform.position, playerPos) <= dropAheadDistance)
            {
                return part;
            }
        }
        return null;
    }*/
  
  private GameObject GetNextRockAhead()
  {
      Vector3 playerPos = player.transform.position;

      var candidates = bridgeParts
          .Where(part => part != null && !droppedParts.Contains(part))
          .OrderBy(part => Vector3.Distance(part.transform.position, playerPos))
          .ToList();

     // Debug.Log($"Checking {candidates.Count} candidate rocks...");

      foreach (var part in candidates)
      {
          Vector3 toPart = part.transform.position - playerPos;
          float dot = Vector3.Dot(toPart.normalized, player.transform.forward);
          float dist = Vector3.Distance(part.transform.position, playerPos);

         // Debug.Log($"→ Rock: {part.name}, Dot: {dot:F2}, Dist: {dist:F2}");

          if (dot > 0.5f && dist <= dropAheadDistance)
          {
             // Debug.Log($"✅ Dropping rock: {part.name}");
              return part;
          }
      }

      Debug.Log("❌ No valid rock found ahead.");
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
