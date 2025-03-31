using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    public float dayLengthMinutes = 1f; // One full day duration
    private float rotationSpeed;

    void Start()
    {
        rotationSpeed = 360f / (dayLengthMinutes * 60f); // Degrees per second
    }

    void Update()
    {
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}