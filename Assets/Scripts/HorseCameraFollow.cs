using UnityEngine;

public class HorseCameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform horse;
    public Transform female; // Assign in Inspector
    private Transform currentTarget; // Currently followed object

    public Vector3 unmountedOffset = new Vector3(0, 5, -10);
    public Vector3 mountedOffset = new Vector3(0, 3, -6);
    public Vector3 mountedOffsetLeft = new Vector3(0, 3, -6);

    public Vector3 mountedIdleOffset = new Vector3(1.5f, 3, -6);
    public Vector3 currentOffset; // Stores the current camera offset

    public float followSpeed = 5f;
    public Camera cam;
    public float defaultFOV = 60f; // Slightly zoomed out when idle
    public float movingFOV = 55f; // Closer zoom when moving
    public float zoomSpeed = 2f; // Speed of zoom transition

    private Vector3 lastHorsePosition;
    public bool isMounted = false;
    private bool isMoving = false;
    public CameraZoomTriggerr trigger;
    private bool isZoomOverridden = false;
    private Vector3 overriddenOffset;
    
    public Vector3 shakeOffset = Vector3.zero;



    void Start()
    {
        if (GameState.followFemaleOnReturn)
        {
            currentTarget = female;
            isMounted = false;
            currentOffset = unmountedOffset;

            Debug.Log("📸 Returned to Scene A — following female!");
        
            // Optional: Reset flag to avoid affecting later loads
            GameState.followFemaleOnReturn = false;
        }
        else
        {
            currentTarget = player;
            currentOffset = unmountedOffset;
            isMounted = false;
            Debug.Log("📸 Scene A normal start — following player!");
        }
    }
    public void SwitchToHorse()
    {
        currentTarget = horse;
        isMounted = true;
       // Debug.Log("📸 Camera now following HORSE");
    }

    public void SwitchToPlayer()
    {
        currentTarget = player;
        isMounted = false;
       // Debug.Log("📸 Camera now following PLAYER");
    }
    
    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }
    public void SetShakeOffset(Vector3 offset)
    {
        shakeOffset = offset;
    }
    
   
    void LateUpdate()
    {
        if (currentTarget == null) return;

        // Movement logic
        Vector3 movement = currentTarget.position - lastHorsePosition;
        float movementSpeed = movement.magnitude / Time.deltaTime;
        lastHorsePosition = currentTarget.position;
        isMoving = movementSpeed > 0.1f;

        float targetFOV = isMoving ? movingFOV : defaultFOV;

        Vector3 targetOffset = currentOffset;

        // ✅ ✅ ✅ Always prioritize zoom override!
        if (isZoomOverridden)
        {
            targetOffset = trigger.zoomOutOffset;
            //Debug.Log("📸 Applying ZOOM OVERRIDE offset.");
        }
        else if (isMounted && currentTarget == horse)
        {
            float movementX = movement.x;

            if (movementX < -0.05f)
            {
                targetOffset = mountedOffsetLeft;
               // Debug.Log("🎥 Movement LEFT (X–)");
            }
            else if (movementX > 0.05f)
            {
                targetOffset = mountedOffset;
                //Debug.Log("🎥 Movement RIGHT (X+)");
            }
            else
            {
                targetOffset = mountedIdleOffset;
                //Debug.Log("🎥 Movement IDLE");
            }
        }
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, followSpeed * Time.deltaTime);

        // 📍 Follow target with offset
        Vector3 targetPosition = currentTarget.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // 🎥 Smooth FOV transition
        if (cam != null)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        }
    }


    public void SetMounted(bool mounted)
    {
        isMounted = mounted;
        //Debug.Log($"Camera State Changed: isMounted = {isMounted}");
    }

    public void SetZoomedOutOffset(Vector3 offset)
    {
        overriddenOffset = offset;
        isZoomOverridden = true;
       //Debug.Log("✅ Zoom override set");
    }

    public void ResetOffset(Vector3 defaultOffset)
    {
        isZoomOverridden = false;
        overriddenOffset = defaultOffset; // Optional: not strictly needed
       // Debug.Log("↩️ Zoom override cleared");
    }

    public void SwitchToFemale()
    {
        currentTarget = female;
        isMounted = false; // Optional reset
        //Debug.Log("📸 Camera now following female.");
    }
} 