using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;
using MalbersAnimations.Controller;
using MalbersAnimations.HAP;
using StarterAssets;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HorseStopZone : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector timeline;
    public CinemachineCamera timelineCamera;
    public CinemachineCamera gameplayCamera;
    public GameObject timelineDummy;
    public GameObject player;
    public GameObject hair;

    private bool hasPlayed = false;
    public SnowstormTrigger snowstormTrigger;
    
    private static bool hasPlayedReturnRideNarration = false;

    public Volume postProcessingVolume; // Assign in inspector
    private DepthOfField dof;

    public GameObject bridgeNoSnow;
    public GameObject bridgeSnow;
    private void Start()
    {
        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out dof);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var animal = other.GetComponentInParent<MAnimal>();
        var rider = other.GetComponentInParent<MRider>();

        if (animal != null && rider != null && !hasPlayed)
        {
            Debug.Log("Horse entered stop zone.");
            animal.MovementAxis = Vector3.zero;
            animal.RB.angularVelocity = Vector3.zero;

            rider.DismountAnimal(); // Automatically triggers dismount
            hasPlayed = true;

            // Start the timeline logic after dismount
            player.GetComponent<MonoBehaviour>().StartCoroutine(PlayTimeline());
        }
    }

  /*  private void OnTriggerExit(Collider other)
    {
        var animal = other.GetComponentInParent<MAnimal>();

        if (animal != null)
        {
            Debug.Log("Horse exited stop zone.");
            hasPlayed = false; // Reset if you want to allow retrigger
        }
    }*/

    private IEnumerator PlayTimeline()
    {
        yield return new WaitForSeconds(2f);

        // Position dummy at player
        timelineDummy.transform.position = player.transform.position;
        timelineDummy.transform.rotation = player.transform.rotation;
        timelineDummy.SetActive(true);
        
        if (dof != null)
            dof.active = false;

        // Hide player visuals
        var mesh = player.GetComponentInChildren<SkinnedMeshRenderer>();
        //if (mesh) mesh.enabled = false;
        if (hair) hair.SetActive(false);

        // Disable input
        var input = player.GetComponent<StarterAssetsInputs>();
        if (input) input.enabled = false;

        // Switch camera
        if (timelineCamera) timelineCamera.Priority = 20;
        if (gameplayCamera) gameplayCamera.Priority = 5;

        // Play timeline
        timeline.Play();
        while (timeline.state == PlayState.Playing)
            yield return null;
        
        if (dof != null)
            dof.active = true;

        // Restore player position
        player.transform.position = timelineDummy.transform.position;
        player.transform.rotation = timelineDummy.transform.rotation;
        timelineDummy.SetActive(false);

        // Restore visuals and input
        if (mesh) mesh.enabled = true;
        //if (hair) hair.SetActive(true);
        if (input) input.enabled = true;

        // Restore camera
        if (timelineCamera) timelineCamera.Priority = 5;
        if (gameplayCamera) gameplayCamera.Priority = 20;
        
        if (!GameState.hasPlayedReturnRideNarration)
        {
            StartReturnRideNarration();
            hasPlayedReturnRideNarration = true;
            PreconditionTracker.hasEnteredPrecondition = true;
            Debug.Log("bridge should break");

            if (bridgeNoSnow != null) bridgeNoSnow.SetActive(false);
            if (bridgeSnow != null) bridgeSnow.SetActive(true);
        }

        snowstormTrigger.StartSnowstorm();
    }
    
    void StartReturnRideNarration()
    {
        string[] returnLines = new string[]
        {
            "She said yes…",
            "The snow feels heavier now, I can barely see",
            "I must keep moving",
            "My hands are numb. No matter. She'll be waiting for me, I'll be back on Christmas Eve.",
            "It’s darker than I remember… the bridge, the sky… the world.",
            "I will see her again. I will see her again..."
        };

        var narration = FindObjectOfType<NarrationTextManager>();
        if (narration != null)
        {
            narration.StartNarration(returnLines);
        }
        else
        {
            Debug.LogWarning("NarrationTextManager not found in scene.");
        }
    }
}
