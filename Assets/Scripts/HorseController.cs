using UnityEngine;

public class HorseController : MonoBehaviour
{
    public Animator animator; 
    public float gallopSpeed = 7f;
    public float turnSpeed = 10f;
    public float acceleration = 5f;

    private CharacterController controller;
    private float currentSpeed = 0f;
    private bool isActive = false; 
    private Vector3 moveDirection = Vector3.zero;

    [Header("Snowstorm Settings")]
    public bool isInSnowstorm = false; // ✅ Tracks if the horse is in a snowstorm
    public float stormGallopSpeed = 4f; // ✅ Reduced speed in snowstorm
    public float stormTurnSpeed = 4f; // ✅ Reduced turn responsiveness
    public float stormAcceleration = 2f; // ✅ Slower acceleration

    [Header("Audio Settings")]
    public AudioSource gallopAudioSource; 
    public AudioSource footstepAudioSource; 
    public AudioClip footstepClip; 
    private bool hasStartedNarration = false;
    private float narrationTimer = 0f;
    private bool isWaitingToStartNarration = false;


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
        }
    }

    public void PlayFootstep()
    {
        if (footstepAudioSource != null && footstepClip != null)
        {
            footstepAudioSource.pitch = Random.Range(0.9f, 1.1f); 
            footstepAudioSource.PlayOneShot(footstepClip);
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); 
        float vertical = Input.GetAxis("Vertical"); 
       
        if (!hasStartedNarration && !isWaitingToStartNarration && vertical > 0)
        {
            isWaitingToStartNarration = true;
            narrationTimer = 2f; // Wait for 2 seconds
        }

        // Countdown until narration starts
        if (isWaitingToStartNarration)
        {
            narrationTimer -= Time.deltaTime;
            if (narrationTimer <= 0f)
            {
                StartRideNarration();
                hasStartedNarration = true;
                isWaitingToStartNarration = false;
            }
        }

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // ✅ Use slower speed, turn speed, and acceleration if in a snowstorm
        float targetSpeed = isInSnowstorm ? stormGallopSpeed : gallopSpeed;
        float targetTurnSpeed = isInSnowstorm ? stormTurnSpeed : turnSpeed;
        float targetAcceleration = isInSnowstorm ? stormAcceleration : acceleration;

        float finalSpeed = (inputDirection.magnitude > 0) ? targetSpeed : 0f;
        currentSpeed = Mathf.Lerp(currentSpeed, finalSpeed, Time.deltaTime * targetAcceleration);

        if (inputDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, targetTurnSpeed * Time.deltaTime);
        }

        moveDirection = transform.forward * currentSpeed * Time.deltaTime;
        controller.Move(moveDirection);
        
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("Galloping", currentSpeed > 0);

        if (gallopAudioSource != null)
        {
            if (currentSpeed > 0 && !gallopAudioSource.isPlaying)
            {
                gallopAudioSource.Play(); 
            }
            else if (currentSpeed <= 0 && gallopAudioSource.isPlaying)
            {
                gallopAudioSource.Stop(); 
            }
        }
    }

    public void ActivateHorseControl()
    {
        isActive = true;
    }
    
    void StartRideNarration()
    {
        string[] narrationLines = new string[]
        {
            "Tonight, she will hear my voice again.",
            "This cold cannot reach me — not when I'm riding to her.",
            "I wonder if she’s still wearing the apron I gave her.",
            "She'll be surprised to see me — but she will come."
        };

        var narrationManager = FindObjectOfType<NarrationTextManager>();
        if (narrationManager != null)
        {
            narrationManager.StartNarration(narrationLines);
        }
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

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
