using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    [Header("Bridge Settings")]
    public bool isOnBridge = false; // ✅ Tracks if the horse is in a snowstorm
    public float bridgeGallop = 0.4f; // ✅ Reduced speed in snowstorm
    public float bridgeTurn = 0.2f; // ✅ Reduced turn responsiveness
    public float bridgeAcceleration = 0.1f; // ✅ Slower acceleration

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
        
        if (SceneManager.GetActiveScene().name == "SampleScene" && !GameState.hasStartedRideNarration)
        {
            if (!GameState.hasStartedRideNarration)
            {
                if (!isWaitingToStartNarration && currentSpeed > 0.1f)
                {
                    isWaitingToStartNarration = true;
                    narrationTimer = 2f;
                }

                if (isWaitingToStartNarration)
                {
                    narrationTimer -= Time.deltaTime;
                    if (narrationTimer <= 0f)
                    {
                        StartRideNarration();
                        GameState.hasStartedRideNarration = true;
                        isWaitingToStartNarration = false;
                    }
                }
            }
        }

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // ✅ Use slower speed, turn speed, and acceleration if in a snowstorm
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

// Apply turning
        if (inputDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, activeTurnSpeed * Time.deltaTime);
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
        if (currentSpeed < 0.05f)
        {
            currentSpeed = 0f;
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
            "Tonight, I will see her again.",
            "This cold cannot reach me... not when I'm riding to her.",
            "I wonder if she’s still wearing the apron I gave her.",
            "She'll be surprised to see me...but she will accept my invitation."
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
    
    public void EnterBridge()
    {
        Debug.Log("🌨 Horse is struggling on the bridge!");
        isOnBridge = true;
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
