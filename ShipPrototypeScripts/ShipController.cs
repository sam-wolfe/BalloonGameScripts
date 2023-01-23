using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DefaultNamespace;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ShipController : MonoBehaviour {
    
    // ------------------------------------------------
    // Notes:
    //  Drag is currently set to 1 for drag and angular
    //  drag, so when moving to a new scene dont forget
    //  to update those in the inspector if they aren't
    //  a parameter here.
    //
    // ------------------------------------------------
    
    [Header("Movement Settings")]
    [SerializeField]
    private float speedFactor = 10f;
    [SerializeField]
    [Tooltip("How fast the ship can change altitude, normally.")]
    private float maxSpeed = 40f;
    private float minSpeed = 40f;
    [SerializeField]
    [Tooltip("How fast the ship can change altitude, normally.")]
    private float sailsSpeed = 40f;
    
    [Header("Input")]

    // TODO make interface that inputs share
    [SerializeField] private GamePadInputManager _input;
    
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
    
        
    [Header("Dev")]
    [SerializeField] private float devTargetAltitudeSpeed = 5f;
    
    // PID settings
    [SerializeField] private float pTerm = 0f;
    [SerializeField] private float iTerm = 0f;
    [SerializeField] private float dTerm = 0f;
    
    
    // ########################################

    private Vector2 move;
    private float altitude;
    private float sails;

    private Rigidbody rb;

    public Vector3 targetAltitude { get; private set; }

    private PIDController _pid = new PIDController();
    private PIDAngleController _Xpid = new PIDAngleController();
    private PIDAngleController _Zpid = new PIDAngleController();

    private void Start() {
        rb = GetComponent<Rigidbody>();
        targetAltitude = transform.position;
        
        // Hide that mouse, nobody want to see that shit, put it away you nasty bitch
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {

        updatePIDSettings();
        
        move = _input.move;
        altitude = _input.altitude;
        sails = _input.sails;
    }

    private void updatePIDSettings() {
        _pid.proportionalGain = pTerm;
        _pid.integralGain = iTerm;
        _pid.derivitiveGain = dTerm;
        
        _Xpid.proportionalGain = pTerm;
        _Xpid.integralGain = iTerm;
        _Xpid.derivitiveGain = dTerm;
        
        _Zpid.proportionalGain = pTerm;
        _Zpid.integralGain = iTerm;
        _Zpid.derivitiveGain = dTerm;
    }

    private void FixedUpdate() {
        moveShipHorizonal();
        moveToTargetAltitude();
        applyTargetTorque();
        rotateSails();
        updateTargetAltitue();
        // keepUpright();
        // UpdateUprightForce();
        NewUpdateUprightForce();
        // Debug.Log(transform.forward);
    }

    private void moveShipHorizonal() {
        Vector3 forwardForce = transform.forward * move.y;
        Vector3 sideForce = transform.right * move.x;

        Vector3 direction = forwardForce + sideForce;
        
        rb.AddForce(direction * speedFactor, ForceMode.Force);
        
    }

    private void applyTargetTorque() {
        // TODO make another pid to control torque
    }

    private void moveToTargetAltitude() {

        // We only want the difference in altitute between target and 
        // current position, so zero out the other axis's (axees? axies?)
        float currentHeight = transform.position.y;
        var targetHeight = targetAltitude.y;


        float input = _pid.Update(Time.deltaTime, currentHeight, targetHeight);
        
        if (input > 0) {
            
            // Not related to base PID controller function, limits ascent speed
            var localMin = float.MinValue;
            Vector3 newThrust = new Vector3(0, Mathf.Clamp(input, localMin, maxSpeed), 0);
            //------------------------------------------------------------

            rb.AddForce(newThrust, ForceMode.Force);
        }
    }

    private void rotateSails() {
        // Multiplying by 40 as a hack because I thought turn was too low. //TODO make setting
        rb.AddTorque(Vector3.up * (sails * 40 * speedFactor * Time.deltaTime), ForceMode.Force);
        
        // Quaternion deltaRotation = Quaternion.Euler(Vector3.up * (sails * 40 * Time.deltaTime));
        // rb.MoveRotation(rb.rotation * deltaRotation);
        // // TODO try moveRotation
    }

    private void keepUpright() {
        float currentXrot = transform.eulerAngles.x;
        float currentZrot = transform.eulerAngles.z;
        var targetRot = 0f;


        float inputX = _Xpid.UpdateAngle(Time.deltaTime, currentXrot, 0f);
        float inputZ = _Zpid.UpdateAngle(Time.deltaTime, currentZrot, 0f);
        
        rb.AddTorque(transform.forward * (10 * inputX * Time.deltaTime), ForceMode.Force);
        rb.AddTorque(transform.up * (10 * inputZ * Time.deltaTime), ForceMode.Force);
        
    }

    private void updateTargetAltitue() {
        var shipPosition = transform.position;
        
        targetAltitude = new Vector3(
                shipPosition.x, 
                targetAltitude.y + (altitude * devTargetAltitudeSpeed * Time.deltaTime), 
                shipPosition.z
            );
    }



    //------------------------------------
    // Gizmo stuff
    //------------------------------------
    
    
    // [Header("Dev")]
    // [SerializeField]
    // // Test for gizmo, remove
    // private BoxCollider shipFloorCollider;
    // [SerializeField]
    // // Test for gizmo, remove
    // private bool _gizmoEnabled = false;
    //
    // private void OnDrawGizmos()
    // {
    //     if (_gizmoEnabled) {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawSphere(targetAltitude, 1f);    
    //     }
    // }
    //
    // private void setAltitudeGizmoInitialPos() {
    //     if (_gizmoEnabled) {
    //         var col = shipFloorCollider;
    //
    //         Debug.Log(col.size.x);
    //         Debug.Log(col.size.z);
    //
    //         targetAltitude = new Vector3(
    //             transform.position.x - col.size.x / 2, 0, transform.position.z - col.size.z / 2
    //         );
    //     }
    // }
    
    
    [SerializeField] private float _uprightStrength = 10f;
    [SerializeField] private float _uprightStrengthDamper = 0.5f;
    public Quaternion ValetShortestRotation(Quaternion currentRotation) {
        Vector3 targetRotVec = new Vector3(transform.forward.x, 0f, transform.forward.z);
        var targetRotation = Quaternion.LookRotation(targetRotVec, Vector3.up);
        
        float angle = Quaternion.Angle(currentRotation, targetRotation);
        if (angle > 180f) {
            Debug.Log("Overlimit");
            targetRotation = Quaternion.Inverse(targetRotation);
        }
        Quaternion toGoal = Quaternion.Inverse(currentRotation) * targetRotation;
        
        // return Quaternion.Inverse(currentRotation) * targetRotation;
        return toGoal;
    }

    private Quaternion ShortestRotation(Quaternion to, Quaternion from) {
        if (Quaternion.Dot(to, from) < 0) {
            return to * Quaternion.Inverse(Multiply(from, -1));
        }
        else {
            return to * Quaternion.Inverse(from);
        }
    }
    
    private Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    public void NewUpdateUprightForce() {
        Quaternion currentRotation = transform.rotation;
        Vector3 targetRotVec = new Vector3(transform.forward.x, 0f, transform.forward.z);
        Quaternion targetRotation = Quaternion.LookRotation(targetRotVec, Vector3.up);
        Quaternion toGoal = ShortestRotation(targetRotation, currentRotation);
        
        float rotDegrees;
        Vector3 rotAxis;
        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;
        
        rb.AddTorque((rotAxis * (rotRadians * _uprightStrength))- (rb.angularVelocity * _uprightStrengthDamper));

    }

    public void UpdateUprightForce() {
        Quaternion currentRotation = transform.rotation;
        Quaternion toGoal = ValetShortestRotation(currentRotation);
        
        float rotDegrees;
        Vector3 rotAxis;
        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        
        rotAxis.Normalize();
        
        if (rotDegrees > 180) {
            rotDegrees -= 360;
            rotAxis = -rotAxis;
        }

        float rotRadians = rotDegrees * Mathf.Deg2Rad;
        
        Debug.Log(rotAxis);
        // Debug.Log((rotAxis * (rotRadians * _uprightStrength)) - (rb.angularVelocity * _uprightStrengthDamper));
        try {
            rb.AddTorque((rotAxis * (rotRadians * _uprightStrength)));

        }
        catch { }
        // rb.AddTorque((rotAxis * (rotRadians * _uprightStrength)));
        
    }

}


public class RotationInterpolation : MonoBehaviour
{
    public Quaternion ShortestRotation(Quaternion targetRotation, Quaternion currentRotation)
    {
        Quaternion rotationDifference = Quaternion.Inverse(currentRotation) * targetRotation;
        float angle;
        Vector3 axis;
        rotationDifference.ToAngleAxis(out angle, out axis);
    
        if (angle > 180) {
            angle -= 360;
        }
    
        return Quaternion.AngleAxis(angle, axis);
    }
    
    public Quaternion ValetShortestRotation(Quaternion targetRotation, Quaternion currentRotation)
    {
        return Quaternion.Inverse(currentRotation) * targetRotation;
    }

    public void UpdateUprightForce(float elapsed) {
        Quaternion currentRotation = transform.rotation;
        Quaternion toGoal = ValetShortestRotation(
            Quaternion.LookRotation(transform.forward, transform.up), transform.rotation);
        
        float rotDegrees;
        Vector3 rotAxis;
        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        
        // if (angle > 180) {
        //     angle -= 360;
        // }
        
    }
}
