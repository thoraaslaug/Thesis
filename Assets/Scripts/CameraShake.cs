using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public bool isShaking = false;
    private Vector3 shakeOffset = Vector3.zero;

    public float magnitude = 0.5f;
    public float duration = 1f;

    private float elapsed = 0f;

    public void StartShake(float dur, float mag)
    {
        duration = dur;
        magnitude = mag;
        elapsed = 0f;
        isShaking = true;
    }

    void LateUpdate()
    {
        if (isShaking)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= duration)
            {
                isShaking = false;
                shakeOffset = Vector3.zero;
            }
            else
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                shakeOffset = new Vector3(x, y, 0f);
            }

            // Add offset to camera's current position
            transform.localPosition += shakeOffset;
        }
    }
}