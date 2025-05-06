using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DeactivatOnSignal : MonoBehaviour, INotificationReceiver
{
    public GameObject[] targets;  // Assign all the objects to deactivate in the inspector

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is SignalEmitter)
        {
            foreach (GameObject target in targets)
            {
                if (target != null)
                {
                    target.SetActive(false);
                    Debug.Log("📴 Deactivated via signal: " + target.name);
                }
            }
        }
    }
}