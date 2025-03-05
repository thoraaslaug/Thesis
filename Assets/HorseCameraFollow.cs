using UnityEngine;

public class HorseCameraFollow : MonoBehaviour
{
    public Transform horse; // Assign the horse GameObject in Inspector
    public Vector3 defaultOffset = new Vector3(0, 3, -6); // Default camera position
    public Vector3 mountedOffset = new Vector3(0, 2, -4); // Closer zoom when mounted
    public float followSpeed = 5f; // Smooth follow speed
    public float rotationSpeed = 5f; // Smooth rotation speed
    public float zoomSpeed = 2f; // Speed of zoom transition
    public Camera cam; // Assign the Camera in Inspector
    public float defaultFOV = 60f;
    public float mountedFOV = 50f; // Adjust FOV for a zoom effect

    private bool isMounted = false; // Track if the player is mounted

    void LateUpdate()
    {
        if (horse == null) return;

        // Choose the correct offset and FOV based on mount status
        Vector3 targetOffset = isMounted ? mountedOffset : defaultOffset;
        float targetFOV = isMounted ? mountedFOV : defaultFOV;

        // Smoothly move camera to horse position + offset
        Vector3 targetPosition = horse.position + horse.TransformDirection(targetOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate camera to match horse rotation
        Quaternion targetRotation = Quaternion.LookRotation(horse.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

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
        Debug.Log($"Camera Zoom: isMounted = {isMounted}");
    }

}