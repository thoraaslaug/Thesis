using StarterAssets;
using UnityEngine;

public class DismountTrigger : MonoBehaviour
{
    
    public ThirdPersonController controller;
    public HorseController horseController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure Player has "Player" tag
        {
            if (controller != null && controller.isMounted) // Check if mounted
            {
                Debug.Log("Player entered dismount area, playing animation.");
                controller.DismountHorse();
            }
        }
    }
}