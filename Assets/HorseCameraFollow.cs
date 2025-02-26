using UnityEngine;

public class HorseCameraFollow : MonoBehaviour
{
    public Transform horse; // Assign the horse GameObject in Inspector
    public Vector3 offset = new Vector3(0, 3, -6); // Offset behind and above the horse
    public float followSpeed = 5f; // Smooth follow speed
    public float rotationSpeed = 5f; // Smooth rotation speed

    void LateUpdate()
    {
        if (horse == null) return;

        // Smoothly move camera to horse position + offset
        Vector3 targetPosition = horse.position + horse.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate camera to match horse rotation
        Quaternion targetRotation = Quaternion.LookRotation(horse.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}