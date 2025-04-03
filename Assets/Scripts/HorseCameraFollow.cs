using UnityEngine;
using UnityEngine.InputSystem;


public class HorseCameraFollow : MonoBehaviour
{
    public Transform horse;
    public Transform female; // Assign in Inspector
    private Transform currentTarget; // Currently followed object

    public Vector3 unmountedOffset = new Vector3(0, 5, -10);
    public Vector3 mountedOffset = new Vector3(0, 3, -6);
    public Vector3 mountedOffsetLeft = new Vector3(0, 2, -6);

    public Vector3 mountedIdleOffset = new Vector3(1.5f, 3, -6);
    public Vector3 currentOffset; // Stores the current camera offset

    public float followSpeed = 5f;
    public Camera cam;
    public float defaultFOV = 60f; // Slightly zoomed out when idle
    public float movingFOV = 55f; // Closer zoom when moving
    public float zoomSpeed = 2f; // Speed of zoom transition

    private Vector3 lastHorsePosition;
    private bool isMounted = false;
    private bool isMoving = false;
    public PlayerInput playerInput; // assign this in the Inspector


    void Start()
    {
        currentTarget = horse;
        if (horse != null)
        {
            lastHorsePosition = horse.position;
        }

        currentOffset = unmountedOffset;
    }

    void LateUpdate()
    {
        if (currentTarget == null) return;

        // 🧭 Calculate movement direction
        Vector3 movementDirection = currentTarget.position - lastHorsePosition;
        float movementSpeed = movementDirection.magnitude / Time.deltaTime;
        lastHorsePosition = currentTarget.position;

        isMoving = movementSpeed > 0.1f;
        float targetFOV = isMoving ? movingFOV : defaultFOV;

        // 🎯 Choose target offset based on movement direction
        Vector3 targetOffset = currentOffset;

        if (isMounted && currentTarget == horse)
        {
            float movementDirectionX = movementDirection.x;

            if (movementDirectionX < -0.05f)
            {
                targetOffset = mountedOffsetLeft;
                Debug.Log("🎥 Movement LEFT (X–)");
            }
            else if (movementDirectionX > 0.05f)
            {
                targetOffset = mountedOffset;
                Debug.Log("🎥 Movement RIGHT (X+)");
            }
            else
            {
                targetOffset = mountedIdleOffset;
                Debug.Log("🎥 Movement IDLE");
            }
        }
        else
        {
            targetOffset = unmountedOffset;
        }

        // 💫 Smooth the camera offset
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, followSpeed * Time.deltaTime);

        // 🧭 Follow target
        Vector3 targetPosition = currentTarget.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // 🎥 Adjust camera FOV smoothly
        if (cam != null)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        }
    }


    public void SetMounted(bool mounted)
    {
        isMounted = mounted;
        Debug.Log($"Camera State Changed: isMounted = {isMounted}");
    }

    public void SetZoomedOutOffset(Vector3 newOffset)
    {
        Debug.Log("Setting new camera offset: " + newOffset);
        currentOffset = newOffset;
    }

    public void ResetOffset(Vector3 defaultOffset)
    {
        Debug.Log("Resetting camera offset to: " + defaultOffset);
        currentOffset = defaultOffset;
    }

    public void SwitchToFemale()
    {
        currentTarget = female;
        isMounted = false; // Optional reset
        Debug.Log("📸 Camera now following female.");
    }
} 
