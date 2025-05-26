using System;
using UnityEngine;
using MalbersAnimations.Controller;

public class EnableCameraInputOnStart : MonoBehaviour
{
    private MAnimal animal;

    private void Awake()
    {
        animal = GetComponent<MAnimal>();

        if (animal != null)
        {
            animal.UseCameraInput = true;

            if (animal.InputSource != null)
            {
                animal.InputSource.Enable(true);
            }

            Debug.Log($"✅ Camera Input enabled for {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ No MAnimal component found on {gameObject.name}");
        }
    }
    
}
