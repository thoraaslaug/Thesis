using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DoorOpener : MonoBehaviour
{
    public GameObject door;
    public float openRot = 90f;
    public float closeRot = 0f;
    public float speed = 2f;
    public bool opening = false;

    [Header("Open Settings")]
    public DayNightSystem dayNightSystem;
    public float openAfterDays = 5f;

    private bool hasOpened = false;

    public PlayableDirector timeline;
    public ScreenFade screenFade;


    void Update()
    {
        if (!hasOpened && dayNightSystem != null)
        {
            float daysPassed = dayNightSystem.currentTime / (dayNightSystem.dayLengthMinutes * 60f);
            if (daysPassed >= openAfterDays)
            {
                opening = true;
                hasOpened = true;
                Debug.Log("🚪 Door should now open after 5 days!");
                timeline.Play();
                //StartFadeFromSignal();
                //StartCoroutine(WaitForTimelineAndFade()); // ⏳ Start coroutine


            }
        }

        if (opening)
        {
            Vector3 currentRot = door.transform.localEulerAngles;
            float newY = Mathf.LerpAngle(currentRot.y, openRot, speed * Time.deltaTime);
            door.transform.localEulerAngles = new Vector3(currentRot.x, newY, currentRot.z);
        }
    }
    
    
  /*  private IEnumerator WaitForTimelineAndFade()
    {
        // Wait for the timeline to finish playing
        while (timeline.state == PlayState.Playing)
        {
            yield return null;
        }

        // 👩 Switch camera to follow the female
        HorseCameraFollow camFollow = Camera.main.GetComponent<HorseCameraFollow>();
        if (camFollow != null)
        {
            camFollow.SwitchToFemale();
            Debug.Log("📸 Switched camera to follow female.");
        }

        yield return new WaitForSeconds(2f); // <- adjust this as needed

        // Fade to black
        yield return screenFade.FadeToBlack(1f);

        // Load the next scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }*/
    
    public void StartFadeFromSignal()
    {
        StartCoroutine(FadeAndPrepareSceneSwitch());
    }
    
    private IEnumerator FadeAndPrepareSceneSwitch()
    {
        // 👩 Switch camera before fade
        HorseCameraFollow camFollow = Camera.main.GetComponent<HorseCameraFollow>();
        if (camFollow != null)
        {
            camFollow.SwitchToFemale();
        }

        // 🌑 Start the fade
        yield return screenFade.FadeToBlack(1f);
        GameState.followFemaleOnReturn = true;
        GameState.returnWithHorse = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

    }

}