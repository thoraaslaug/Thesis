using System.Collections;
using UnityEngine;
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


        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Debug.Log("ThirdPersonController script is running...");

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            
            _snowPathDrawer = GetComponent<SnowPathDrawer>();

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
                _animator.SetFloat(_animIDSpeed, 1.0f);
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

        private IEnumerator HandleDismount()
        {
            yield return new WaitForSeconds(1.5f);

            // Move the player slightly to the side
            //Vector3 dismountPosition = transform.position + transform.right * 1.5f;
           // transform.position = dismountPosition;

            _controller.enabled = true;
            _input.enabled = true;

            _animator.SetFloat("Speed", 0.0f);

            MountSystem mountSystem = horse.GetComponent<MountSystem>();
            if (mountSystem != null)
            {
                mountSystem.isMounted = false;
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
