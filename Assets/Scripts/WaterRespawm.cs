using UnityEngine;
using System.Collections;
using StarterAssets;

public class WaterRespawn : MonoBehaviour
{
    public Transform spawnPoint;
    public ParticleSystem particles;

    public GameObject player;
    public GameObject female;
    public Camera mainCamera;
    public ScreenFade screenFade;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleFallSequence(other.gameObject));
        }
    }

    private IEnumerator HandleFallSequence(GameObject playerObj)
    {
        // 🌊 Splash effect
        if (particles != null)
        {
            particles.transform.position = playerObj.transform.position;
            particles.Play();
        }

        yield break;

    }


}
