using System;
using UnityEngine;
using MalbersAnimations.Controller;
using UnityEngine.SceneManagement;

public class EnableCameraInputOnStart : MonoBehaviour
{
    private MAnimal animal;
    private bool hasStartedSceneNarration = false;


    private void Start()
    {
        StartCoroutine(DelayedEnable(3f));
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "FemaleScene" && !hasStartedSceneNarration)
        {
            hasStartedSceneNarration = true;
            string[] lines = {
                "He really came for me...",
                "After all that snow... I didn’t think..",
                "It’s strange though... he hasn’t said a word.",
            };

            var narration = FindObjectOfType<NarrationTextManager>();
            if (narration != null)
                narration.StartNarration(lines);
        }
    }

  
    public System.Collections.IEnumerator DelayedEnable(float delay)
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
