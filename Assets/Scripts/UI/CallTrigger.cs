using System.Collections;
using MalbersAnimations.Controller;
using MalbersAnimations.HAP;
using UnityEngine;

public class CallTrigger : MonoBehaviour
{
    public MRider rider;
    public float idleThreshold = 0.05f;
    public float idleTime = 5f;
    public TextPopUpManager popupManager;

    private float idleTimer = 0f;
    private bool hintShown = false;
    private Vector3 lastPosition;
    public float visibleDuration = 2f;
    public float fadeDuration = 1f;
    public NarrationTextManager narrationManager;  // assign in Inspector
    public UnityEngine.Playables.PlayableDirector timelineDirector;


    void Start()
    {
        if (rider != null)
            lastPosition = rider.transform.position;
    }

    void Update()
    {
        if (rider == null || rider.IsRiding) return;

        Vector3 currentPos = rider.transform.position;
        float speed = (currentPos - lastPosition).magnitude / Time.deltaTime;

        // Debug logs
        //Debug.Log($"Speed: {speed}");

        if (speed < idleThreshold)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTime && !hintShown &&
               (narrationManager == null || !narrationManager.isNarrating) &&
               (timelineDirector == null || timelineDirector.state != UnityEngine.Playables.PlayState.Playing))
            {
                popupManager.ShowMessage("Hold E to call horse");
                hintShown = true;
                Debug.Log("✅ Showing call horse message");
                StartCoroutine(ResetHintShown());
            }
        }
        else
        {
            idleTimer = 0f;
            hintShown = false;
        }

        lastPosition = currentPos;
    }
    
    private IEnumerator ResetHintShown()
    {
        yield return new WaitForSeconds(visibleDuration + fadeDuration + 1f);
        hintShown = false;
    }
}
