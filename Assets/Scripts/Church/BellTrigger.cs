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

        yield return new WaitForSeconds(2f); // Let the bell sound

        if (screenFade != null)
            yield return screenFade.FadeToBlack(2f);

        Debug.Log("🎬 Game Over.");
        // Add Application.Quit(); or load end scene
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
}