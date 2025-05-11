using System;
using System.Collections;
using MalbersAnimations.HAP;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class ChurchTrigger : MonoBehaviour
{
    public MountSystem mountSystem;
    public GameObject horse;
    public Transform horseDestination;

    public GameObject manObject;
    public GameObject newManObject;
    public GameObject ridingWomanObject;
    public GameObject newWomanObject;

    public MRider ridingMan;
    public MRider ridingWoman;
    public HorseController horseController;
    public ScreenFade screenFade;

    public HorseCameraFollow cameraFollow;
    public static bool timelineHasPlayed = false;

    private bool hasTriggered = false;
    public CinemachineCamera churchCam;
    public PlayableDirector draggingTimeline;
    public NarrationTextManager narrationTextManager;
    public string[] preTimelineNarration = {
        "Where is he going?",
        "I have to get to the church",
        "I have to escape him"
    };

   /* private void Awake()
    {
        timelineHasPlayed = false;
    }*/
    
    

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(DismountSequence());
        }
    }

    private IEnumerator DismountSequence()
    {
        if (screenFade != null)
            yield return StartCoroutine(screenFade.FadeToBlack(1f));

        if (ridingWoman != null)
        {
            ridingWoman.DismountAnimal();
            yield return new WaitUntil(() => !ridingWoman.Montura.Mounted);
            Debug.Log(" Woman has dismounted.");
            var inputLink = ridingWoman.GetComponent<MalbersAnimations.InputSystem.MInputLink>();
            if (inputLink != null) inputLink.Enable(false);
        }

        yield return StartCoroutine(SwapToNewWoman());

        yield return new WaitForSeconds(0.8f);

        if (ridingMan != null)
        {
            ridingMan.DismountAnimal();
            yield return new WaitUntil(() => !ridingMan.Montura.Mounted);
            Debug.Log(" Man has dismounted.");

           var inputLink = ridingMan.GetComponent<MalbersAnimations.InputSystem.MInputLink>();
            if (inputLink != null) inputLink.Enable(false);

            yield return new WaitForSeconds(1f);
            manObject.SetActive(false);
        }

        StartCoroutine(BeginChurchSequence());
    }

    private IEnumerator SwapToNewWoman()
    {
        yield return new WaitForSeconds(1.2f);

        //if (churchCam != null) churchCam.Priority = 20;

        newWomanObject.transform.SetPositionAndRotation(ridingWomanObject.transform.position, ridingWomanObject.transform.rotation);

        newWomanObject.SetActive(true);
        ridingWomanObject.SetActive(false);

        //if (cameraFollow != null)
          //  cameraFollow.SwitchToTarget(newWomanObject.transform, false);

       /* var input = newWomanObject.GetComponent<StarterAssetsInputs>();
        var controller = newWomanObject.GetComponent<ThirdPersonController>();
        if (input != null) input.enabled = false;
        if (controller != null) controller.enabled = false;*/

        //FindObjectOfType<ChurchCam>()?.ActivateChurchZoom();
    }

    public void EnableFall()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(transform.forward * 2f + Vector3.down * 5f, ForceMode.VelocityChange);
        }
    }

    private IEnumerator BeginChurchSequence()
    {
        if (horseController != null)
            horseController.enabled = false;

        float moveDuration = 3.0f;
        float elapsed = 0f;
        Vector3 startPos = horse.transform.position;
        Vector3 targetPos = horseDestination.position;

        while (elapsed < moveDuration)
        {
            horse.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        horse.SetActive(false);

        //  Start Timeline
        if (draggingTimeline != null)
        {
            draggingTimeline.Play();
            Debug.Log(" Dragging timeline triggered.");
        }

        // Start narration
        if (narrationTextManager != null)
            narrationTextManager.StartNarration(preTimelineNarration);

        //  Fade in to timeline
        if (screenFade != null)
            yield return StartCoroutine(screenFade.FadeFromBlack(0.4f));

        //  Wait for timeline to complete
        yield return new WaitUntil(() => draggingTimeline.state != PlayState.Playing);

        //  Switch to ChurchCam for gameplay
        if (churchCam != null)
            churchCam.Priority = 20;
        
        if (narrationTextManager != null)
        {
            string[] churchLines = {
                "The church... maybe they’ll hear the bells.",
                "This has to end now.",
            };
            narrationTextManager.StartNarration(churchLines);
        }
       
       /* var controller = newWomanObject.GetComponent<ThirdPersonController>();
        var input = newWomanObject.GetComponent<StarterAssetsInputs>();
        if (controller != null) controller.enabled = true;
        if (input != null) input.enabled = true;*/
        

        Debug.Log("Cutscene finished. ChurchCam activated and input enabled.");
        timelineHasPlayed = true;

    }
    
    private void Update()
    {
        if (newWomanObject != null)
        {
            var input = newWomanObject.GetComponent<StarterAssetsInputs>();
            var controller = newWomanObject.GetComponent<ThirdPersonController>();
            if (input != null && controller != null)
            {
                Debug.Log($"Input: {input.move} — Enabled: {input.enabled}, Controller: {controller.enabled}");
            }
        }
    }


}
