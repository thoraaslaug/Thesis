using System.Collections;
using UnityEngine;

public class BellTrigger : MonoBehaviour
{
    public AudioSource bellSound;
    public ScreenFade screenFade; // Assign in inspector
    public GameObject player;
    private bool playerInZone = false;
    private bool hasRung = false;
    public TextPopUpManager textPopUpManager; // Assign this in the inspector
    public Transform cameraTransform; // Assign your main/virtual camera transform in the Inspector
    public float panDuration = 3f;
    public float panAngle = 20f; // How many degrees downward to tilt


    void Update()
    {
        if (playerInZone && !hasRung && ChurchTrigger.timelineHasPlayed && Input.GetKeyDown(KeyCode.E))
        {
            hasRung = true;
            StartCoroutine(RingBellAndEnd());
        }
    }
    
    private IEnumerator RingBellAndEnd()
    {
        if (bellSound != null)
            bellSound.Play();

        // 🔽 Start camera pan
        if (cameraTransform != null)
            StartCoroutine(PanCameraDown());

        yield return new WaitForSeconds(2f); // Let the bell ring

        if (screenFade != null)
            yield return screenFade.FadeToBlack(2f);

        Debug.Log("🎬 Game Over.");
        // Application.Quit() or load scene
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInZone = true;

            if (ChurchTrigger.timelineHasPlayed && textPopUpManager != null)
                textPopUpManager.ShowMessage("Press E to ring bell");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInZone = false;
            if (textPopUpManager != null)
                textPopUpManager.HideMessage();
        }
    }
    
    private IEnumerator PanCameraDown()
    {
        Vector3 initialPos = cameraTransform.position;
        Vector3 targetPos = initialPos + new Vector3(0f, -20f, 0f); // Move 2 units downward (adjust as needed)

        float elapsed = 0f;
        while (elapsed < panDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / panDuration);
            cameraTransform.position = Vector3.Lerp(initialPos, targetPos, t);
            yield return null;
        }
    }


}