using Unity.Cinemachine;
using UnityEngine;


public class CameraZoomTriggerr : MonoBehaviour
{
    public HorseCameraFollow cameraFollow; // Assign in Inspector
    public Vector3 zoomOutOffset = new Vector3(0, 8, -15); // New zoomed-out camera offset
    public Vector3 normalOffset; // Stores the default offset to reset later
    public CinemachineCamera vcamKissZoom;
 
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
            
            if (vcamKissZoom != null)
                vcamKissZoom.Priority = 20;
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
            
            if (vcamKissZoom != null)
                vcamKissZoom.Priority = 3;
            Debug.Log("Player exited zoom trigger area!");
            if (cameraFollow != null)
            {
                cameraFollow.ResetOffset(normalOffset);
            }
        }
    }
}