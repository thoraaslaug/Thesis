using UnityEngine;
using Unity.Cinemachine;

public class HorseCameraFollow : MonoBehaviour
{
    public CinemachineCamera cinemachineCam;
    public Transform player;
    public Transform horse;
    public Transform female;

    public Vector3 unmountedOffset = new Vector3(0, 5, -10);
    public Vector3 mountedOffset = new Vector3(0, 3, -6);
    public Vector3 mountedOffsetLeft = new Vector3(0, 3, -6);
    public Vector3 mountedIdleOffset = new Vector3(1.5f, 3, -6);

    public float followSpeed = 5f;

    private Transform currentTarget;
    private Vector3 currentOffset;
    private Vector3 lastPosition;

    private bool isMounted = false;
    private bool isZoomOverridden = false;
    private Vector3 overriddenOffset;

    public CameraZoomTriggerr trigger;

    void Start()
    {
        if (GameState.followFemaleOnReturn)
        {
            currentTarget = female;
            isMounted = false;
            currentOffset = unmountedOffset;
            GameState.followFemaleOnReturn = false;
        }
        else
        {
            currentTarget = player;
            currentOffset = unmountedOffset;
        }

        if (cinemachineCam != null)
        {
            cinemachineCam.Follow = currentTarget;
        }

        lastPosition = currentTarget.position;
    }

    
    public void SwitchToTarget(Transform newTarget, bool mounted = false)
    {
        currentTarget = newTarget;
        isMounted = mounted;
        if (cinemachineCam != null)
        {
            cinemachineCam.Follow = newTarget;
        }
    }
    void LateUpdate()
    {
        if (currentTarget == null || cinemachineCam == null) return;

        // Detect movement
        Vector3 movement = currentTarget.position - lastPosition;
        float speed = movement.magnitude / Time.deltaTime;
        lastPosition = currentTarget.position;

        Vector3 targetOffset;

        if (isZoomOverridden)
        {
            targetOffset = overriddenOffset;
        }
        else if (isMounted && currentTarget == horse)
        {
            if (movement.x < -0.05f)
                targetOffset = mountedOffsetLeft;
            else if (movement.x > 0.05f)
                targetOffset = mountedOffset;
            else
                targetOffset = mountedIdleOffset;
        }
        else
        {
            targetOffset = unmountedOffset;
        }

        // Lerp to new offset
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, followSpeed * Time.deltaTime);

        // Apply manually
        Vector3 desiredPosition = currentTarget.position + currentOffset;
        cinemachineCam.transform.position = desiredPosition;
    }

    // Controls
    public void SwitchToHorse()
    {
        currentTarget = horse;
        isMounted = true;
        if (cinemachineCam != null) cinemachineCam.Follow = currentTarget;
    }

    public void SwitchToPlayer()
    {
        currentTarget = player;
        isMounted = false;
        if (cinemachineCam != null) cinemachineCam.Follow = currentTarget;
    }

    public void SwitchToFemale()
    {
        currentTarget = female;
        isMounted = false;
        if (cinemachineCam != null) cinemachineCam.Follow = currentTarget;
    }

    public void SetMounted(bool mounted)
    {
        isMounted = mounted;
    }

    public void SetZoomedOutOffset(Vector3 offset)
    {
        overriddenOffset = offset;
        isZoomOverridden = true;
    }

    public void ResetOffset(Vector3 defaultOffset)
    {
        isZoomOverridden = false;
        overriddenOffset = defaultOffset;
    }
}
