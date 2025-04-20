using System.Collections;
using UnityEngine;
using Cinemachine;

public class KissTrigger : MonoBehaviour
{
    public AudioSource kissAudio;
    public CinemachineVirtualCamera vcamKissZoom;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;
            kissAudio.Play();

            // Zoom in & slow mo
            vcamKissZoom.Priority = 20;
            Time.timeScale = 0.4f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            StartCoroutine(ResetSceneAfterAudio(kissAudio.clip.length));
        }
    }

    IEnumerator ResetSceneAfterAudio(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        // Reset zoom and time
        vcamKissZoom.Priority = 5;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}