using System;
using UnityEngine;

namespace FPSController {

    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour {

        // TODO may use, may not use
        public bool CanMove { get; private set; } = true;

        [Header("Input Interface")] [SerializeField]
        private InputManager _input;


        [Header("Movement Settings")]

        // [SerializeField, Range(1, 10)]
        [SerializeField]
        [Tooltip("Move speed of the character in m/s")]
        private float MoveSpeed = 4.0f;

        [SerializeField] [Tooltip("Sprint speed of the character in m/s")]
        private float SprintSpeed = 6.0f;

        [SerializeField] [Tooltip("Acceleration and deceleration")]
        private float SpeedChangeRate = 10.0f;

        
        
        [Space(10)] 
        [Header("Jump Settings")] 
        [SerializeField] [Tooltip("The height the player can jump")]
        private float JumpHeight = 1.2f;

        [SerializeField] [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        private float Gravity = -15.0f;

        [SerializeField] [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        private float JumpTimeout = 0.1f;

        [SerializeField] [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        private float FallTimeout = 0.15f;

        
        
        [Space(10)]
        [Header("Look Settings / Cinemachine")]
        
        [SerializeField]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        private GameObject CinemachineCameraTarget;

        [SerializeField] [Tooltip("Rotation speed of the character")]
        private float RotationSpeed = 1.0f;

        [SerializeField] [Tooltip("How far in degrees can you move the camera down")]
        private float BottomClamp = 80.0f;

        [SerializeField] [Tooltip("How far in degrees can you move the camera up")]
        private float TopClamp = -70.0f;

        
        
        [Space(10)]
        [Header("Player Grounded")]
        [SerializeField] [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        private bool Grounded = true;

        [SerializeField] [Tooltip("Useful for rough ground")] 
        private float GroundedOffset = -0.14f;

        [SerializeField] [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        private float GroundedRadius = 0.5f;

        [SerializeField] [Tooltip("What layers the character uses as ground")]
        private LayerMask GroundLayers;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // Threshold, deadzone, before movement is detected (in camera only I think)
        private const float _threshold = 0.01f;


        // Caching
        private CharacterController _controller;
        private GameObject _mainCamera;



        private void Awake() {
            // get a reference to our main camera
            if (_mainCamera == null) {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start() {
            // TODO do on focus, event based, possibly not in this class
            Cursor.lockState = CursorLockMode.Locked;

            if (_input == null) throw new Exception("Input manager is not set on FPS controller.");

            _controller = gameObject.GetComponent<CharacterController>();

        }

        void Update() {
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void Move() {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, 
            // and is cheaper than magnitude if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity 
            float currentHorizontalSpeed =
                new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            // This offset is because we are using Lerp in a non-best practice way. By having currentHorizontalSpeed
            // as the first argument of Lerp, we will technically never arrive at the targetSpeed, only approach it 
            // in small amounts forever. This offset is when we decide we are "close enough" and no longer accelerate
            // and just set the speed to the targetSpeed.
            float speedOffset = 0.1f;

            // TODO not sure why Unity FPS controller has a bool for whether it is analog or not. Need to find out why
            // float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            float inputMagnitude = _input.move.magnitude;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset) {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                // TODO bug, if you swap directions too fast you HALT completely for a second. Maybe just rip out?
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, 
            // and is cheaper than magnitude if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero) {
                // move relative to to the forward direction of the camera (in this case the player as this 
                // controller keeps the camera static about the y axis and rotates the whole player)
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }


            // if (inputDirection.x == 0 && inputDirection.y == 0 && inputDirection.z == 0 && _verticalVelocity != 0) {
            //     
            //     // move the player
            //     _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
            //                      new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            //     
            // } else 
            
            // Only move the player when input is recieved, or when not on the ground. That way 
            // moving platforms will work implicitly by setting the player as a child of the platform.
            if (inputDirection != Vector3.zero || !Grounded || _verticalVelocity > 0) {
                // Debug.Log(inputDirection);
                // Debug.Log(inputDirection != Vector3.zero);
                // Debug.Log(_verticalVelocity);
                // Debug.Log(_verticalVelocity != 0f);
                

                // move the player
                _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                
            }
            
            // if (inputDirection.x != 0 && inputDirection.y != 0 && inputDirection.z != 0) {
            //     // move the player
            //     _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
            //                      new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            // }

        }

        private void LateUpdate() {
            CameraRotation();
        }

        private bool isCurrentDeviceMouse => _input.currentControlScheme == "KeyboardMouse";


        private void CameraRotation() {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold) {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = isCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier * 2;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, TopClamp, BottomClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void GroundedCheck() {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            // Checks if any layers in GroundLayers is contained within the sphere, if it is then we are on the
            // ground.
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void JumpAndGravity() {
            if (Grounded) {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                // TODO | Bug, if standing on an object that isnt a ground layer,
                // TODO | when you step off of it you will blast downwards like a comet
                if (_verticalVelocity < 0.0f) {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f) {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f) {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f) {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity) {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }


        private void OnDrawGizmosSelected() {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

    }

}
