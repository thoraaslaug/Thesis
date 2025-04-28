using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using StarterAssets;

public class KissTrigger : MonoBehaviour
{
    [Header("Audio & Camera")]
    public AudioSource kissAudio;
    public CinemachineVirtualCamera vcamKissZoom;

    [Header("Post Processing")]
    public Volume postProcessingVolume;
    private DepthOfField dof;

    [Header("Lighting")]
    public Color slowMoColor = Color.gray;
    public Color normalColor = Color.white;
    private Color originalAmbientColor;
    public Light moonLight;

    [Header("Hair Swaps")]
    public GameObject normalHair;
    public GameObject deadHair;

    [Header("Characters")]
    public GameObject man;
    public GameObject woman;
    public GameObject horse;

    private Animator manAnimator;
    private Animator womanAnimator;
    private Animator horseAnimator;

    private StarterAssetsInputs inputMan;
    private StarterAssetsInputs inputWoman;

    private bool hasPlayed = false;
    
    public HorseController horseController;


    private void Start()
    {
        // Assign components
        if (postProcessingVolume != null)
            postProcessingVolume.profile.TryGet(out dof);

        originalAmbientColor = RenderSettings.ambientLight;

        if (moonLight != null)
            moonLight.enabled = false;

        if (normalHair != null) normalHair.SetActive(true);
        if (deadHair != null) deadHair.SetActive(false);

        if (man != null)
        {
            manAnimator = man.GetComponent<Animator>();
            inputMan = man.GetComponent<StarterAssetsInputs>();
        }

        if (woman != null)
        {
            womanAnimator = woman.GetComponent<Animator>();
            inputWoman = woman.GetComponent<StarterAssetsInputs>();
        }

        if (horse != null)
        {
            horseAnimator = horse.GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;

            kissAudio.Play();
            var poemDisplay = FindObjectOfType<PoemDisplayManager>();
            if (poemDisplay != null)
            {
                poemDisplay.StartPoem();
            }

            // Camera
            if (vcamKissZoom != null)
                vcamKissZoom.Priority = 20;

            // Freeze all animations
            if (horseController != null) horseController.enabled = false;
            if (manAnimator) manAnimator.enabled = false;
            if (womanAnimator) womanAnimator.enabled = false;
            if (horseAnimator) horseAnimator.enabled = false;

            // Disable input
            if (inputMan) inputMan.enabled = false;
            if (inputWoman) inputWoman.enabled = false;

            // Post Processing & Lighting
            if (dof != null) dof.active = false;
            RenderSettings.ambientLight = slowMoColor;
            if (moonLight != null) moonLight.enabled = true;

            // Swap hair
            if (normalHair != null) normalHair.SetActive(false);
            if (deadHair != null) deadHair.SetActive(true);

            StartCoroutine(ResetAfterAudio(kissAudio.clip.length));
        }
    }

    private IEnumerator ResetAfterAudio(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        // Unfreeze animations
        if (horseController != null) horseController.enabled = true;

        if (manAnimator) manAnimator.enabled = true;
        if (womanAnimator) womanAnimator.enabled = true;
        if (horseAnimator) horseAnimator.enabled = true;

        // Re-enable input
        if (inputMan) inputMan.enabled = true;
        if (inputWoman) inputWoman.enabled = true;

        // Reset camera
        if (vcamKissZoom != null)
            vcamKissZoom.Priority = 5;

        // Reset lighting
        RenderSettings.ambientLight = normalColor;
        if (moonLight != null) moonLight.enabled = false;

        // Re-enable DOF
        if (dof != null) dof.active = true;

        // Reset hair
        if (normalHair != null) normalHair.SetActive(true);
        if (deadHair != null) deadHair.SetActive(false);
    }
}
