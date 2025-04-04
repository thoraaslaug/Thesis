using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public GameObject door;
    public float openRot = 90f;
    public float closeRot = 0f;
    public float speed = 2f;
    public bool opening = false;

    [Header("Open Settings")]
    public DayNightSystem dayNightSystem;
    public float openAfterDays = 5f;

    private bool hasOpened = false;

    void Update()
    {
        if (!hasOpened && dayNightSystem != null)
        {
            float daysPassed = dayNightSystem.currentTime / (dayNightSystem.dayLengthMinutes * 60f);
            if (daysPassed >= openAfterDays)
            {
                opening = true;
                hasOpened = true;
                Debug.Log("🚪 Door should now open after 5 days!");
            }
        }

        if (opening)
        {
            Vector3 currentRot = door.transform.localEulerAngles;
            float newY = Mathf.LerpAngle(currentRot.y, openRot, speed * Time.deltaTime);
            door.transform.localEulerAngles = new Vector3(currentRot.x, newY, currentRot.z);
        }
    }
}