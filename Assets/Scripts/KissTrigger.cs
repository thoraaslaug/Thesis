using UnityEngine;
using UnityEngine.Playables;

public class KissTrigger : MonoBehaviour
{
    public PlayableDirector kissTimeline;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            kissTimeline.Play();
        }
    }
}