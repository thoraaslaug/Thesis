using UnityEngine;

public class MountText : MonoBehaviour
{
    public TextPopUpManager popupManager;  // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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

