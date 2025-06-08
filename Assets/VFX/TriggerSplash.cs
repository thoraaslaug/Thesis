using UnityEngine;

public class WaterSplashTrigger : MonoBehaviour
{
    public ParticleSystem splashEffect; // Single Particle System (assign in Inspector)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rock")) // Ensure bridge parts have this tag
        {
            //ebug.Log($"Bridge part {other.gameObject.name} hit the water!");

            PlaySplashEffect(other.transform.position);
        }
    }

    public void PlaySplashEffect(Vector3 position)
    {
        if (splashEffect != null)
        {
            splashEffect.transform.position = position; // Move effect to splash point
            splashEffect.Play(); // Play the Particle System
        }
        else
        {
            Debug.LogWarning("No Particle System assigned to WaterSplashTrigger!");
        }
    }
}