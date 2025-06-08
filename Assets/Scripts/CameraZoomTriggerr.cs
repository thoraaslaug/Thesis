using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class CameraZoomTriggerr : MonoBehaviour
{
    public HorseCameraFollow cameraFollow; // Assign in Inspector
    public Vector3 zoomOutOffset = new Vector3(0, 8, -15); // New zoomed-out camera offset
    public Vector3 normalOffset; // Stores the default offset to reset later
    public CinemachineCamera vcamKissZoom;
    [Header("Post Processing")] public Volume postProcessingVolume;
    private DepthOfField dof;
    private void Start()
    {
        if (cameraFollow != null)
        {
            normalOffset = cameraFollow.unmountedOffset; // Save the default offset
        }
        if (postProcessingVolume != null)
                         postProcessingVolume.profile.TryGet(out dof);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (vcamKissZoom != null)
                vcamKissZoom.Priority = 20;
            
            if (dof != null)
            {
                dof.active = true;
                dof.focusDistance.value = 7.2f;
                dof.aperture.value = 5.4f;
                dof.focalLength.value = 96f;
            }
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
            
             if (dof != null)
            {
                dof.active = true;
                dof.focusDistance.value = 7.2f;
                dof.aperture.value = 5.4f;
                dof.focalLength.value = 106f;
            }
            
            if (vcamKissZoom != null)
                vcamKissZoom.Priority = 3;
            if (dof != null) dof.active = true;
            Debug.Log("Player exited zoom trigger area!");
            if (cameraFollow != null)
            {
                cameraFollow.ResetOffset(normalOffset);
            }
        }
    }
}