using UnityEngine;

public class HorseController : MonoBehaviour
{
    public Animator animator; // Horse Animator
    public float gallopSpeed = 7f;
    public float turnSpeed = 10f;
    public float acceleration = 5f;

    private CharacterController controller;
    private float currentSpeed = 0f;
    private bool isActive = false; // Determines if the horse can move
    private Vector3 moveDirection = Vector3.zero;
    public AudioSource audioSource;
    public AudioClip clip;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Ensure horse starts in idle mode
        animator.SetFloat("Speed", 0.0f);
    }

    void Update()
    {
        if (isActive) // Only handle movement if active
        {
            HandleMovement();
        }
    }

    public void PlayFootstep()
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical"); // W/S or Up/Down

        // Normalize movement vector so diagonal movement isn't faster
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Determine target speed based on movement input
        float targetSpeed = (inputDirection.magnitude > 0) ? gallopSpeed : 0f;

        // Smoothly transition to the target speed
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        // Move the horse
        if (inputDirection.magnitude > 0)
        {
            // Rotate the horse towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Apply movement
        moveDirection = transform.forward * currentSpeed * Time.deltaTime;
        controller.Move(moveDirection);

        // Update animator
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("Galloping", currentSpeed > 0);
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
