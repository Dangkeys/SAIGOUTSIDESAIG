using InputSystem;
using UnityEngine;
using VContainer;

namespace Core.Player
{

    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Components")]
        public CharacterController CharacterController { get; private set; }

        [Header("Player Movement")]
        [SerializeField]
        private float _moveSpeed = 7.5f;

        [SerializeField]
        private float _walkMoveSpeed = 3.75f;

        [SerializeField]
        private float _crouchMoveSpeed = 1.75f;

        [SerializeField]
        private float _sprintMoveSpeed = 5.75f;

        [SerializeField]
        private float _jumpHeight = 0.0f;

        [Header("Camera Look")]
        [SerializeField]
        private float _lookSpeed = 0.12f;

        [SerializeField]
        private float _lookXLimit = 60.0f;

        [Header("Physics")]
        [SerializeField]
        private float _gravity = -15.0f;

        private Transform _cameraTransform;

        private GameInputReader _inputReader;

        [field: SerializeField]
        public Transform PlayerEyesTransform;

        [SerializeField]
        private float _crouchingHeight = 3f;

        private float _standingHeight;
        private Vector3 _initialCameraPosition;

        [SerializeField]
        private float _crouchLerpSpeed = 7f;

        public bool IsCrouching { get; private set; } = false;

        private Vector3 _playerVelocity;
        private bool isGrounded;
        private float rotationX = 0;

        private Vector2 moveInput;
        private Vector2 lookInput;
        public bool IsSprinting { get; private set; } = false;
        public bool IsExhausted { get; private set; } = false;

        [SerializeField]
        private float _staminaDepleteRate = .5f;

        [SerializeField]
        private float _staminaRegenRate = .5f;

        [SerializeField]
        public float CurrentStamina;

        [SerializeField]
        private float _maxStamina = 100f;
        private float _minStamina = 0f;

        [field: SerializeField]
        public float HorizontalSpeed;

        [Inject]
        private void Construct(GameInputReader inputReader)
        {
            _inputReader = inputReader;
        }

        void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
            _standingHeight = CharacterController.height;
            Camera mainCamera = Camera.main;
            _initialCameraPosition = mainCamera.transform.localPosition;
            _cameraTransform = mainCamera.transform;
            CurrentStamina = _maxStamina;
        }

        private void Start()
        {
            _inputReader.MoveEvent += OnMove;
            _inputReader.JumpEvent += OnJump;
            _inputReader.LookEvent += OnLook;
            _inputReader.CrouchEvent += OnCrouch;
            _inputReader.SprintEvent += OnSprint;
        }

        private void OnSprint(bool isSprinting)
        {
            IsSprinting = isSprinting;
        }

        private void OnCrouch(bool isCrouching)
        {
            IsCrouching = isCrouching;
        }

        private void OnMove(Vector2 vector)
        {
            moveInput = vector;
        }

        private void OnJump()
        {
            if (isGrounded)
            {
                _playerVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
        }

        private void OnLook(Vector2 look)
        {
            lookInput = look;
        }

        void Update()
        {
            HandleGrounded();
            HandleMovement();
            HandleGravity();
            HandleCameraLook();
            HandleCrouch();
            HandleSprint();
        }

        private void HandleSprint()
        {
            if (IsPerformSprinting())
            {
                CurrentStamina -= _staminaDepleteRate * Time.deltaTime;
            }
            else if (CurrentStamina < _maxStamina && !IsSprinting)
            {
                CurrentStamina += _staminaRegenRate * Time.deltaTime;
            }
            CurrentStamina = Mathf.Clamp(CurrentStamina, _minStamina, _maxStamina);
            IsExhausted =
                CurrentStamina == _minStamina || (IsExhausted && CurrentStamina < _maxStamina);
        }

        public bool IsPerformSprinting()
        {
            return IsSprinting && !IsExhausted;
        }

        private void HandleCrouch()
        {
            var heightTarget = IsCrouching ? _crouchingHeight : _standingHeight;
            var halfHeightDiff = new Vector3(0, (_standingHeight - heightTarget) / 2, 0);
            var newCameraPosition = _initialCameraPosition - halfHeightDiff;

            _cameraTransform.localPosition = Vector3.Lerp(
                _cameraTransform.localPosition,
                newCameraPosition,
                _crouchLerpSpeed * Time.deltaTime
            );
            CharacterController.height = Mathf.Lerp(
                CharacterController.height,
                heightTarget,
                _crouchLerpSpeed * Time.deltaTime
            );
        }

        private void HandleGrounded()
        {
            isGrounded = CharacterController.isGrounded;
            if (isGrounded && _playerVelocity.y < 0)
            {
                _playerVelocity.y = -2f;
            }
        }

        private void HandleMovement()
        {
            _moveSpeed =
                IsCrouching == true
                    ? _crouchMoveSpeed
                    : ((IsSprinting && !IsExhausted) == true ? _sprintMoveSpeed : _walkMoveSpeed);
            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            CharacterController.Move(moveDirection * _moveSpeed * Time.deltaTime);

            var horizontalVelocity = new Vector3(
                CharacterController.velocity.x,
                0,
                CharacterController.velocity.z
            );
            HorizontalSpeed = horizontalVelocity.magnitude;
        }

        private void HandleGravity()
        {
            _playerVelocity.y += _gravity * Time.deltaTime;
            CharacterController.Move(_playerVelocity * Time.deltaTime);
        }

        private void HandleCameraLook()
        {
            float mouseX = lookInput.x * _lookSpeed;
            float mouseY = lookInput.y * _lookSpeed;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -_lookXLimit, _lookXLimit);

            _cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void OnDestroy()
        {
            _inputReader.MoveEvent -= OnMove;
            _inputReader.JumpEvent -= OnJump;
            _inputReader.LookEvent -= OnLook;
        }

        public void EnablePlayerMovement(bool shouldEnable = true)
        {
            CursorController.ShowCursor(!shouldEnable);
            enabled = shouldEnable;
        }
    }


}
