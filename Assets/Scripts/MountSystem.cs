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
    public Transform reinsLeftEnd;
    public Transform reinsRightEnd;
    public Transform riderLeftHand;
    public Transform riderRightHand;
    public Transform reinsResetParent;
    public Transform respawnNearFemalePoint;



    void Start()
    {    
        playerController = player.GetComponent<ThirdPersonController>();
             horseController = horse.GetComponent<HorseController>();
             cameraFollow = FindFirstObjectByType<HorseCameraFollow>();
     
             //horseController.enabled = false;
             //playerController.enabled = true;
             horseAnimator.SetFloat("Speed", 0.0f); 
        // 🐎 Move player & horse near female if returning
        
        if (GameState.returnWithHorse)
        {
            //player.transform.position = respawnNearFemalePoint.position;
            //player.transform.rotation = respawnNearFemalePoint.rotation;

            horse.transform.position = respawnNearFemalePoint.position + new Vector3(1.5f, 0f, 0f);
            horse.transform.rotation = respawnNearFemalePoint.rotation;
            player.transform.position = mountPoint.position;
            player.transform.rotation = mountPoint.rotation;
            player.transform.SetParent(mountPoint);
            playerAnimator.SetBool("IsRiding", true);
            playerAnimator.SetFloat("Speed", 0.0f);
            playerAnimator.Play("Ride"); // 👈 force this


            GameState.returnWithHorse = false; // Reset flag
            
        } 
        else
        {
            // 🚨 Make sure the player starts unmounted!
            //playerController = player.GetComponent<ThirdPersonController>();

            player.transform.SetParent(null);
            isMounted = false;
            horseController.enabled = false;
            playerController.enabled = true;
        }

        // Existing setup
    
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
            if (horseController != null)
            {
                float horseSpeed = horseController.GetCurrentSpeed();
                playerAnimator.SetFloat("Speed", horseSpeed);
            }
            //float horseSpeed = horseController.GetCurrentSpeed(); // Get the horse's current speed

            // Sync the player's riding animation with horse movement
           // playerAnimator.SetFloat("Speed", horseSpeed);

          //  Debug.Log("Horse Speed: " + horseSpeed);
        }
    }

    bool IsPlayerNearHorse()
    {
        return Vector3.Distance(player.transform.position, horse.transform.position) < 2f;
    }

    IEnumerator MountHorse()
    {
        isMounted = true;

        Debug.Log("🐎 MountHorse DEBUG:");
        Debug.Log("playerController: " + (playerController != null));
        Debug.Log("mountPoint: " + (mountPoint != null));
        Debug.Log("player: " + (player != null));
        Debug.Log("playerAnimator: " + (playerAnimator != null));
        playerController.enabled = false;
        horseController.ActivateHorseControl();

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        playerAnimator.SetTrigger("Mount");

        yield return new WaitForSeconds(1.5f);

        player.transform.position = mountPoint.position;
        player.transform.rotation = mountPoint.rotation;
        player.transform.SetParent(mountPoint);

        // ✅ SAFETY CHECK
        if (cameraFollow != null)
        {
            cameraFollow.SwitchToHorse();
            cameraFollow.SetMounted(true);
        }
        else
        {
            Debug.LogWarning("📸 cameraFollow is null! Cannot switch camera.");
        }

        horseController.enabled = true;
        playerController.MountHorse();

        playerAnimator.SetBool("IsRiding", true);
        playerAnimator.SetFloat("Speed", 0.0f);
    }

    
   /* void AttachReinsToHands()
    {
        if (reinsLeftEnd && riderLeftHand)
            reinsLeftEnd.SetParent(riderLeftHand, worldPositionStays: false);

        if (reinsRightEnd && riderRightHand)
            reinsRightEnd.SetParent(riderRightHand, worldPositionStays: false);
    }
    
    public void DetachReins()
    {
        if (reinsLeftEnd && reinsResetParent)
            reinsLeftEnd.SetParent(reinsResetParent, worldPositionStays: true);

        if (reinsRightEnd && reinsResetParent)
            reinsRightEnd.SetParent(reinsResetParent, worldPositionStays: true);
    }*/
}
