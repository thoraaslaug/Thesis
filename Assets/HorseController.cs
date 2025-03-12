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

    [Header("Audio Settings")]
    public AudioSource gallopAudioSource; // Continuous galloping sound (looped)
    public AudioSource footstepAudioSource; // Individual footstep sounds
    public AudioClip footstepClip; // Assign in Inspector

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        animator.SetFloat("Speed", 0.0f);

        // Ensure gallopAudioSource is set to loop for continuous galloping sound
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

    // 🎵 Play this via Animation Events for hoofstep sounds
    public void PlayFootstep()
    {
        if (footstepAudioSource != null && footstepClip != null)
        {
            footstepAudioSource.pitch = Random.Range(0.9f, 1.1f); // Randomize pitch for variation
            footstepAudioSource.PlayOneShot(footstepClip);
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); 
        float vertical = Input.GetAxis("Vertical"); 

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        float targetSpeed = (inputDirection.magnitude > 0) ? gallopSpeed : 0f;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        if (inputDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        moveDirection = transform.forward * currentSpeed * Time.deltaTime;
        controller.Move(moveDirection);
        
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("Galloping", currentSpeed > 0);

        // 🎵 Control galloping sound properly
        if (gallopAudioSource != null)
        {
            if (currentSpeed > 0 && !gallopAudioSource.isPlaying)
            {
                gallopAudioSource.Play(); // Start looped galloping sound
            }
            else if (currentSpeed <= 0 && gallopAudioSource.isPlaying)
            {
                gallopAudioSource.Stop(); // Stop sound when horse stops
            }
        }
    }

    public void ActivateHorseControl()
    {
        isActive = true;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
