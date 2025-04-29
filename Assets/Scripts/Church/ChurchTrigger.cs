using System.Collections;
using StarterAssets;
using UnityEngine;

public class ChurchTrigger : MonoBehaviour
{
    public MountSystem mountSystem;
    public Animator manAnimator;
    public Animator womanAnimator;

    public HorseController horseController;
    public GameObject horse;
    public Transform horseDestination; // where man brings the horse

    public GameObject playerControlWoman; // Enable after dismount
    public GameObject playerControlMan;   // Disable after dismount
    public ThirdPersonController womanController;
    public ThirdPersonController manController;

    private bool hasTriggered = false;
    public HorseCameraFollow camera;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            manAnimator.transform.SetParent(null);
            playerControlWoman.transform.SetParent(null);
            womanController.DismountFromHorseImmediately();
            manController.DismountFromHorseImmediately();
            camera.SwitchToPlayer();
            
            if (manAnimator != null)
            {
                manAnimator.SetTrigger("Dismount");
                manAnimator.SetBool("IsRiding", false);
            }
        
            if (womanAnimator != null)
            {
                womanAnimator.SetTrigger("Dismount");
                womanAnimator.SetBool("IsRiding", false);

            }


            // Deactivate horse
            horse.SetActive(false);

            StartCoroutine(BeginChurchSequence());
        }
    }

    private IEnumerator BeginChurchSequence()
    {
        // 🐎 Step 1: Dismount Male and Female
        mountSystem.DismountMale();
        mountSystem.DismountFemale();
        mountSystem.DetachReins();

        // Play dismount animations manually
        /*if (manAnimator != null)
        {
            manAnimator.SetTrigger("Dismount");
            manAnimator.SetBool("IsRiding", false);
        }
        
        if (womanAnimator != null)
        {
            womanAnimator.SetTrigger("Dismount");
            womanAnimator.SetBool("IsRiding", false);

        }*/

        yield return new WaitForSeconds(1.5f);

        // 🐴 Step 2: Man leads horse away
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

        horse.SetActive(false); // 💨 Hide horse when out of view

        // 🏃‍♀️ Step 3: Switch control to Woman
        playerControlWoman.SetActive(true);
        playerControlMan.SetActive(false);

        // (then the Dragging System kicks in after a few seconds)
        var dragger = FindObjectOfType<DraggingSystem>();
        if (dragger != null)
            dragger.StartDragging();
    }
}

