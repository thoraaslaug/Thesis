using UnityEngine;

public class KissTrigger : MonoBehaviour
{
    public Animator playerAnimator;        // Assign Player's Animator
    public Animator partnerAnimator;       // Assign the other character's Animator
    public string triggerName = "Kiss";    // Make sure both animators have this trigger

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;

        if (other.CompareTag("Player")) // Make sure the Player has this tag
        {
            Debug.Log("Player reached the partner! Starting kiss animation.");

            hasPlayed = true;

            // Trigger kiss animation on both characters
            playerAnimator.SetTrigger(triggerName);
            partnerAnimator.SetTrigger(triggerName);
        }
    }
}
