using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MountSystem : MonoBehaviour
{
    [Header("Mount Setup")]
    public Transform mountPoint; // Male
    public Transform rearMountPoint; // Female
    public GameObject player;
    public GameObject female;
    public GameObject horse;
    public Vector3 maleOffsetRight = new Vector3(-0.2f, 0f, 0f);
    public Vector3 maleOffsetLeft = new Vector3(0.2f, 0f, 0f);
    public Vector3 femaleOffsetRight = new Vector3(-0.3f, 0f, 0f);
    public Vector3 femaleOffsetLeft = new Vector3(0.3f, 0f, 0f);
    
    [Header("Animators")]
    public Animator playerAnimator;
    public Animator femaleAnimator;
    public Animator horseAnimator;

    [Header("Controllers")]
    private ThirdPersonController playerController;
    private ThirdPersonController femaleController;
    private HorseController horseController;
    private HorseCameraFollow cameraFollow;

    [Header("Mount State Flags")]
    public bool isMounted = false;
    private bool femaleMounted = false;
    private bool hasShownMountMessage = false;
    private bool frontOccupied = false;
    private bool rearOccupied = false;

    [Header("Optional Extras")]
    public Transform respawnNearFemalePoint;
    public Transform reinsLeftEnd, reinsRightEnd, riderLeftHand, riderRightHand, reinsResetParent;
    public bool triggerNarrationOnStart = true;
    private bool hasStartedSceneNarration = false;

    void Start()
    {
        playerController = player.GetComponent<ThirdPersonController>();
        femaleController = female.GetComponent<ThirdPersonController>();
        horseController = horse.GetComponent<HorseController>();
        cameraFollow = FindFirstObjectByType<HorseCameraFollow>();

        horseAnimator.SetFloat("Speed", 0.0f);

        // 🎬 Scene-based narration
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "FemaleScene" && !hasStartedSceneNarration)
        {
            hasStartedSceneNarration = true;
            string[] lines = {
                "He really came for me...",
                "After all that snow... I didn’t think—",
                "It’s strange though... he hasn’t said a word.",
                "I suppose... I should go with him."
            };

            var narration = FindObjectOfType<NarrationTextManager>();
            if (narration != null)
                narration.StartNarration(lines);
        }

        // 🔁 Setup based on GameState
        if (GameState.returnWithHorse)
        {
            horse.transform.position = respawnNearFemalePoint.position + new Vector3(1.5f, 0f, 0f);
            horse.transform.rotation = respawnNearFemalePoint.rotation;
            player.transform.position = mountPoint.position;
            player.transform.rotation = mountPoint.rotation;
            player.transform.SetParent(mountPoint);

            playerAnimator.SetBool("IsRiding", true);
            playerAnimator.SetFloat("Speed", 0.0f);
            playerAnimator.Play("Ride");

            frontOccupied = true;
            isMounted = true;
            GameState.returnWithHorse = false;
        }
        else
        {
            player.transform.SetParent(null);
            isMounted = false;
            horseController.enabled = false;
            playerController.enabled = true;
        }
    }

    void LateUpdate()
    {
        AdjustMountOffsetBasedOnFacing();
    }
    void Update()
    {
        Debug.Log("🌀 MountSystem is running Update()");
        float distanceToHorse = Vector3.Distance(player.transform.position, horse.transform.position);
        float distanceToFemale = Vector3.Distance(female.transform.position, horse.transform.position);

        // 🧍 Male mounts (only if front is free)
        if (!isMounted && distanceToHorse < 2f && !frontOccupied)
        {
            Debug.Log("📍 Entered male mount check");
            if (!hasShownMountMessage)
            {
                FindObjectOfType<TextPopUpManager>()?.ShowMessage("Press Space to mount horse");
                hasShownMountMessage = true;
            }

            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(MountHorse());
        }

        // 👩 Female mounts only if male is already on horse
        if (!femaleMounted && frontOccupied && distanceToFemale < 2f && !rearOccupied)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(FemaleMountsHorse());
        }

        if (isMounted && horseController != null)
        {
            float speed = horseController.GetCurrentSpeed();
            playerAnimator.SetFloat("Speed", speed);
        }

        if (femaleMounted && horseController != null)
        {
            float speed = horseController.GetCurrentSpeed();
            femaleAnimator.SetFloat("Speed", speed);
        }
    }

    IEnumerator MountHorse()
    {
        isMounted = true;
        frontOccupied = true;

        playerController.enabled = false;
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        playerAnimator.SetTrigger("Mount");
        yield return new WaitForSeconds(1.5f);

        player.transform.position = mountPoint.position;
        player.transform.rotation = mountPoint.rotation;
        player.transform.SetParent(mountPoint);
        AttachReinsToHands();

        if (cameraFollow != null)
        {
            cameraFollow.SwitchToHorse();
            cameraFollow.SetMounted(true);
        }

        horseController.ActivateHorseControl();
        horseController.enabled = true;
        playerController.MountHorse();

        playerAnimator.SetBool("IsRiding", true);
        playerAnimator.SetFloat("Speed", 0.0f);
    }

    IEnumerator FemaleMountsHorse()
    {
        femaleMounted = true;
        rearOccupied = true;

        CharacterController controller = female.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        femaleAnimator.SetTrigger("Mount");
        yield return new WaitForSeconds(1.5f);

        female.transform.position = rearMountPoint.position;
        female.transform.rotation = rearMountPoint.rotation;
        female.transform.SetParent(rearMountPoint);

        if (cameraFollow != null)
        {
            cameraFollow.SwitchToHorse();
            cameraFollow.SetMounted(true);
        }

        horseController.enabled = true;
        horseController.ActivateHorseControl();

        femaleController.MountHorse();

        femaleAnimator.SetBool("IsRiding", true);
        femaleAnimator.SetFloat("Speed", 0.0f);

        Debug.Log("👩 Female mounted the horse behind the male.");
    }
    
    public void DismountMale()
    {
        isMounted = false;
        frontOccupied = false;
        Debug.Log("🧍 Male dismounted and mount flags reset.");
    }

    public void DismountFemale()
    {
        femaleMounted = false;
        rearOccupied = false;
        Debug.Log("👩 Female dismounted and mount flags reset.");
    }
    
    void AdjustMountOffsetBasedOnFacing()
    {
        bool facingRight = horse.transform.forward.x > 0;

        // Male offset
        if (mountPoint != null)
        {
            mountPoint.localPosition = facingRight ? maleOffsetRight : maleOffsetLeft;
        }

        // Female offset
        if (rearMountPoint != null)
        {
            rearMountPoint.localPosition = facingRight ? femaleOffsetRight : femaleOffsetLeft;
        }
    }
    
    void AttachReinsToHands()
    {
        if (reinsLeftEnd && riderLeftHand)
            reinsLeftEnd.SetParent(riderLeftHand, worldPositionStays: true);
        reinsLeftEnd.localPosition = Vector3.zero;         // or a fine-tuned offset
        reinsLeftEnd.localRotation = Quaternion.identity;  // or a custom rotation if needed
        
        if (reinsRightEnd && riderRightHand)
            reinsRightEnd.SetParent(riderRightHand, worldPositionStays: true);
        reinsRightEnd.localPosition = Vector3.zero;
        reinsRightEnd.localRotation = Quaternion.identity;
        

        Debug.Log("🪢 Reins attached to rider hands.");
    }

    public void DetachReins()
    {
        if (reinsLeftEnd && reinsResetParent)
            reinsLeftEnd.SetParent(reinsResetParent, true);

        if (reinsRightEnd && reinsResetParent)
            reinsRightEnd.SetParent(reinsResetParent, true);

        Debug.Log("🔓 Reins detached from rider.");
    }

}
