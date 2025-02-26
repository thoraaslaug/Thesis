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

        private bool _hasAnimator;
        private float _speed;

        private void Start()
        {
            Debug.Log("ThirdPersonController script is running...");

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

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
            }
        }

        public void DismountHorse()
        {
            Debug.Log("Dismounting horse...");
            isMounted = false;

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDRiding, false);
                _animator.SetFloat(_animIDSpeed, 0.0f);
                Debug.Log($"Animator isRiding = {_animator.GetBool(_animIDRiding)}");
            }
        }

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
            if (isMounted) return; // Prevents ground movement while mounted

            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            _speed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _speed);
                _animator.SetFloat(_animIDMotionSpeed, 1.0f);
            }
        }
    }
}
