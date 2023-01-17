using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class PhysicsFPSController : MonoBehaviour
{
    
    [Header("Input Interface")] 
    [SerializeField] private PlayerControlInputManager _input;

    [Header("Player Settings")] 
    [SerializeField] private float speed = 10;
    
    
    // Gotta vet all these settings again ------------------
    
    [SerializeField] [Tooltip("Rotation speed of the character(may have to change)")]
    private float RotationSpeed = 1.0f;
    [SerializeField] [Tooltip("How far in degrees can you move the camera down")]
    private float BottomClamp = 80.0f;

    [SerializeField] [Tooltip("How far in degrees can you move the camera up")]
    private float TopClamp = -70.0f;
    
    // Threshold, deadzone, before movement is detected (in camera only I think)
    private const float _threshold = 0.01f;
    private float _rotationVelocity;
    
    [SerializeField]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    private GameObject CinemachineCameraTarget;
    
    // ---------------------------------------------------
    
    
    private Rigidbody playerRB;
    
    void Start() {
        
        if (_input == null) throw new Exception("Input manager is not set on FPS controller.");

        playerRB = GetComponent<Rigidbody>();
        if (playerRB == null) throw new Exception(
            "PhysicsFPSController could not get a rigidbody. " +
            "PhysicsFPSController must be attached to a GameObject with a rigidbody.");

    }

    void FixedUpdate() {
        ProcessMove();
        GroundedCheck();
        ProcessJump();
        // TODO proces rotation when ship rotates
    }
    
    private void LateUpdate() {
        CameraRotation();
    }

    private void GroundedCheck() {
        return;
    }
    
    // TODO improve jumping
    private float timeSinceLastJump;

    private void ProcessJump() {
        timeSinceLastJump += Time.deltaTime;
        if (_input.jump && timeSinceLastJump > 1.5f) {
            timeSinceLastJump = 0;
            playerRB.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }

    private void ProcessMove() {
        var move = _input.move * speed;
        var p = playerRB.gameObject.transform.position;

        var newX = transform.right * move.x;
        var newY = transform.forward * move.y;
    
        var newPos = transform.right * move.x + transform.forward * move.y;

        if (newPos != Vector3.zero) {

            // TODO clean up
            playerRB.MovePosition(new Vector3(p.x+newPos.x, p.y, p.z+newPos.z));
        }
    } 
    
    
    // Camera ---------------------------------------------
    
    private float _cinemachineTargetPitch;
    private void CameraRotation() {
        // if there is an input
        if (_input.look.sqrMagnitude >= _threshold) {
            //Don't multiply mouse input by Time.deltaTime
            // float deltaTimeMultiplier = isCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            float deltaTimeMultiplier = 1.0f;
            // float deltaTimeMultiplier = Time.deltaTime;
            
            // Debug.Log(_input.look.x);

            _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier * 2;
            _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, TopClamp, BottomClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(
                _cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
