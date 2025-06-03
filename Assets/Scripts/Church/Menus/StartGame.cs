using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using MalbersAnimations.HAP;  // For MRider

public class StartGame : MonoBehaviour
{
    public string sceneToLoad = "SampleScene";
    public ScreenFade screenFade;
    public float fadeDuration = 1f;
    private bool hasStarted = false;

    public TextMeshProUGUI[] menuTexts;

    [Header("Mount Setup")]
    public MRider rider;  // Assign in inspector
    public GameObject horse; // Optional, just to ensure visuals are ready

    private void Start()
    {
        // Hide texts initially
        foreach (var text in menuTexts)
        {
            Color c = text.color;
            text.color = new Color(c.r, c.g, c.b, 0f);
        }

        StartCoroutine(InitMenuSequence());
    }

    private IEnumerator InitMenuSequence()
    {
        yield return new WaitForSeconds(2f); // Delay after game starts

      /*  if (rider == null || horse == null)
        {
            Debug.LogError("Rider or Horse not assigned.");
            yield break;
        }*/

        // 🔧 Set up mount reference
      //  rider.Set_StoredMount(horse);

        // 🔁 Let the system process mount data
        //yield return null;

        // ✅ Check if mounting is possible (makes CanMount true if within valid range)
       // rider.UpdateCanMountDismount();

        if (rider.CanMount)
        {
            // ✅ Use Malbers' full mount logic
            rider.MountAnimal();
        }
        else
        {
            Debug.LogWarning("Rider cannot mount. Check mount trigger range and setup.");
        }
        yield return new WaitForSeconds(1f); // Optional buffer after mounting


        StartCoroutine(FadeInMenuTexts());
    }




    public void StartPlay()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(FadeAndLoad());
        }
    }

    private IEnumerator FadeAndLoad()
    {
        foreach (var text in menuTexts)
        {
            StartCoroutine(FadeTextOut(text));
        }

        if (screenFade != null)
        {
            yield return screenFade.FadeToBlack(fadeDuration);
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeInMenuTexts()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);

            foreach (var text in menuTexts)
            {
                Color c = text.color;
                text.color = new Color(c.r, c.g, c.b, alpha);
            }

            yield return null;
        }
    }

    private IEnumerator FadeTextOut(TextMeshProUGUI text)
    {
        float t = 0f;
        Color original = text.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            text.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }
    }
}
