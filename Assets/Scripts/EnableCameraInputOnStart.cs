using UnityEngine;
using MalbersAnimations.Controller;

public class EnableCameraInputOnStart : MonoBehaviour
{
    private MAnimal animal;

    private void Start()
    {
        StartCoroutine(DelayedEnable(3f));
    }

    private System.Collections.IEnumerator DelayedEnable(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait one frame

        animal = GetComponent<MAnimal>();

        if (animal != null)
        {
            animal.UseCameraInput = true;

            if (animal.InputSource != null)
            {
                animal.InputSource.Enable(true);
            }

            Debug.Log($"✅ Camera Input re-enabled after delay on: {gameObject.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ MAnimal component not found on object!");
        }
    }
}
