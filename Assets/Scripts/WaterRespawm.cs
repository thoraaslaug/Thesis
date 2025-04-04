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
        // 🌊 Splash effect
        if (particles != null)
        {
            particles.transform.position = playerObj.transform.position;
            particles.Play();
        }

        // 🕶️ Fade to black once
        yield return screenFade.FadeToBlack(1f);

        if (BridgeBreakSystem.HasBroken)
        {
            // 🌁 Bridge broke → load next scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("Interior");
            yield break;
        }
        else
        {
            // ❗ Disable CharacterController before teleporting
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            // 🧭 Move the player
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;

            yield return new WaitForSeconds(0.2f); // Short wait to ensure position is applied

            // ✅ Re-enable CharacterController
            if (controller != null) controller.enabled = true;

            // 🌅 Fade from black
            yield return screenFade.FadeFromBlack(1f);
        }
    }


        // Fade to black
        
        

        // Disable player
       /* ThirdPersonController playerController = playerObj.GetComponent<ThirdPersonController>();
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
        }*/

        // Fade from black
       // yield return new WaitForSeconds(0.3f);
        //yield return screenFade.FadeFromBlack(1f);
}
