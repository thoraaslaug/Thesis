using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MalbersAnimations.Controller;
using Unity.Cinemachine;
using UnityEngine;

public class BridgeBreakSystem : MonoBehaviour
{
    
    private Queue<GameObject> rockQueue = new Queue<GameObject>();

    [Header("Bridge Settings")]
    //public GameObject[] bridgeParts;
    public GameObject firstRock;
    public float dropCheckInterval = 0.1f;
    public float fallSpeed = 50f;
    public float fallDistance = 10f;

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
    
    public Transform bridgeRoot; // ← Assign this in Inspector
    private GameObject[] bridgeParts;
    public Transform bridgeStartPoint; // ⬅️ Assign this to the BACK of the bridge in Inspector


    public GameObject blocker1;
    public GameObject blocker2;
    public GameObject blocker3;
    public GameObject blocker4;

    private bool collapseStarted = false;
    
    [Header("Collapse Speed Progression")]
    public float initialDropInterval = 0.3f;
    public float finalDropInterval = 0.05f;
    public float dropSpeedAcceleration = 0.005f;

    public float initialFallSpeed = 10f;
    public float finalFallSpeed = 80f;
    public float minFallDuration = 0.5f; // Never shorter than this

    public WaterSplashTrigger splashTrigger;

    public MAnimal horse;


    private void Awake()
    {
        HasBroken = false;
        PlayerIsOnBridge = false;
        blocker1.SetActive(false);
        blocker2.SetActive(false);
        blocker3.SetActive(false);
        blocker4.SetActive(false);
    }

    private void Start()
    {
        
        // Automatically collect all child rocks under bridgeRoot
        if (bridgeRoot != null)
        {
            bridgeParts = bridgeRoot
                .GetComponentsInChildren<Transform>()
                .Where(t => t != bridgeRoot)
                .Select(t => t.gameObject)
                .OrderBy(part => Vector3.Distance(part.transform.position, bridgeStartPoint.position))
                .ToArray();

            foreach (var rock in bridgeParts)
            {
                rockQueue.Enqueue(rock);
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            PlayerIsOnBridge = true;

            if (!hasPlayerEnteredOnce)
            {
                collapseStarted = false;
                hasPlayerEnteredOnce = true;

                if (firstRock != null)
                {
                    Debug.Log("Player entered bridge. Dropping first rock in 2 seconds...");
                    StartCoroutine(DelayedFirstRock());
                }
            }

            if (PreconditionTracker.hasEnteredPrecondition && !HasBroken)
            {
                HasBroken = true;

                horseSound?.Play();
                impulseSource?.GenerateImpulse(1f);

                StartCoroutine(DropRocksBehind());
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

    private IEnumerator DelayedFirstRock()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(FallDown(firstRock, 10f));
        droppedParts.Add(firstRock);
    }

    private IEnumerator DropRocksBehind()
    {
        
        horse.SpeedDown();
        collapseStarted = true;

        blocker1.SetActive(true);
        blocker2.SetActive(true);
        blocker3.SetActive(true);
        blocker4.SetActive(true);

        float currentInterval = initialDropInterval;
        float currentFallSpeed = initialFallSpeed;

        while (rockQueue.Count > 0)
        {
            for (int i = 0; i < 12 && rockQueue.Count > 0; i++)
            {
                var rock = rockQueue.Dequeue();
                StartCoroutine(FallDown(rock, currentFallSpeed));
            }

            // Gradually speed up
            currentInterval = Mathf.Max(finalDropInterval, currentInterval - dropSpeedAcceleration);
            currentFallSpeed = Mathf.Min(finalFallSpeed, currentFallSpeed + dropSpeedAcceleration * 100f);

            yield return new WaitForSeconds(currentInterval);
        }
    }


    private GameObject GetNextRockBehindPlayer()
    {
        Vector3 playerPos = player.transform.position;

        var candidates = bridgeParts
            .Where(part => part != null && !droppedParts.Contains(part))
            .OrderByDescending(part => Vector3.Distance(part.transform.position, playerPos)) // Furthest to closest
            .ToList();

        foreach (var part in candidates)
        {
            Vector3 toPart = playerPos - part.transform.position;
            float dot = Vector3.Dot(toPart.normalized, player.transform.forward);

            if (dot > 0.5f) // Rock is behind the player
            {
                return part;
            }
        }

        return null;
    }

    private IEnumerator FallDown(GameObject part, float customFallSpeed)
    {
        Vector3 start = part.transform.position;
        Vector3 end = start + Vector3.down * fallDistance;
        float duration = Mathf.Max(minFallDuration, fallDistance / customFallSpeed);

        float time = 0f;
        while (time < duration)
        {
            part.transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        part.transform.position = end;

       /* if (splashTrigger != null)
        {
            splashTrigger.PlaySplashEffect(part.transform.position); // ✅ Always current position
        }*/

    }
}