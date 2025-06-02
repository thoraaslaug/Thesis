using System;
using System.Collections;
using MalbersAnimations;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;
using MalbersAnimations.Controller;
using MalbersAnimations.HAP;
using MalbersAnimations.InputSystem;
using StarterAssets;
using UnityEngine.InputSystem;
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
    public MRider rider;
    public MInput riderInput;
    public GameObject bridgeNoSnow;
    public GameObject bridgeSnow;
    
    public ScreenFade screenFade; // Assign in inspector

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

        if (rider != null && !hasPlayed)
        {
            Debug.Log("Horse entered stop zone.");
           // animal.MovementAxis = Vector3.zero;
            //animal.RB.angularVelocity = Vector3.zero;
            
            if (rider.Montura != null)
            {
                rider.Set_StoredMount(rider.Montura.gameObject);
            }

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
        
        if (screenFade != null)
            yield return screenFade.FadeToBlack(1f);
        //yield return new WaitForSeconds(3f); 


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
        //var input = player.GetComponent<StarterAssetsInputs>();
       // if (input) input.enabled = false;

        // Switch camera
        if (timelineCamera) timelineCamera.Priority = 20;
        if (gameplayCamera) gameplayCamera.Priority = 5;

        // Play timeline
        var inputRider = player.GetComponent<MInputLink>();
        if (inputRider != null)
        {
            inputRider.enabled = false;
            Debug.Log("player stopped.");
        }
        timeline.Play();
        yield return new WaitForSeconds(3f);          // ⏳ Step 3: hold black screen while timeline runs
            
          if (screenFade != null)
              yield return screenFade.FadeFromBlack(2f);
        while (timeline.state == PlayState.Playing)
            yield return null;
      
        if (dof != null)
            dof.active = true;

        // Restore player position
        player.transform.position = timelineDummy.transform.position;
        player.transform.rotation = timelineDummy.transform.rotation;
        timelineDummy.SetActive(false);
    
        // Restore visuals and input
        //if (mesh) mesh.enabled = true;
        //if (hair) hair.SetActive(true);
        //if (input) input.enabled = true;
       
        var rider = player.GetComponent<MRider>();
        var animal = player.GetComponent<MAnimal>();

        animal.InputSource?.Enable(true);
        var inputLink = player.GetComponent<MInputLink>();
        if (inputLink != null)
        {
            inputLink.ClearPlayerInput();
            //inputLink.PlayerInput_Set(player);  // Ensure it re-finds the PlayerInput component
            var playerInputComponent = player.GetComponentInChildren<MInputLink>();
            if (playerInputComponent != null)
            {
                inputLink.PlayerInput_Set(playerInputComponent);
            }
            else
            {
                Debug.LogError("❌ Could not find PlayerInput component on player or its children.");
            }
            inputLink.Enable(true);             // Reactivate
            Debug.Log("✅ Input Link reconnected after timeline.");
        }
        if (animal != null)
        {
            animal.InputSource?.Enable(true);
        }

// Restore Rider input
        if (rider?.RiderInput != null)
        {
            rider.RiderInput.MoveCharacter = true;
            rider.RiderInput.Enable(true);
            Debug.Log("✅ Rider input restored");
        }

// Wake up animal
        if (animal != null)
        {
            //animal.SetSleep(false);               // Wake up animal
            animal.InputSource?.Enable(true);     // Re-enable inputs
            animal.ResetController();             // Reset full control state
            Debug.Log("✅ Animal awake and input enabled");
        }

// Remount rider if needed
       /* if (rider != null && rider.Montura == null && rider.MountStored != null)
        {
            rider.Set_StoredMount(rider.MountStored.gameObject);
            rider.MountAnimal();
            Debug.Log("✅ Rider remounted after timeline");
        }*/
        // Restore camera
        if (timelineCamera) timelineCamera.Priority = 5;
        if (gameplayCamera) gameplayCamera.Priority = 20;
        
        if (!GameState.hasPlayedReturnRideNarration)
        {
            Debug.Log("Is Riding: " + rider.IsRiding);
            Debug.Log("Mount is: " + (rider.Montura != null ? rider.Montura.name : "null"));
            Debug.Log("Input Source Enabled: " + (animal.InputSource != null));
            Debug.Log("Current State ID: " + (animal.ActiveState != null ? animal.ActiveState.ID.name : "None"));

            StartReturnRideNarration();
            hasPlayedReturnRideNarration = true;

            if (bridgeNoSnow != null) bridgeNoSnow.SetActive(false);
            if (bridgeSnow != null) bridgeSnow.SetActive(true);
        }

        snowstormTrigger.StartSnowstorm();
    }
    
    void StartReturnRideNarration()
    {
        string[] returnLines = new string[]
        {
            "I have to get back home",
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
            PreconditionTracker.hasEnteredPrecondition = true;
            Debug.Log("📜 Return narration started. Precondition set. Bridge can now break.");
            //if (bridgeNoSnow != null) bridgeNoSnow.SetActive(false);
            //if (bridgeSnow != null) bridgeSnow.SetActive(true);
        }
        else
        {
            Debug.LogWarning("NarrationTextManager not found in scene.");
        }
    }

}
