using UnityEngine;
using UnityEngine.Playables;

public class InactivityTimelineControl : MonoBehaviour
{
    public PlayableDirector director;

    private void Start()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        if (director != null)
        {
            director.played += OnTimelineStarted;
            director.stopped += OnTimelineStopped;
        }
        else
        {
            Debug.LogWarning("❌ No PlayableDirector found!");
        }
    }

    private void OnTimelineStarted(PlayableDirector pd)
    {
        Debug.Log("🎬 Timeline started — pausing inactivity.");
        FindObjectOfType<InactivityManager>()?.PauseInactivity();
    }

    private void OnTimelineStopped(PlayableDirector pd)
    {
        var manager = FindObjectOfType<InactivityManager>();
        if (manager != null)
        {
            manager.ResumeInactivity();
            manager.ResetTimer(); // ✅ Reset inactivity timer after cutscene ends
        }

        Debug.Log("🛑 Timeline ended — resuming inactivity and resetting timer.");
    }

}