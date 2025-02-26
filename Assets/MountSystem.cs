using System.Collections;
using StarterAssets;
using UnityEngine;

public class MountSystem : MonoBehaviour
{
    public Transform mountPoint; // Position where the player sits on the horse
    public GameObject player; // Reference to the player character
    public GameObject horse; // Reference to the horse (horse object with HorseController)

    public Animator playerAnimator;
    public Animator horseAnimator;

    private ThirdPersonController playerController;
    private HorseController horseController;
    private bool isMounted = false; // Track if player is mounted

    void Start()
    {
        playerController = player.GetComponent<ThirdPersonController>();
        horseController = horse.GetComponent<HorseController>();

        // Disable horse movement initially
        horseController.enabled = false;
    }

    void Update()
    {
        if (!isMounted && Input.GetKeyDown(KeyCode.Space) && IsPlayerNearHorse())
        {
            Debug.Log("Player is attempting to mount the horse...");
            StartCoroutine(MountHorse());
        }
        if (isMounted)
        {
            float horseSpeed = horseController.GetCurrentSpeed(); // Get the horse's current speed

            // Sync the player's riding animation with horse movement
            playerAnimator.SetFloat("Speed", horseSpeed);

            Debug.Log("Horse Speed: " + horseSpeed);
        }
    }

    bool IsPlayerNearHorse()
    {
        return Vector3.Distance(player.transform.position, horse.transform.position) < 2f;
    }

    IEnumerator MountHorse()
    {
        Debug.Log("Mount sequence started...");
        isMounted = true;  // Prevent multiple mounts
        playerController.isMounted = true;

        // Disable player movement
        playerController.enabled = false;
        CharacterController playerControllerComponent = player.GetComponent<CharacterController>();
        if (playerControllerComponent != null)
        {
            playerControllerComponent.enabled = false;
        }

        // Play mounting animation
        playerAnimator.SetTrigger("Mount");
        Debug.Log("Mount animation triggered...");

        // Wait for animation to complete
        yield return new WaitForSeconds(1.5f);

        // Attach player to mount point
        player.transform.position = mountPoint.position;
        player.transform.rotation = mountPoint.rotation;
        player.transform.SetParent(mountPoint);

        // Enable horse movement
        horseController.enabled = true;

        // Ensure the animator switches to riding animation
        playerAnimator.SetBool("IsRiding", true);
        Debug.Log("Player is now riding. Animator updated.");

        // Re-enable CharacterController AFTER parenting to prevent physics issues
        if (playerControllerComponent != null)
        {
            playerControllerComponent.enabled = true;
        }


        Debug.Log("Mount sequence complete.");
    }
}
