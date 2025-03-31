using UnityEngine;
using UnityEngine.VFX;

public class SnowIntensityByTime : MonoBehaviour
{
    public VisualEffect snowVFX;
    public DayNightSystem dayNightSystem;

    [Header("Snow Settings")]
    public float daySpawnRate = 100f;
    public float nightSpawnRate = 600f;

    public float daySpeed = 1f;
    public float nightSpeed = 4f;

    void Update()
    {
        if (snowVFX == null || dayNightSystem == null) return;

        // Normalize time (0 to 1 over full day)
        float normalizedTime = dayNightSystem.currentTime / (dayNightSystem.dayLengthMinutes * 60f);
        float timeAngle = normalizedTime * 360f;

        // Consider 0–180 as day, 180–360 as night
        float dayWeight = Mathf.Cos(timeAngle * Mathf.Deg2Rad) * 0.5f + 0.5f; // 1 at noon, 0 at midnight
        float nightWeight = 1f - dayWeight;

        float currentRate = Mathf.Lerp(daySpawnRate, nightSpawnRate, nightWeight);
        float currentSpeed = Mathf.Lerp(daySpeed, nightSpeed, nightWeight);

        snowVFX.SetFloat("SpawnRate", currentRate);
        snowVFX.SetFloat("SnowSpeed", currentSpeed);
    }
}