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
        
        private SnowPathDrawer _snowPathDrawer;


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

            if (_cameraFollow != null)
            {
                _cameraFollow.SetMounted(true);
            }
            
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

                _controller.Move(moveDirection * (_speed * Time.deltaTime));
                
                if (_snowPathDrawer != null)
                {
                    _snowPathDrawer.GetPosition();  // Update path position
                    _snowPathDrawer.DrawSpot();     // Draw footprint at current position
                }

                

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

    }
}
