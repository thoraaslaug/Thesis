using System;
using GlobalSnowEffect;
using UnityEngine;

public class PreconditionTracker : MonoBehaviour
{
    public static bool hasEnteredPrecondition = false; // Shared across scripts
    public SnowstormTrigger snowstormTrigger; // Reference to the Snowstorm System
    public GameObject bridgeNoSnow;
    public GameObject bridgeSnow;

    private void Awake()
    {
        hasEnteredPrecondition = false;
    }

    private void Start()
    {
        bridgeNoSnow.SetActive(true);
        bridgeSnow.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Something entered: {other.gameObject.tag}");

        if (other.CompareTag("Player") && !hasEnteredPrecondition)
        {
            Debug.Log("✅ Player has entered the precondition area. Snowstorm begins!");
            hasEnteredPrecondition = true;

            // Start the snowstorm
            if (snowstormTrigger != null)
            {
                //snowstormTrigger.StartSnowstorm();
                bridgeNoSnow.SetActive(false);
                bridgeSnow.SetActive(true);
            }
            else
            {
                Debug.LogWarning("⚠ No SnowstormTrigger assigned to PreconditionTracker.");
            }
        }
    }
}