using System;
using UnityEngine;
using MalbersAnimations.Controller;
using MalbersAnimations.HAP;
using UnityEngine.SceneManagement;

public class EnableCameraInputOnStart : MonoBehaviour
{
    private MAnimal animal;
    public MRider rider;
    private bool hasStartedSceneNarration = false;
    public AudioClip[] femaleSceneClips; // Assign 5 clips in Inspector


    private void Start()
    {

        if (rider.Mounted)
        {
            // forcibly stop dismount if needed
            if (Input.GetKeyDown(KeyCode.E))  // or use Input System action
            {
                Debug.Log("❌ Dismount blocked!");
            }
        }        StartCoroutine(DelayedEnable(2f));
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "FemaleScene" && !hasStartedSceneNarration)
        {
            hasStartedSceneNarration = true;

            string[] lines = {
                "He really came for me...",
                "After all that snow... I didn’t think..",
                "It’s strange though... he hasn’t said a word.",
            };

            AudioClip[] voiceClips = femaleSceneClips; // assign this in inspector, length = 3

            var narration = FindObjectOfType<NarrationTextManager>();
            if (narration != null)
            {
                narration.StartNarrationWithAudio(lines, voiceClips, delayBetweenLines: 3f, startDelay: 5f);
            }
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
