using StarterAssets;
using UnityEngine;

public class HorseController : MonoBehaviour
{
    public Animator animator; // Horse Animator
    public float gallopSpeed = 7f;
    public float acceleration = 5f;

    private CharacterController controller;
    private float currentSpeed = 0f;
    private bool isActive = false; // Determines if the horse can move
    private ThirdPersonController character;

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

    void HandleMovement()
    {
        float targetSpeed = 0f;

        if (Input.GetKey(KeyCode.D))
        {
            targetSpeed = gallopSpeed;
        }

        // Smoothly transition to the target speed
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        // Move the horse
        Vector3 moveDirection = transform.forward * currentSpeed * Time.deltaTime;
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