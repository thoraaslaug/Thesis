using System;
using System.Collections;
using MalbersAnimations.HAP;
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
    private bool hasStartedNarration = false;

    public AudioSource source;
    
    public AudioClip[] interiorNarrationClips; // Assign 5 clips in Inspector
    public SceneFadeIn screenFadeIn;

    private void Awake()
    {
        GameState.hasStartedInteriorNarration = false; // ✅ Mark it as played
        StartCoroutine(screenFadeIn.fadeFromBlack(2));
    }

    void Update()
    {
        
        if (!hasStartedNarration && dayNightSystem != null)
        {
            StartInterorNarration();
            hasStartedNarration = true;
        }
        
       /* if (!hasOpened && dayNightSystem != null)
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
        }*/

        if (opening)
        {
            Vector3 currentRot = door.transform.localEulerAngles;
            float newY = Mathf.LerpAngle(currentRot.y, openRot, speed * Time.deltaTime);
            door.transform.localEulerAngles = new Vector3(currentRot.x, newY, currentRot.z);
            
        }
    }
    
   public void StartInterorNarration()
    {
        if (GameState.hasStartedInteriorNarration) return; // ✅ Already played, exit

        GameState.hasStartedInteriorNarration = true; // ✅ Mark it as played

        string[] narrationLines = new string[]
        {
            "The days have passed so slowly",
            "He’ll ride through anything for me… <br>I know he will.",
            "The lamp is lit. He’ll see it.",
            "I keep listening for hoofbeats… <br>but the snow eats every sound.",
            "Just get here safe… <br>please, just get here safe."
        };

        var narrationManager = FindObjectOfType<NarrationTextManager>();
        if (narrationManager != null)
        {
            narrationManager.screenFade = screenFade; // ✅ Only needed for this type of narration
            narrationManager.onNarrationComplete = OpenDoorAfterNarration;
            narrationManager.StartNarrationWithAudioAndFades(narrationLines, interiorNarrationClips, 3f);
        }

    }
    void OpenDoorAfterNarration()
    {
        //source.Play();
        opening = true;
        hasOpened = true;
        Debug.Log("🕯️ Narration finished — opening door...");
        timeline.Play();
    }
    
    public void TriggerDoorOpen()
    {
        opening = true;
        hasOpened = true;
      //  source.Play();  // Optional sound
        Debug.Log("📢 Signal received — Door is opening!");
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
        Debug.Log("Setting GameState.returnWithHorse = true");
        UnityEngine.SceneManagement.SceneManager.LoadScene("FemaleScene");

    }

}