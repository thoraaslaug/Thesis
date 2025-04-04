using StarterAssets;
using UnityEngine;

public class DismountTrigger : MonoBehaviour
{
    
    public ThirdPersonController controller;
    public HorseController horseController;
    public MountSystem mount;
    public HorseCameraFollow cam;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure Player has "Player" tag
        {
            if (controller != null && controller.isMounted) // Check if mounted
            {
                Debug.Log("Player entered dismount area, playing animation.");
                controller.DismountHorse();
                //mount.DetachReins();
                cam.SwitchToPlayer();
            }
        }
    }
}