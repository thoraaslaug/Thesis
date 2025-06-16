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
    private List<Coroutine> activeFallCoroutines = new List<Coroutine>();
    private Coroutine dropRocksCoroutine;
    private bool cancelCollapse = false;
    private bool hasRecordedOriginalPositions = false;

    [Header("Bridge Settings")]
    public GameObject firstRock;
    public float dropCheckInterval = 0.1f;
    public float fallSpeed = 50f;
    public float fallDistance = 10f;

    [Header("Audio & Horse")]
    public AudioSource horseSound;
    public HorseController horseController;

    [Header("Cinemachine Camera Shake")]
    public CinemachineImpulseSource impulseSource;

    public static bool HasBroken { get; private set; }
    public static bool PlayerIsOnBridge { get; private set; } = false;

    private bool hasPlayerEnteredOnce = false;
    private bool collapseStarted = false;
    private GameObject player;

    [Header("Bridge Setup")]
    public Transform bridgeRoot;
    public Transform bridgeStartPoint;
    private GameObject[] bridgeParts;
    private HashSet<GameObject> droppedParts = new HashSet<GameObject>();
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    public GameObject blocker1;
    public GameObject blocker2;
    public GameObject blocker3;
    public GameObject blocker4;

    [Header("Collapse Speed Progression")]
    public float initialDropInterval = 0.3f;
    public float finalDropInterval = 0.05f;
    public float dropSpeedAcceleration = 0.005f;

    public float initialFallSpeed = 10f;
    public float finalFallSpeed = 80f;
    public float minFallDuration = 0.5f;

    public WaterSplashTrigger splashTrigger;
    public MAnimal horse;

    [Header("Reset Settings")]
    public float fallThresholdY = -10f;

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
        if (bridgeRoot != null)
        {
            bridgeParts = bridgeRoot
                .GetComponentsInChildren<Transform>()
                .Where(t => t != bridgeRoot)
                .Select(t => t.gameObject)
                .OrderBy(part => Vector3.Distance(part.transform.position, bridgeStartPoint.position))
                .ToArray();
        }
    }

    private void Update()
    {
        if (player != null && player.transform.position.y < fallThresholdY)
        {
            Debug.Log("😵 Player fell — resetting bridge!");
            ResetBridge();
            RespawnPlayer();
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

            // Try to start collapse — delayed check
            Invoke(nameof(StartCollapseIfStillOnBridge), 0.7f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerIsOnBridge = false;

            if (collapseStarted && !HasBroken)
            {
                Debug.Log("🚫 Player exited bridge early. Resetting.");
                ResetBridge();
            }
        }
    }

    private void StartCollapseIfStillOnBridge()
    {
        if (!PlayerIsOnBridge)
        {
            Debug.Log("🚫 Player left before collapse started. Canceling.");
            ResetBridge();
            return;
        }

        if (!HasBroken && PreconditionTracker.hasEnteredPrecondition)
        {
            HasBroken = true;
            cancelCollapse = false;

            RecordOriginalPositions(); // ✅ NOW we store correct positions

            horseSound?.Play();
            impulseSource?.GenerateImpulse(1f);

            dropRocksCoroutine = StartCoroutine(DropRocksBehind());
        }
    }

    private void RecordOriginalPositions()
    {
        if (hasRecordedOriginalPositions || bridgeParts == null) return;

        originalPositions.Clear();
        rockQueue.Clear(); // ✅ clear queue first

        foreach (var rock in bridgeParts)
        {
            originalPositions[rock] = rock.transform.position;
            rockQueue.Enqueue(rock); // ✅ queue it here too!
        }

        hasRecordedOriginalPositions = true;
        Debug.Log("✅ Recorded bridge positions at collapse start.");
    }


    private IEnumerator DelayedFirstRock()
    {
        yield return new WaitForSeconds(2f);
        Coroutine firstDrop = StartCoroutine(FallDown(firstRock, 10f));
        activeFallCoroutines.Add(firstDrop);
        droppedParts.Add(firstRock);
    }

    private IEnumerator DropRocksBehind()
    {
        horse.SpeedDown();
        collapseStarted = true;
        cancelCollapse = false;

        blocker1.SetActive(true);
        blocker2.SetActive(true);
        blocker3.SetActive(true);
        blocker4.SetActive(true);

        float currentInterval = initialDropInterval;
        float currentFallSpeed = initialFallSpeed;

        while (rockQueue.Count > 0)
        {
            if (!PlayerIsOnBridge)
            {
                Debug.Log("🚫 Player left during collapse — cancelling and resetting.");
                ResetBridge();
                yield break;
            }

            if (cancelCollapse)
            {
                Debug.Log("⛔ Collapse canceled mid-way.");
                yield break;
            }

            for (int i = 0; i < 12 && rockQueue.Count > 0; i++)
            {
                var rock = rockQueue.Dequeue();
                Coroutine fall = StartCoroutine(FallDown(rock, currentFallSpeed));
                activeFallCoroutines.Add(fall);
            }

            currentInterval = Mathf.Max(finalDropInterval, currentInterval - dropSpeedAcceleration);
            currentFallSpeed = Mathf.Min(finalFallSpeed, currentFallSpeed + dropSpeedAcceleration * 100f);

            yield return new WaitForSeconds(currentInterval);
        }
    }

    private IEnumerator FallDown(GameObject part, float customFallSpeed)
    {
        Vector3 start = part.transform.position;
        Vector3 end = start + Vector3.down * fallDistance;
        float duration = Mathf.Max(minFallDuration, fallDistance / customFallSpeed);

        float time = 0f;
        while (time < duration)
        {
            if (cancelCollapse)
            {
                yield break;
            }

            part.transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        part.transform.position = end;

        if (splashTrigger != null && !cancelCollapse)
        {
            splashTrigger.PlaySplashEffect(end);
        }
    }

    private void CancelAllFallCoroutines()
    {
        foreach (var c in activeFallCoroutines)
        {
            if (c != null) StopCoroutine(c);
        }

        activeFallCoroutines.Clear();
    }

    public void ResetBridge()
    {
        cancelCollapse = true;

        if (dropRocksCoroutine != null)
        {
            StopCoroutine(dropRocksCoroutine);
            dropRocksCoroutine = null;
        }

        CancelAllFallCoroutines();
        rockQueue.Clear();
        droppedParts.Clear();

        foreach (var rock in bridgeParts)
        {
            if (rock == null || !originalPositions.ContainsKey(rock)) continue;

            rock.SetActive(true);
            rock.transform.localPosition = originalPositions[rock]; // 🔧 restore localPosition
            rockQueue.Enqueue(rock);
        }

        HasBroken = false;
        hasPlayerEnteredOnce = false;
        collapseStarted = false;
        hasRecordedOriginalPositions = false;

        blocker1.SetActive(false);
        blocker2.SetActive(false);
        blocker3.SetActive(false);
        blocker4.SetActive(false);

        Debug.Log("🔁 Bridge reset to original state.");
    }



    private void RespawnPlayer()
    {
        Transform respawnPoint = GameObject.Find("RespawnPoint")?.transform;
        if (respawnPoint != null && player != null)
        {
            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;

            var animal = player.GetComponent<MAnimal>();
            if (animal != null)
            {
                animal.ResetController();
            }

            Debug.Log("↩️ Player respawned.");
        }
        else
        {
            Debug.LogWarning("⚠️ RespawnPoint or player not found!");
        }
    }
}
