using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    public float dayLengthMinutes = 1f; // One full day duration
    private float rotationSpeed;

    [Header("Current Time of Day (in seconds)")]
    public float currentTime = 0f;

    void Start()
    {
        rotationSpeed = 360f / (dayLengthMinutes * 60f); // Degrees per second
    }

    void Update()
    {
        // Track current time (wraps after one full day)
        currentTime += Time.deltaTime;
        if (currentTime > dayLengthMinutes * 60f)
        {
            currentTime = 0f;
        }

        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}