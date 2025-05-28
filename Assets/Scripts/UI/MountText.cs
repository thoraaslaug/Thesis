using MalbersAnimations.HAP;
using UnityEngine;

public class MountText : MonoBehaviour
{
    public TextPopUpManager popupManager;  // Assign in Inspector
    public MRider rider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !rider.Mounted)
        {
            popupManager?.ShowMessage("Press E to mount");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popupManager?.HideMessage();
        }
    }
}

