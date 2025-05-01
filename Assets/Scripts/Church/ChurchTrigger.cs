using System.Collections;
using MalbersAnimations.HAP;
using Unity.Cinemachine;
using UnityEngine;

public class ChurchTrigger : MonoBehaviour
{
    public MountSystem mountSystem;
    public GameObject horse;
    public Transform horseDestination;

    public GameObject manObject;               // Man character (disable input here)
    public GameObject ridingWomanObject;       // Rider (to deactivate)
    public GameObject newWomanObject;          // New playable character

    public MRider ridingMan;
    public MRider ridingWoman;
    public HorseController horseController;

    public HorseCameraFollow cameraFollow;

    private bool hasTriggered = false;
    public CinemachineCamera churchCam;

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
                inputLink.Enable(false);  // Disables input while keeping him active
                Debug.Log("🚫 Man's input disabled.");
            }
        }

        // Wait a moment before the woman dismounts
        yield return new WaitForSeconds(0.5f);

        if (ridingWoman != null)
        {
            ridingWoman.DismountAnimal();
        }

        // Then continue with the rest
        StartCoroutine(SwapToNewWoman());
    }
    private IEnumerator SwapToNewWoman()
    {
        yield return new WaitForSeconds(1.2f); // Slight buffer for dismount animation (adjust if needed)
        
        if (churchCam != null)
            churchCam.Priority = 20;


        // Position the new woman where the rider was
        newWomanObject.transform.SetPositionAndRotation(ridingWomanObject.transform.position, ridingWomanObject.transform.rotation);

        // Activate new player and disable others
        newWomanObject.SetActive(true);
        ridingWomanObject.SetActive(false);
        //manObject.SetActive(false);

        // Switch camera to follow the new woman
        if (cameraFollow != null)
        {
            cameraFollow.SwitchToTarget(newWomanObject.transform, false);
        }

        // Update ChurchCam zoom and rotation
        FindObjectOfType<ChurchCam>()?.ActivateChurchZoom();

        // Continue with event
        StartCoroutine(BeginChurchSequence());
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

        // Begin dragging mechanic
        var dragger = FindObjectOfType<DraggingSystem>();
        if (dragger != null)
            dragger.StartDragging();
    }
}
