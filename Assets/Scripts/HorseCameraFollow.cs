using UnityEngine;

public class HorseCameraFollow : MonoBehaviour
{
    public Transform horse;
    public Vector3 unmountedOffset = new Vector3(0, 5, -10);
    public Vector3 mountedOffset = new Vector3(0, 3, -6);
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

    void Start()
    {
        if (horse != null)
        {
            lastHorsePosition = horse.position;
        }

        currentOffset = unmountedOffset;
    }

    void LateUpdate()
    {
        if (horse == null) return;

        // Calculate movement speed
        float movementSpeed = (horse.position - lastHorsePosition).magnitude / Time.deltaTime;
        lastHorsePosition = horse.position;

        // Check if the horse is moving
        isMoving = movementSpeed > 0.1f;
        float targetFOV = isMoving ? movingFOV : defaultFOV;

        // Adjust offset based on movement
        if (isMounted)
        {
            currentOffset = isMoving ? mountedOffset : mountedIdleOffset;
        }

        // Smoothly transition camera position
        Vector3 targetPosition = horse.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        
        if (cam != null)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        }
    }
    
 /*   void LateUpdate()
    {
        if (horse == null) return;

        // Calculate movement speed
        float movementSpeed = (horse.position - lastHorsePosition).magnitude / Time.deltaTime;
        lastHorsePosition = horse.position; // Update last position

        // Check if the horse is moving
        isMoving = movementSpeed > 0.1f;

        // Determine the correct offset based on mounted state and movement
        Vector3 targetOffset;
        if (!isMounted)
        {
            targetOffset = unmountedOffset;
        }
        else
        {
            // Use different X offsets based on movement
            targetOffset = isMoving ? mountedOffset : mountedIdleOffset;
        }

        float targetFOV = isMoving ? movingFOV : defaultFOV;

        // Smoothly transition camera position
        Vector3 targetPosition = horse.position + targetOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Smoothly transition FOV for zoom effect
        if (cam != null)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        }
    }*/

    public void SetMounted(bool mounted)
    {
        isMounted = mounted;
        Debug.Log($"Camera State Changed: isMounted = {isMounted}");
    }

    // ✅ New method to adjust offset when entering zoom trigger
    public void SetZoomedOutOffset(Vector3 newOffset)
    {
        Debug.Log("Setting new camera offset: " + newOffset);
        currentOffset = newOffset;
    }

    // ✅ New method to reset to default offset when leaving trigger
    public void ResetOffset(Vector3 defaultOffset)
    {
        Debug.Log("Resetting camera offset to: " + defaultOffset);
        currentOffset = defaultOffset;
    }
}
