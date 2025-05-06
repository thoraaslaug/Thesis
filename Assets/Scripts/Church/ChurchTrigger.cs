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
    public GameObject newManObject;          // New playable character
    // Man character (disable input here)
    public GameObject ridingWomanObject;       // Rider (to deactivate)
    public GameObject newWomanObject;          // New playable character

    public MRider ridingMan;
    public MRider ridingWoman;
    public HorseController horseController;

    public HorseCameraFollow cameraFollow;

    private bool hasTriggered = false;
    public CinemachineCamera churchCam;
    public PlayableDirector draggingTimeline;
    public NarrationTextManager narrationTextManager;
    public string[] preTimelineNarration = {
        "Where did he go?",
        "I have to get to the church",
        "I have to escape him"
    };

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
        if (ridingMan != null)
        {
            ridingMan.DismountAnimal();
            var inputLink = ridingMan.GetComponent<MalbersAnimations.InputSystem.MInputLink>();
            if (inputLink != null)
            {
                inputLink.Enable(false);
                Debug.Log("🚫 Man's input disabled.");
            }
        }
        yield return new WaitForSeconds(1f);


        if (ridingWoman != null)
        {
            ridingWoman.DismountAnimal();
            yield return new WaitUntil(() => !ridingWoman.Montura.Mounted);
            yield return new WaitForSeconds(1f); 
            StartCoroutine(SwapToNewWoman());


            Debug.Log("✅ Woman has dismounted.");
        }

        // ✅ Wait until man has fully dismounted too
        yield return new WaitUntil(() => !ridingMan.Montura.Mounted);
        Debug.Log("✅ Man has dismounted.");

        
        yield return new WaitForSeconds(1f); 

        // ✅ Now deactivate old man
        manObject.SetActive(false);

        // Continue with swap and sequence
        //StartCoroutine(SwapToNewWoman());
    }

    private IEnumerator SwapToNewWoman()
    {
        var woman = newWomanObject.GetComponent<CharacterController>();
        if (woman != null)
        {
            woman.enabled = false; // Freeze
        }
      
        yield return new WaitForSeconds(0.05f); // Slight buffer for dismount animation (adjust if needed)
        
        if (churchCam != null)
            churchCam.Priority = 20;


        // Position the new woman where the rider was
        newWomanObject.transform.SetPositionAndRotation(ridingWomanObject.transform.position, ridingWomanObject.transform.rotation);
        //newWomanObject.transform.SetPositionAndRotation(finalPos, finalRot);

        // Activate new player and disable others
        newWomanObject.SetActive(true);
        ridingWomanObject.SetActive(false);
        //manObject.SetActive(false);

        // Switch camera to follow the new woman
        if (cameraFollow != null)
        {
            cameraFollow.SwitchToTarget(newWomanObject.transform, false);
        }
        var input = newWomanObject.GetComponent<StarterAssetsInputs>();
        var controller = newWomanObject.GetComponent<ThirdPersonController>();
        if (input != null) input.enabled = false;
        if (controller != null) controller.enabled = false;

        // Update ChurchCam zoom and rotation
        FindObjectOfType<ChurchCam>()?.ActivateChurchZoom();

        // Continue with event
        StartCoroutine(BeginChurchSequence());
    }
    
   // private void SwapToNewMan()
    //{
        //newManObject.transform.SetPositionAndRotation(manObject.transform.position, manObject.transform.rotation);
      //  newManObject.SetActive(true);
        //manObject.SetActive(false);
    //}

    
   /* private IEnumerator SwapToNewMan()
    {
        yield return new WaitForSeconds(1.2f); // Slight buffer for dismount animation (adjust if needed)
        
        if (churchCam != null)
            churchCam.Priority = 20;


        // Position the new woman where the rider was
        newManObject.transform.SetPositionAndRotation(manObject.transform.position, manObject.transform.rotation);

        // Activate new player and disable others
        newManObject.SetActive(true);
        manObject.SetActive(false);
        //manObject.SetActive(false);

        // Switch camera to follow the new woman
        //if (cameraFollow != null)
        //{
          //  cameraFollow.SwitchToTarget(newWomanObject.transform, false);
        //}

        // Update ChurchCam zoom and rotation
        //FindObjectOfType<ChurchCam>()?.ActivateChurchZoom();

        // Continue with event
        //StartCoroutine(BeginChurchSequence());
    }*/
    
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
        mountSystem.DismountMale();
        mountSystem.DetachReins();

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

        if (narrationTextManager != null)
        {
            bool narrationFinished = false;
            narrationTextManager.onNarrationComplete = () => narrationFinished = true;
            narrationTextManager.StartNarration(preTimelineNarration);

            yield return new WaitUntil(() => narrationFinished);
        }

        // ✅ Activate new man object before timeline starts
        if (newManObject != null && !newManObject.activeSelf)
        {
            newManObject.SetActive(true);
            Debug.Log("✅ New man activated.");
        }

        if (draggingTimeline != null)
        {
            draggingTimeline.Play();
            Debug.Log("🎬 Dragging timeline triggered.");

            var input = newWomanObject.GetComponent<StarterAssetsInputs>();
            var controller = newWomanObject.GetComponent<ThirdPersonController>();
            if (input != null) input.enabled = true;
            if (controller != null) controller.enabled = true;

            yield break;
        }

        var dragger = FindObjectOfType<DraggingSystem>();
        if (dragger != null)
            dragger.StartDragging();
    }

}