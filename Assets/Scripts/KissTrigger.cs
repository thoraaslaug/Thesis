using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using StarterAssets;
using Unity.Cinemachine;
using MalbersAnimations.Controller;

namespace MalbersAnimations.HAP
{
    public class KissTrigger : MonoBehaviour
    {
        [Header("Audio & Camera")] public AudioSource kissAudio;
        public CinemachineCamera vcamKissZoom;

        [Header("Post Processing")] public Volume postProcessingVolume;
        private DepthOfField dof;

        [Header("Lighting")] public Color slowMoColor = Color.gray;
        public Color normalColor = Color.white;
        private Color originalAmbientColor;
        public Light moonLight;

        [Header("Hair Swaps")] public GameObject normalHair;
        public GameObject deadHair;

        [Header("Characters")] public GameObject man;
        public GameObject woman;
        public GameObject horse;

        private Animator manAnimator;
        private Animator womanAnimator;
        private Animator horseAnimator;

        private StarterAssetsInputs inputMan;
        private StarterAssetsInputs inputWoman;
        public PoemDisplayManager poemDisplayManager;

        [Header("Malbers Horse")] public MAnimal malbersHorse; // Reference to Malbers' MAnimal script
        private bool originalGravity;
        private bool hasPlayed = false;
        
        //public Animator horseAnimator;
        public Rigidbody horseRigidbody;

        private void Start()
        {
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

            if (malbersHorse != null)
            {
                originalGravity = malbersHorse.enabled;
            }
        }
        
        public void OnHorseMovementLocked(bool isLocked)
        {
            if (horseAnimator != null)
                horseAnimator.enabled = !isLocked;

            if (horseRigidbody != null)
            {
                if (isLocked)
                {
                    horseRigidbody.linearVelocity = Vector3.zero;
                    horseRigidbody.angularVelocity = Vector3.zero;
                    horseRigidbody.isKinematic = true; // stops physics from continuing motion
                }
                else
                {
                    horseRigidbody.isKinematic = false; // restore normal physics
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!hasPlayed && other.CompareTag("Player"))
            {
                hasPlayed = true;
                kissAudio.Play();

                //poemDisplayManager.StartPoem();

                // Camera zoom
                if (vcamKissZoom != null)
                    vcamKissZoom.Priority = 20;

                // Freeze characters
                if (inputMan) inputMan.enabled = false;
                if (inputWoman) inputWoman.enabled = false;
                if (manAnimator) manAnimator.enabled = false;
                if (womanAnimator) womanAnimator.enabled = false;
                if (horseAnimator) horseAnimator.enabled = false;

                // Freeze Malbers horse
                if (malbersHorse != null)
                {
                    malbersHorse.LockMovement = true;
                    malbersHorse.LockInput = true;
                    malbersHorse.StopMoving();
                }
                /*  malbersHorse.Gravity = false;

                  if (malbersHorse.AI != null)
                      malbersHorse.AI.enabled = false;
              }*/

                // Post Processing & lighting
                if (dof != null) dof.active = false;
                RenderSettings.ambientLight = slowMoColor;
                if (moonLight != null) moonLight.enabled = true;

                // Hair swap
                if (normalHair != null) normalHair.SetActive(false);
                if (deadHair != null) deadHair.SetActive(true);

                poemDisplayManager.StartPoem(() =>
                {
                    StartCoroutine(ResetAfterAudio(0f));
                });            }
        }

        private IEnumerator ResetAfterAudio(float duration)
        {
            yield return new WaitForSeconds(0f);

            // Re-enable characters
            if (inputMan) inputMan.enabled = true;
            if (inputWoman) inputWoman.enabled = true;
            if (manAnimator) manAnimator.enabled = true;
            if (womanAnimator) womanAnimator.enabled = true;
            if (horseAnimator) horseAnimator.enabled = true;

            // Re-enable horse
            if (malbersHorse != null)
            {
                malbersHorse.LockMovement = false;
                malbersHorse.LockInput = false;
                //  malbersHorse.Gravity = originalGravity;

                /*if (malbersHorse.AI != null)
                    malbersHorse.AI.enabled = true;
            }*/

                // Reset camera
                if (vcamKissZoom != null)
                    vcamKissZoom.Priority = 5;

                // Reset lighting
                RenderSettings.ambientLight = normalColor;
                if (moonLight != null) moonLight.enabled = false;

                // Restore post-processing
                if (dof != null) dof.active = true;

                // Hair reset
                if (normalHair != null) normalHair.SetActive(true);
                if (deadHair != null) deadHair.SetActive(false);
            }
        }
    }
}