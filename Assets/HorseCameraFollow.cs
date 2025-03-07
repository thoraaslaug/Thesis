using UnityEngine;

public class HorseCameraFollow : MonoBehaviour
{
    public Transform horse; // Assign the horse GameObject in Inspector
    public Vector3 unmountedOffset = new Vector3(0, 5, -10); // Offset before mounting
    public Vector3 mountedOffset = new Vector3(0, 3, -6); // Offset when mounted and moving
    public Vector3 mountedIdleOffset = new Vector3(1.5f, 3, -6); // Offset when mounted but idle (X shifted)
    public float followSpeed = 5f; // Smooth follow speed
    public float zoomSpeed = 2f; // Speed of zoom transition
    public Camera cam; // Assign the Camera in Inspector
    public float defaultFOV = 60f; // Slightly zoomed out when idle
    public float movingFOV = 55f; // Closer zoom when moving

    private Vector3 lastHorsePosition;
    private bool isMounted = false;
    private bool isMoving = false;

    void Start()
    {
        if (horse != null)
        {
            lastHorsePosition = horse.position;
        }
    }

    void LateUpdate()
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
    }

    // Call this method when player mounts or dismounts
    public void SetMounted(bool mounted)
    {
        isMounted = mounted;
        Debug.Log($"Camera State Changed: isMounted = {isMounted}");
    }
}
