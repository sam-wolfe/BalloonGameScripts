using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShipController : MonoBehaviour {
    
    [SerializeField]
    private float speed = 10f;

    // TODO make interface that inputs share
    [SerializeField] private GamePadInputManager _input;
    
    private Vector2 move;
    private float altitude;
    private float sails;

    private Rigidbody rb;

    private Vector3 targetAltitude;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        setAltitudeGizmoInitialPos();
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

        if (distanceToTarget.y > 0) {
            var newPos = distanceToTarget * (speed * Time.deltaTime);
        
            rb.AddForce(newPos * speed, ForceMode.Force);
        }
    }

    private void rotateSails() {
        // Multiplying by 40 as a hack because I thought turn was too low. //TODO make setting
        rb.AddTorque(Vector3.up * (sails * 40 * speed * Time.deltaTime), ForceMode.Force);
    }

    private void updateTargetAltitue() {
        var shipPosition = transform.position;
        var col = shipFloorCollider;
        
        targetAltitude = new Vector3(
                shipPosition.x-col.size.x/2, 
                targetAltitude.y + (altitude * speed * Time.deltaTime), 
                shipPosition.z-col.size.z/2
            );
    }
    
    
    
    //------------------------------------
    // Gizmo stuff
    //------------------------------------
    
    
    
    [SerializeField]
    // Test for gizmo, remove
    private BoxCollider shipFloorCollider;
    [SerializeField]
    // Test for gizmo, remove
    private bool _gizmoEnabled = false;

    private void OnDrawGizmosSelected()
    {
        if (_gizmoEnabled) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetAltitude, 1f);    
        }
    }

    private void setAltitudeGizmoInitialPos() {
        if (_gizmoEnabled) {
            var col = shipFloorCollider;

            Debug.Log(col.size.x);
            Debug.Log(col.size.z);

            targetAltitude = new Vector3(
                transform.position.x - col.size.x / 2, 0, transform.position.z - col.size.z / 2
            );
        }
    }

}
