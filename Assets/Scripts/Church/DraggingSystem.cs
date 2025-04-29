using UnityEngine;
using System.Collections;
using StarterAssets;

public class DraggingSystem : MonoBehaviour
{
    [Header("References")]
    public Transform woman;     // assign woman
    public Transform man;       // assign man
    public Animator manAnimator;
    public Animator womanAnimator;
    public CharacterController womanController; // for movement
    public ThirdPersonController womanInput;    // for input disabling

    [Header("Dragging Settings")]
    public float dragSpeed = 1.5f;
    public float dragDistance = 5f; // how far before woman can break free
    public float struggleThreshold = 5f; // how much struggle is needed
    public float struggleDecayRate = 1f; // how fast struggle decays if not pressing

    private float currentStruggle = 0f;
    private bool isDragging = false;
    private bool isFreed = false;

    void Update()
    {
        if (!isDragging || isFreed)
            return;

        // Move the woman toward the grave slowly
        Vector3 dragDirection = (man.position - woman.position).normalized;
        womanController.Move(dragDirection * dragSpeed * Time.deltaTime);

        // Allow player to "struggle" out
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentStruggle += 1f;
            Debug.Log($"Struggle! Current: {currentStruggle}/{struggleThreshold}");
        }
        else
        {
            // slowly decay if not pressing
            currentStruggle -= struggleDecayRate * Time.deltaTime;
            currentStruggle = Mathf.Max(currentStruggle, 0f);
        }

        if (currentStruggle >= struggleThreshold)
        {
            BreakFree();
        }
    }

    public void StartDragging()
    {
        isDragging = true;
        isFreed = false;
        currentStruggle = 0f;

        if (womanInput != null)
            womanInput.enabled = false; // disable player movement temporarily

        if (manAnimator != null)
            manAnimator.SetBool("IsDragging", true);

        if (womanAnimator != null)
            womanAnimator.SetBool("IsDragged", true);

        Debug.Log("👨‍🦳 Dragging started!");
    }

    private void BreakFree()
    {
        isDragging = false;
        isFreed = true;

        if (womanInput != null)
            womanInput.enabled = true; // re-enable player movement!

        if (manAnimator != null)
            manAnimator.SetBool("IsDragging", false);

        if (womanAnimator != null)
            womanAnimator.SetBool("IsDragged", false);

        Debug.Log("🙌 Broke free!");
    }
}
