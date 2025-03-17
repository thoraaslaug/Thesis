using UnityEngine;

public class CameraZoomTrigger : MonoBehaviour
{
    public HorseCameraFollow cameraFollow; // Assign in Inspector
    public Vector3 zoomOutOffset = new Vector3(0, 8, -15); // New zoomed-out camera offset
    public Vector3 normalOffset; // Stores the default offset to reset later

    private void Start()
    {
        if (cameraFollow != null)
        {
            normalOffset = cameraFollow.unmountedOffset; // Save the default offset
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered zoom trigger area!");
            if (cameraFollow != null)
            {
                cameraFollow.SetZoomedOutOffset(zoomOutOffset);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited zoom trigger area!");
            if (cameraFollow != null)
            {
                cameraFollow.ResetOffset(normalOffset);
            }
        }
    }
}