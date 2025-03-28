using UnityEngine;
using System.Collections;
using StarterAssets;

public class WaterRespawn : MonoBehaviour
{
    public Transform spawnPoint;
    public ParticleSystem particles;

    public GameObject player;
    public GameObject female;
    public Camera mainCamera;
    public ScreenFade screenFade;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleFallSequence(other.gameObject));
        }
    }

    private IEnumerator HandleFallSequence(GameObject playerObj)
    {
        // Play splash effect
        if (particles != null)
        {
            particles.transform.position = playerObj.transform.position;
            particles.Play();
        }

        // Fade to black
        yield return screenFade.FadeToBlack(1f);

        // Disable player
        ThirdPersonController playerController = playerObj.GetComponent<ThirdPersonController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        CharacterController controller = playerObj.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        playerObj.SetActive(false);

        // Enable female character and controller
        if (female != null)
        {
            female.SetActive(true);

            ThirdPersonController femaleController = female.GetComponent<ThirdPersonController>();
            if (femaleController != null)
            {
                femaleController.enabled = true;
            }

            // Switch camera follow to female
            HorseCameraFollow camFollow = mainCamera.GetComponent<HorseCameraFollow>();
            if (camFollow != null)
            {
                camFollow.SwitchToFemale();
            }
        }

        // Fade from black
        yield return new WaitForSeconds(0.3f);
        yield return screenFade.FadeFromBlack(1f);
    }
}