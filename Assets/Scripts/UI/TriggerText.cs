using UnityEngine;

public class TriggerText : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<TextPopUpManager>().ShowMessage("Guðrún, Guðrún");
        }
    }

}
