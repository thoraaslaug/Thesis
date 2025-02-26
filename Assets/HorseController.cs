using UnityEngine;

public class HorseController : MonoBehaviour
{
    public Animator animator; // Reference to the horse's Animator
    public float gallopSpeed = 7f; // Speed when galloping
    public float acceleration = 5f; // Smooth transition between speeds

    private CharacterController controller;
    private float currentSpeed = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
        {
            animator = GetComponent<Animator>(); // Auto-assign if missing
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float targetSpeed = 0f; // Default to idle speed

        if (Input.GetKey(KeyCode.W)) // Only gallop when W is pressed
        {
            targetSpeed = gallopSpeed;
        }

        // Smoothly transition speed
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        // Move the horse forward only when galloping
        Vector3 moveDirection = transform.forward * currentSpeed * Time.deltaTime;
        controller.Move(moveDirection);

        // Update Animator
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("Galloping", currentSpeed > 0);
    }
    public float GetCurrentSpeed()
    {
        return currentSpeed; // Returns the current speed of the horse
    }

}