using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player Settings")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        public float SpeedChangeRate = 10.0f;

        [Header("Riding System")]
        public bool isMounted = false;

        // Animator Hash IDs
        private int _animIDSpeed;
        private int _animIDMotionSpeed;
        private int _animIDRiding;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        
        private HorseCameraFollow _cameraFollow;

        private bool _hasAnimator;
        private float _speed;
        
        public float turnSpeed = 10f;

        
        private SnowPathDrawer _snowPathDrawer;

        public AudioSource audioSource;
        public AudioClip clip;

        public HorseController horse;
        public ThirdPersonController player;
        public Animator horseAnimator;
        
        private float gravity = -9.81f;  // Standard gravity value
        private float verticalVelocity = 0f; // Stores downward movement
        private float groundCheckDistance = 0.1f; // How close the player has to be to be considered "on the ground"
        public PlayableDirector kissTimeline;
        public Cinemachine.CinemachineVirtualCamera timelineCamera;  // 🎥 Assign in Inspector
        public Cinemachine.CinemachineVirtualCamera gameplayCamera;  // 🎮 main camera

        public GameObject timelineDummy; // assign in inspector
        private GameObject activeDummy;
        public GameObject hair;
        private static bool hasPlayedReturnRideNarration = false;
        public SnowstormTrigger snowstormTrigger;
        private MountSystem mountSystem;
        public Volume postProcessingVolume; // Assign in inspector
        private DepthOfField dof;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Debug.Log("ThirdPersonController script is running...");

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            
            _snowPathDrawer = GetComponent<SnowPathDrawer>();
            if (postProcessingVolume != null)
            {
                postProcessingVolume.profile.TryGet(out dof);
            }
            AssignAnimationIDs();
        }

        private void Update()
        {
            if (isMounted)
            {
                HandleRiding();
            }
            else
            {
                Move();
            }
        }


        public void PlayFootstep()
        {
            if (audioSource != null && clip != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(clip);
            }
        }
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDRiding = Animator.StringToHash("isRiding");
        }

        public void MountHorse()
        {
            Debug.Log("Mounting horse...");
            isMounted = true;
            
                
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDRiding, true);
                _animator.SetFloat(_animIDSpeed, 1.0f);  // Force transition
                Debug.Log($"Animator isRiding = {_animator.GetBool(_animIDRiding)}");
               // _cameraFollow.SetMounted(true);
            }

           /* if (_cameraFollow != null)
            {
                _cameraFollow.SetMounted(true);
            }*/
            
        }

      /*  public void DismountHorse()
        {
            Debug.Log("Dismounting horse...");
            isMounted = false;

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDRiding, false);
                _animator.SetFloat(_animIDSpeed, 0.0f);
                Debug.Log($"Animator isRiding = {_animator.GetBool(_animIDRiding)}");
            }
        }*/

        private void HandleRiding()
        {
            Debug.Log("Player is riding...");

            if (_hasAnimator)
            {
               // _animator.SetFloat(_animIDSpeed, 1.0f);
                _animator.SetBool(_animIDRiding, true);
                Debug.Log($"Animator Speed: {_animator.GetFloat(_animIDSpeed)}");
            }
        }

        private void Move()
        {
            if (_hasAnimator)
            {
                float animSpeed = _animator.GetFloat("Speed");
               // Debug.Log($"Animator Speed Value: {animSpeed}");
            }

            if (isMounted) return; // Prevent movement while mounted

            Vector2 inputDirection = _input.move; // Get player input

            if (inputDirection.magnitude > 0.1f) // If the player is moving
            {
               // Debug.Log($"Player Input: {inputDirection.x}, {inputDirection.y}"); // Debug input
                float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed; 
                _animator.SetBool("IsWalking", true);

                _speed = Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * SpeedChangeRate);

                Vector3 moveDirection = new Vector3(inputDirection.x, 0.0f, inputDirection.y);
                moveDirection.Normalize();
                
                if (IsGrounded())
                {
                    verticalVelocity = 0f;
                }
                else
                {
                    verticalVelocity += gravity * Time.deltaTime;
                }
                
                Vector3 movement = moveDirection * (_speed * Time.deltaTime);
                movement.y = verticalVelocity * Time.deltaTime;
                //_controller.Move(moveDirection * (_speed * Time.deltaTime));
                _controller.Move(movement);
                AdjustToTerrain();

                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                }
               // Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                
               // if (_snowPathDrawer != null)
                //{
                  //  _snowPathDrawer.GetPosition();  // Update path position
                   // _snowPathDrawer.DrawSpot();     // Draw footprint at current position
               // }

                

                if (_hasAnimator)
                {
                    _animator.SetFloat("Speed", _speed); // Update Animator Speed
                }
            }
            else
            {
                _speed = 0.0f;
                if (_hasAnimator)
                {
                    _animator.SetFloat("Speed", 0.0f); // Reset speed when not moving
                    _animator.SetBool("IsWalking", false);
                }
            }
        }
        public void DismountHorse()
        {
            if (!isMounted) return; // Prevent dismounting if already on the ground

            Debug.Log("Dismounting...");

            isMounted = false; // Update state

            player.enabled = true;
            horse.enabled = false;
            _input.move = Vector2.zero; // reset input just in case
            _input.enabled = false;   
            //mountSystem.DetachReins();

            // Enable Animator (if disabled while riding)
            if (!_animator.enabled)
            {
                _animator.enabled = true;
            }

            // Set animation parameters
            _animator.SetBool("IsRiding", false); // Exit Riding State
            _animator.SetTrigger("Dismount"); // Play Dismount Animation

            if (horse != null)
            {
                horseAnimator.SetFloat("Speed", 0.0f);
                horseAnimator.SetBool("Galloping", false);
            }

            // Disable movement while dismounting
            _controller.enabled = false;

            // Delay movement until animation completes
            StartCoroutine(HandleDismount());
        }

       /* private IEnumerator HandleDismount()
        {
            // Move and reposition player
            Vector3 dismountPosition = transform.position + transform.right * 1.5f;
            transform.position = dismountPosition;

            // Align dummy with player and activate it
            timelineDummy.transform.position = transform.position;
            timelineDummy.transform.rotation = transform.rotation;
            timelineDummy.SetActive(true);

            // Hide player mesh / disable input
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            hair.SetActive(false);
            _controller.enabled = false;
            _input.enabled = false;

            // Play timeline
            kissTimeline.Play();

            // Wait until timeline is done
            while (kissTimeline.state == PlayState.Playing)
                yield return null;
            
            if (!GameState.hasPlayedReturnRideNarration)
            {
                StartReturnRideNarration();
                hasPlayedReturnRideNarration = true;
            }
           // StartReturnRideNarration();
            transform.position = timelineDummy.transform.position;
            transform.rotation = timelineDummy.transform.rotation;
            // Deactivate dummy
            timelineDummy.SetActive(false);
            snowstormTrigger.StartSnowstorm();

            // Restore player visuals and control
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            hair.SetActive(true);
            _controller.enabled = true;
            _input.enabled = true;

            _animator.SetFloat("Speed", 0.0f);

            MountSystem mountSystem = horse.GetComponent<MountSystem>();
            if (mountSystem != null)
                mountSystem.DismountMale();
        }*/
       
       private IEnumerator HandleDismount()
{
    
     //this.mountSystem.DetachReins();

    // Move and reposition player
    Vector3 dismountPosition = transform.position + transform.right * 1.5f;
    transform.position = dismountPosition;

    // Align dummy with player and activate it
    timelineDummy.transform.position = transform.position;
    timelineDummy.transform.rotation = transform.rotation;
    timelineDummy.SetActive(true);
    if (dof != null)
        dof.active = false;

    // Hide player mesh / disable input
    GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
    hair.SetActive(false);
    _controller.enabled = false;
    _input.enabled = false;

    // 🎥 Switch to timeline camera
    if (timelineCamera != null)
        timelineCamera.Priority = 20;

    if (gameplayCamera != null)
        gameplayCamera.Priority = 5;

    // Optional slow-mo (uncomment if desired)
    //Time.timeScale = 0.4f;
    //Time.fixedDeltaTime = 0.02f * Time.timeScale;

    // Play timeline
    kissTimeline.Play();

    // Wait until timeline is done
    while (kissTimeline.state == PlayState.Playing)
        yield return null;

    // Optional: Restore time
    //Time.timeScale = 1f;
    //Time.fixedDeltaTime = 0.02f;

    // ✅ Restore main camera after timeline
    if (timelineCamera != null)
        timelineCamera.Priority = 5;

    if (gameplayCamera != null)
        gameplayCamera.Priority = 20;
    if (dof != null)
        dof.active = true;

    // Return player to dummy position
    transform.position = timelineDummy.transform.position;
    transform.rotation = timelineDummy.transform.rotation;
    timelineDummy.SetActive(false);

    // Restore player visuals and control
    GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
    hair.SetActive(true);
    _controller.enabled = true;
    _input.enabled = true;

    _animator.SetFloat("Speed", 0.0f);

    MountSystem mountSystem = horse.GetComponent<MountSystem>();
    if (mountSystem != null)
        mountSystem.DismountMale();

    // ⛄ Trigger narration + snowstorm
    if (!GameState.hasPlayedReturnRideNarration)
    {
        StartReturnRideNarration();
        hasPlayedReturnRideNarration = true;
    }

    snowstormTrigger.StartSnowstorm();
}
        
        void StartReturnRideNarration()
        {
            string[] returnLines = new string[]
            {
                "She said yes…",
                "The snow feels heavier now, I can barely see",
                "I must keep moving",
                "My hands are numb. No matter. She'll be waiting for me, I'll be back on Christmas Eve.",
                "It’s darker than I remember… the bridge, the sky… the world.",
                "I will see her again. I will see her again..."
            };

            var narration = FindObjectOfType<NarrationTextManager>();
            if (narration != null)
            {
                narration.StartNarration(returnLines);
            }
            else
            {
                Debug.LogWarning("NarrationTextManager not found in scene.");
            }
        }


        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        }

        void AdjustToTerrain()
        {
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                Vector3 position = transform.position;
                float terrainHeight = terrain.SampleHeight(position) + terrain.transform.position.y;

                // Ensure we only move up if needed, and smoothly move down
                if (position.y < terrainHeight)
                {
                    position.y = Mathf.Lerp(position.y, terrainHeight, Time.deltaTime * 10f); // Smooth transition
                    _controller.Move(new Vector3(0, position.y - transform.position.y, 0)); // Move using CharacterController
                }
            }
        }

    }
}
