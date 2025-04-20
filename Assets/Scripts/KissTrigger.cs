using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KissTrigger : MonoBehaviour
{
    public AudioSource kissAudio;
    public CinemachineVirtualCamera vcamKissZoom;

    public Volume postProcessingVolume; // Assign in inspector
    private DepthOfField dof;

    public Color slowMoColor = Color.gray;
    public Color normalColor = Color.white;
    private Color originalAmbientColor;

    private bool hasPlayed = false;
    
    public Light moonLight; // Assign in Inspector

    public GameObject normalHair;
    public GameObject deadHair;


    private void Start()
    {
        normalHair.SetActive(true);
        deadHair.SetActive(false);
        moonLight.enabled = false;
        // Get reference to the Depth of Field override
        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out dof);
        }

        // Store original ambient light color
        originalAmbientColor = RenderSettings.ambientLight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;
            kissAudio.Play();

            // Zoom in camera
            vcamKissZoom.Priority = 20;

            // Slow motion
            Time.timeScale = 0.4f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            // Turn off Depth of Field
            if (dof != null)
                dof.active = false;

            // Darken the world
            RenderSettings.ambientLight = slowMoColor;
            moonLight.enabled = true;
            normalHair.SetActive(false);
            deadHair.SetActive(true);

            StartCoroutine(ResetSceneAfterAudio(kissAudio.clip.length));
        }
    }

    IEnumerator ResetSceneAfterAudio(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        // Reset camera priority
        vcamKissZoom.Priority = 5;

        // Reset time scale
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // Turn Depth of Field back on
        if (dof != null)
            dof.active = true;

        // Reset ambient lighting
        RenderSettings.ambientLight = originalAmbientColor;
        moonLight.enabled = false;
        normalHair.SetActive(true);
        deadHair.SetActive(false);
    }
}