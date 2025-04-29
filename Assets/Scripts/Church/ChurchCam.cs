using UnityEngine;

public class ChurchCam : MonoBehaviour
{
    public HorseCameraFollow cameraFollow; // Assign in Inspector
    public Vector3 zoomOutOffset = new Vector3(0, 8, -15); // New zoomed-out camera offset
    public Vector3 normalOffset; // Stores the default offset to reset later

    public float newRotationY = 90f; // ⬅️ New desired Y rotation when triggered
    private float originalRotationY; // To restore original later

    private void Start()
    {
        if (cameraFollow != null)
        {
            normalOffset = cameraFollow.unmountedOffset; // Save the default offset
            originalRotationY = cameraFollow.transform.eulerAngles.y; // Save original rotation
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
                RotateCameraTo(newRotationY);
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
                RotateCameraTo(originalRotationY);
            }
        }
    }

    private void RotateCameraTo(float yRotation)
    {
        Vector3 currentEuler = cameraFollow.transform.eulerAngles;
        cameraFollow.transform.rotation = Quaternion.Euler(currentEuler.x, yRotation, currentEuler.z);
    }
}