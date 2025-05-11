using UnityEngine;

public class GhostRevealTrigger : MonoBehaviour
{
    public NarrationTextManager narrationManager;
    
    [TextArea]
    public string[] ghostRevealLines = {
        "Garun... why can't he say",
        "No. No, it can’t be",
        "He’s dead. He’s been dead this whole time.",
        "I have to get away. I have to get to the church."
    };

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            narrationManager.StartNarration(ghostRevealLines);
        }
    }
}