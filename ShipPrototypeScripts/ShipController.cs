using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    private float speed = 10f;
    [SerializeField]
    [Tooltip("How fast the ship can change altitude, normally.")]
    private float maxSpeed = 40f;
    private float minSpeed = 40f;
    [Tooltip("Add extra delay to simulate inertia, without changing mass of the ship.")]
    private float inertialDelay = 5f;
    
    [Header("Input")]

    // TODO make interface that inputs share
    [SerializeField] private GamePadInputManager _input;

    [Header("Configuration")]
    
    [SerializeField]
    public BoxCollider shipFloor;
    
    private Vector2 move;
    private float altitude;
    private float sails;

    private Rigidbody rb;

    public Vector3 targetAltitude { get; private set; }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        // setAltitudeGizmoInitialPos();
    }

    void Update() {
        move = _input.move;
        altitude = _input.altitude;
        sails = _input.sails;
    }

    private void FixedUpdate() {
        moveShipHorizonal();
        moveToTargetAltitude();
        rotateSails();
        updateTargetAltitue();
    }

    private void moveShipHorizonal() {
        Vector3 forwardForce = transform.forward * move.y;
        Vector3 sideForce = transform.right * move.x;

        Vector3 direction = forwardForce + sideForce;
        
        rb.AddForce(direction * speed, ForceMode.Force);
        
    }
    
    private void moveToTargetAltitude() {

        // We only want the difference in altitute between target and 
        // current position, so zero out the other axis's (axees? axies?)
        var heightVector = new Vector3(0, transform.position.y, 0);
        var targetAltitudeY = new Vector3(0, targetAltitude.y, 0);

        var distanceToTarget = targetAltitudeY - heightVector;

        // Debug.Log(distanceToTarget);
        
        // Problem is that it is riding low when more mass added instead of slowly rising to target
        // or slowly falling to the ground is because I am trying to curve velocity with relative
        // distance to the target already, AND I am trying to add curves with math.

        // if (distanceToTarget.y > 0) {
            // TODO This is the not fixed code, has its charm, but not what I am going for.
            var newPos = distanceToTarget * speed;
            
            // TODO this is the fixed code
            // var newPos = Vector3.up * speed;

            Debug.Log(newPos.y);


            var localMin = float.MinValue;
            
            if (newPos.y < 0) localMin = -5f;
            
            // Limit ascent speed
            newPos.y = Mathf.Clamp(newPos.y, localMin, maxSpeed);

            rb.AddForce(newPos, ForceMode.Force);
        // }
    }

    private void rotateSails() {
        // Multiplying by 40 as a hack because I thought turn was too low. //TODO make setting
        rb.AddTorque(Vector3.up * (sails * 40 * speed * Time.deltaTime), ForceMode.Force);
    }

    private void updateTargetAltitue() {
        var shipPosition = transform.position;
        var col = shipFloor;
        
        targetAltitude = new Vector3(
                shipPosition.x, 
                targetAltitude.y + (altitude * speed * Time.deltaTime), 
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

}
