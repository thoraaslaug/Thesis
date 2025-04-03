using System.Collections;
using StarterAssets;
using UnityEngine;

public class MountSystem : MonoBehaviour
{
    public Transform mountPoint; // Position where the player sits on the horse
    public GameObject player; // Reference to the player character
    public GameObject horse; // Reference to the horse object

    public Animator playerAnimator;
    public Animator horseAnimator;

    private ThirdPersonController playerController;
    private HorseController horseController;
    public bool isMounted = false;
    private HorseCameraFollow horseCameraFollow;
    private HorseCameraFollow cameraFollow;


    void Start()
    {
        playerController = player.GetComponent<ThirdPersonController>();
        horseController = horse.GetComponent<HorseController>();
        cameraFollow = FindFirstObjectByType<HorseCameraFollow>();

        horseController.enabled = false;
        playerController.enabled = true;
        horseAnimator.SetFloat("Speed", 0.0f); 
    }

    void Update()
    {
        // Check if player is near horse and presses Space to mount
        if (!isMounted && Input.GetKeyDown(KeyCode.Space) && IsPlayerNearHorse())
        {
            //Debug.Log("Player is attempting to mount the horse...");
            StartCoroutine(MountHorse());
            //if (cameraFollow != null)
            //{
             //   cameraFollow.SetMounted(true);
            //}
            //horseController.enabled = true;
        }
        if (isMounted)
        {
            float horseSpeed = horseController.GetCurrentSpeed(); // Get the horse's current speed

            // Sync the player's riding animation with horse movement
            playerAnimator.SetFloat("Speed", horseSpeed);

          //  Debug.Log("Horse Speed: " + horseSpeed);
        }
    }

    bool IsPlayerNearHorse()
    {
        return Vector3.Distance(player.transform.position, horse.transform.position) < 2f;
    }

    IEnumerator MountHorse()
    {
        //Debug.Log("Mount sequence started...");
        isMounted = true;
        // Disable player movement script to prevent walking mid-mount
        playerController.enabled = false;
        horseController.ActivateHorseControl();

        // Disable CharacterController to prevent physics glitches
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        playerAnimator.SetTrigger("Mount");
        //Debug.Log("Playing Mount animation...");

        yield return new WaitForSeconds(1.5f);

        //Debug.Log("Mount animation finished. Player is now riding.");

        player.transform.position = mountPoint.position;
        player.transform.rotation = mountPoint.rotation;
        player.transform.SetParent(mountPoint);

        horseController.enabled = true;
        playerController.MountHorse();
        cameraFollow.SetMounted(true);

        playerAnimator.SetBool("IsRiding", true);
        playerAnimator.SetFloat("Speed", 0.0f); // Ensure starts in idle riding

        //Debug.Log("Player is now riding. Horse control enabled.");
    }
}
