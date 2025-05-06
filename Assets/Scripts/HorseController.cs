using UnityEngine;
using UnityEngine.SceneManagement;

public class HorseController : MonoBehaviour
{
    public Animator animator;
    public CharacterController controller;
    public AudioSource gallopAudioSource;
    public AudioSource footstepAudioSource;
    public AudioClip footstepClip;

    public NarrationRide narrationRide; // <-- Assign this in Inspector!

    [Header("Horse Movement Settings")]
    public float gallopSpeed = 7f;
    public float turnSpeed = 10f;
    public float acceleration = 5f;

    [Header("Snowstorm Settings")]
    public bool isInSnowstorm = false;
    public float stormGallopSpeed = 4f;
    public float stormTurnSpeed = 4f;
    public float stormAcceleration = 2f;

    [Header("Bridge Settings")]
    public bool isOnBridge = false;
    public float bridgeGallop = 0.4f;
    public float bridgeTurn = 0.2f;
    public float bridgeAcceleration = 0.1f;

    private float currentSpeed = 0f;
    private bool isActive = false;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        animator.SetFloat("Speed", 0.0f);

        if (gallopAudioSource != null)
        {
            gallopAudioSource.loop = true;
            gallopAudioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (isActive)
        {
            HandleMovement();
            HandleNarrationRidingState();
        }
    }
    
    

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        float activeSpeed = gallopSpeed;
        float activeTurnSpeed = turnSpeed;
        float activeAcceleration = acceleration;

        if (isOnBridge)
        {
            activeSpeed = bridgeGallop;
            activeTurnSpeed = bridgeTurn;
            activeAcceleration = bridgeAcceleration;
        }
        else if (isInSnowstorm)
        {
            activeSpeed = stormGallopSpeed;
            activeTurnSpeed = stormTurnSpeed;
            activeAcceleration = stormAcceleration;
        }

        float finalSpeed = (inputDirection.magnitude > 0) ? activeSpeed : 0f;
        currentSpeed = Mathf.Lerp(currentSpeed, finalSpeed, Time.deltaTime * activeAcceleration);

        if (inputDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, activeTurnSpeed * Time.deltaTime);
        }

        moveDirection = transform.forward * currentSpeed * Time.deltaTime;
        controller.Move(moveDirection);

        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("Galloping", currentSpeed > 0.5f);

        if (gallopAudioSource != null)
        {
            if (currentSpeed > 0.1f && !gallopAudioSource.isPlaying)
            {
                gallopAudioSource.Play();
            }
            else if (currentSpeed <= 0.1f && gallopAudioSource.isPlaying)
            {
                gallopAudioSource.Stop();
            }
        }
    }

    void HandleNarrationRidingState()
    {
        if (narrationRide == null)
            return;

        bool isHoldingForward = Input.GetKey(KeyCode.D);
       // narrationRide.isRiding = isHoldingForward;
    }

    public void PlayFootstep()
    {
        if (footstepAudioSource != null && footstepClip != null)
        {
            footstepAudioSource.pitch = Random.Range(0.9f, 1.1f);
            footstepAudioSource.PlayOneShot(footstepClip);
        }
    }

    public void ActivateHorseControl()
    {
        isActive = true;
    }

    public void DeActivateHorseControl()
    {
        isActive = false;
    }

    public void EnterSnowstorm()
    {
        Debug.Log("🌨 Horse is struggling in the snowstorm!");
        isInSnowstorm = true;
    }

    public void ExitSnowstorm()
    {
        Debug.Log("☀ Horse is moving freely again!");
        isInSnowstorm = false;
    }

    public void EnterBridge()
    {
        Debug.Log("🌉 Horse is crossing a fragile bridge...");
        isOnBridge = true;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
